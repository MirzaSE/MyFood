import apiClient from './api';
import type {
  Ingredient,
  IngredientApiDto,
  IngredientFormValues,
  IngredientMetadata,
} from '../types/ingredient';

const STORAGE_KEY = 'ingredient-metadata';

const defaultMetadata: IngredientMetadata = {
  unit: 'g',
  caloriesPerUnit: 1,
  protein: 1,
  carbs: 1,
  fat: 1,
};

const loadMetadata = (): Record<string, IngredientMetadata> => {
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) {
    return {};
  }

  try {
    return JSON.parse(raw) as Record<string, IngredientMetadata>;
  } catch {
    return {};
  }
};

const saveMetadataStore = (store: Record<string, IngredientMetadata>) => {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(store));
};

const toIngredient = (apiIngredient: IngredientApiDto, metadataStore: Record<string, IngredientMetadata>): Ingredient => {
  const metadata = metadataStore[String(apiIngredient.id)] ?? defaultMetadata;

  return {
    id: apiIngredient.id,
    name: apiIngredient.name ?? 'Unnamed ingredient',
    foodEntityId: apiIngredient.foodEntityId,
    unit: metadata.unit,
    caloriesPerUnit: metadata.caloriesPerUnit,
    protein: metadata.protein,
    carbs: metadata.carbs,
    fat: metadata.fat,
  };
};

const saveMetadata = (ingredientId: number, values: IngredientFormValues) => {
  const store = loadMetadata();
  store[String(ingredientId)] = {
    unit: values.unit,
    caloriesPerUnit: values.caloriesPerUnit,
    protein: values.protein,
    carbs: values.carbs,
    fat: values.fat,
  };
  saveMetadataStore(store);
};

const removeMetadata = (ingredientId: number) => {
  const store = loadMetadata();
  delete store[String(ingredientId)];
  saveMetadataStore(store);
};

const getExistingFoodId = async (): Promise<number> => {
  const response = await apiClient.get<{ value: Array<{ id: number }> }>('/v1/foods', {
    params: { page: 1, pageCount: 1 },
  });

  const firstFood = response.data.value?.[0];

  if (!firstFood?.id) {
    throw new Error('You need at least one food before creating ingredients.');
  }

  return firstFood.id;
};

export const ingredientService = {
  async getAllIngredients(): Promise<Ingredient[]> {
    const response = await apiClient.get<IngredientApiDto[]>('/v1/ingredients');
    const metadataStore = loadMetadata();
    return response.data.map((ingredient) => toIngredient(ingredient, metadataStore));
  },

  async searchIngredients(term: string): Promise<Ingredient[]> {
    const trimmedTerm = term.trim();
    if (!trimmedTerm) {
      const response = await apiClient.get<IngredientApiDto[]>('/v1/ingredients');
      const metadataStore = loadMetadata();
      return response.data.map((ingredient) => toIngredient(ingredient, metadataStore));
    }

    const response = await apiClient.get<IngredientApiDto[]>('/v1/ingredients/search', {
      params: { name: trimmedTerm },
    });
    const metadataStore = loadMetadata();
    return response.data.map((ingredient) => toIngredient(ingredient, metadataStore));
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<IngredientApiDto>(`/v1/ingredients/${id}`);
    return toIngredient(response.data, loadMetadata());
  },

  async createIngredient(values: IngredientFormValues): Promise<Ingredient> {
    const foodEntityId = values.foodEntityId && values.foodEntityId > 0
      ? values.foodEntityId
      : await getExistingFoodId();

    const response = await apiClient.post<IngredientApiDto>('/v1/ingredients', {
      name: values.name,
      foodEntityId,
    });

    saveMetadata(response.data.id, values);
    return toIngredient(response.data, loadMetadata());
  },

  async updateIngredient(id: number, values: IngredientFormValues): Promise<Ingredient> {
    const foodEntityId = values.foodEntityId && values.foodEntityId > 0
      ? values.foodEntityId
      : await getExistingFoodId();

    const response = await apiClient.put<IngredientApiDto>(`/v1/ingredients/${id}`, {
      name: values.name,
      foodEntityId,
    });

    saveMetadata(id, values);
    return toIngredient(response.data, loadMetadata());
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
    removeMetadata(id);
  },
};