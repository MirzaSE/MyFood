import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types';

interface PaginationParams {
  page?: number;
  pageCount?: number;
  query?: string;
}

export const ingredientService = {
  async getAllIngredients(params: PaginationParams = {}): Promise<Ingredient[]> {
    const { page = 1, pageCount = 10, query = '' } = params;
    const response = await apiClient.get('/v1/ingredients', {
      params: { page, pageCount, query },
    });
    return response.data;
  },

  async searchIngredients(name: string, params: PaginationParams = {}): Promise<Ingredient[]> {
    const { page = 1, pageCount = 10 } = params;
    const response = await apiClient.get('/v1/ingredients/search', {
      params: { name, page, pageCount },
    });
    return response.data;
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredients', data);
    return response.data;
  },

  async updateIngredient(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/v1/ingredients/${id}`, data);
    return response.data;
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
