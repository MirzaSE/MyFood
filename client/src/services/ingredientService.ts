import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  async getAllIngredients(page: number = 1, pageCount: number = 10): Promise<{ value: Ingredient[], links: any }> {
    const response = await apiClient.get(`/v1/ingredients?page=${page}&pageCount=${pageCount}`);
    return response.data;
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return response.data;
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    if (!data.name || !data.quantity || data.quantity <= 0 || !data.foodEntityId) {
      throw new Error('Name, quantity (> 0), and food entity ID are required');
    }
    const response = await apiClient.post<Ingredient>('/v1/ingredients', data);
    return response.data;
  },

  async updateIngredient(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    if (!data.name || !data.quantity || data.quantity <= 0 || !data.foodEntityId) {
      throw new Error('Name, quantity (> 0), and food entity ID are required');
    }
    const response = await apiClient.put<Ingredient>(`/v1/ingredients/${id}`, data);
    return response.data;
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },

  async searchIngredients(name: string): Promise<{ value: Ingredient[] }> {
    const response = await apiClient.get(`/v1/ingredients/search?name=${encodeURIComponent(name)}`);
    return response.data;
  },
};
