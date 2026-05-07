import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

type IngredientCollectionResponse = Ingredient[] | { value?: Ingredient[] };

const normalizeIngredient = (ingredient: Ingredient): Ingredient => ({
  ...ingredient,
  caloriesPerUnit: Number(ingredient.caloriesPerUnit ?? 0),
  protein: Number(ingredient.protein ?? 0),
  carbs: Number(ingredient.carbs ?? 0),
  fat: Number(ingredient.fat ?? 0),
});

const cleanIngredientPayload = <T extends IngredientCreateDto | IngredientUpdateDto>(data: T): T => ({
  ...data,
  name: data.name.trim(),
  unit: data.unit.trim(),
  caloriesPerUnit: Number(data.caloriesPerUnit),
  protein: Number(data.protein),
  carbs: Number(data.carbs),
  fat: Number(data.fat),
});

export const ingredientService = {
  async getAllIngredients(): Promise<Ingredient[]> {
    const response = await apiClient.get<IngredientCollectionResponse>('/v1/ingredients');
    const ingredients = Array.isArray(response.data) ? response.data : response.data.value ?? [];
    return ingredients.map(normalizeIngredient);
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return normalizeIngredient(response.data);
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredients', cleanIngredientPayload(data));
    return normalizeIngredient(response.data);
  },

  async updateIngredient(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/v1/ingredients/${id}`, cleanIngredientPayload(data));
    return normalizeIngredient(response.data);
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
