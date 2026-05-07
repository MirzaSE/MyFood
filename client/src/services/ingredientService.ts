import apiClient from './api';
import type { Ingredient, IngredientFormValues } from '../types/ingredient';

export const ingredientService = {
  async getAll(): Promise<Ingredient[]> {
    const response = await apiClient.get<Ingredient[]>('/v1/ingredients');
    return response.data;
  },

  async search(query: string): Promise<Ingredient[]> {
    const response = await apiClient.get<Ingredient[]>('/v1/ingredients/search', {
      params: { query },
    });
    return response.data;
  },

  async getById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return response.data;
  },

  async create(data: IngredientFormValues): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredients', data);
    return response.data;
  },

  async update(id: number, data: IngredientFormValues): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/v1/ingredients/${id}`, {
      ...data,
      id,
    });
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
