import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  async getAllIngredients(page = 1, pageCount = 10, query = ''): Promise<Ingredient[]> {
    const response = await apiClient.get('/v1/ingredients', {
      params: { page, pageCount, query },
    });
    return response.data.value ?? [];
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get(`/v1/ingredients/${id}`);
    return response.data;
  },

  async searchIngredients(name: string): Promise<Ingredient[]> {
    const response = await apiClient.get('/v1/ingredients/search', { params: { name } });
    return response.data.value ?? [];
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post('/v1/ingredients', data);
    return response.data;
  },

  async updateIngredient(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put(`/v1/ingredients/${id}`, data);
    return response.data;
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
