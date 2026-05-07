import apiClient from './api';
import type {
  Ingredient,
  IngredientApi,
  IngredientCreateDto,
  IngredientListResponse,
  IngredientUpdateDto,
} from '../types';

type NutritionMeta = {
  unit: string;
};

const NUTRITION_STORAGE_KEY = 'ingredient_nutrition_meta';

const readNutritionMeta = (): Record<string, NutritionMeta> => {
  try {
    const raw = localStorage.getItem(NUTRITION_STORAGE_KEY);
    if (!raw) return {};
    return JSON.parse(raw) as Record<string, NutritionMeta>;
  } catch {
    return {};
  }
};

const writeNutritionMeta = (meta: Record<string, NutritionMeta>) => {
  localStorage.setItem(NUTRITION_STORAGE_KEY, JSON.stringify(meta));
};

const toIngredient = (apiIngredient: IngredientApi): Ingredient => {
  const meta = readNutritionMeta()[String(apiIngredient.id)];
  return {
    id: apiIngredient.id,
    name: apiIngredient.name,
    caloriesPerUnit: apiIngredient.quantity,
    foodEntityId: apiIngredient.foodEntityId,
    unit: meta?.unit ?? 'unit',
    protein: apiIngredient.protein,
    carbs: apiIngredient.carbs,
    fat: apiIngredient.fat,
  };
};

const saveNutritionMeta = (id: number, data: IngredientCreateDto | IngredientUpdateDto) => {
  const allMeta = readNutritionMeta();
  allMeta[String(id)] = {
    unit: data.unit,
  };
  writeNutritionMeta(allMeta);
};

const parsePagination = (header: string | null, itemCount: number) => {
  if (!header) {
    return {
      totalCount: itemCount,
      pageSize: itemCount || 1,
      currentPage: 1,
      totalPages: 1,
    };
  }

  try {
    const parsed = JSON.parse(header) as {
      totalCount: number;
      pageSize: number;
      currentPage: number;
      totalPages: number;
    };
    return parsed;
  } catch {
    return {
      totalCount: itemCount,
      pageSize: itemCount || 1,
      currentPage: 1,
      totalPages: 1,
    };
  }
};

export const ingredientService = {
  async getAllIngredients(params?: {
    page?: number;
    pageCount?: number;
    query?: string;
  }): Promise<IngredientListResponse> {
    const response = await apiClient.get('/v1/ingredients', {
      params: {
        page: params?.page ?? 1,
        pageCount: params?.pageCount ?? 10,
        query: params?.query ?? '',
      },
    });

    const rawItems = (response.data?.value ?? []) as IngredientApi[];
    const items = rawItems.map(toIngredient);
    const pagination = parsePagination(response.headers['x-pagination'] ?? null, items.length);

    return {
      items,
      ...pagination,
    };
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<IngredientApi>(`/v1/ingredients/${id}`);
    return toIngredient(response.data);
  },

  async createIngredient(data: IngredientCreateDto, foodEntityId: number): Promise<Ingredient> {
    const response = await apiClient.post<IngredientApi>('/v1/ingredients', {
      name: data.name,
      quantity: data.caloriesPerUnit,
      protein: data.protein,
      carbs: data.carbs,
      fat: data.fat,
      foodEntityId,
    });

    const created = toIngredient(response.data);
    saveNutritionMeta(created.id, data);
    return { ...created, ...data };
  },

  async updateIngredient(id: number, data: IngredientUpdateDto, foodEntityId: number): Promise<Ingredient> {
    const response = await apiClient.put<IngredientApi>(`/v1/ingredients/${id}`, {
      name: data.name,
      quantity: data.caloriesPerUnit,
      protein: data.protein,
      carbs: data.carbs,
      fat: data.fat,
      foodEntityId,
    });

    const updated = toIngredient(response.data);
    saveNutritionMeta(updated.id, data);
    return { ...updated, ...data };
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
