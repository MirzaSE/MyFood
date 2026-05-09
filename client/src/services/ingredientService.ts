import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  async getAllIngredients(page = 1, pageCount = 10): Promise<{ items: Ingredient[]; total: number }> {
    const response = await apiClient.get(`/v1/ingredient?page=${page}&pageCount=${pageCount}`);
    const pagination = response.headers['x-pagination']
      ? JSON.parse(response.headers['x-pagination'])
      : { totalCount: 0 };
    return { items: response.data.value ?? [], total: pagination.totalCount };
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredient/${id}`);
    return response.data;
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredient', data);
    return response.data;
  },

  async updateIngredient(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/v1/ingredient/${id}`, data);
    return response.data;
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredient/${id}`);
  },
};
