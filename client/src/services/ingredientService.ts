import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  async getAll(page = 1, pageCount = 10): Promise<Ingredient[]> {
    const response = await apiClient.get(`/v1/ingredients`, { params: { page, pageCount } });
    return response.data;
  },

  async getById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return response.data;
  },

  async create(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredients', data);
    return response.data;
  },

  async update(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/v1/ingredients/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },

  async search(name: string): Promise<Ingredient[]> {
    const response = await apiClient.get(`/v1/ingredients/search`, { params: { name } });
    return response.data;
  },
};
