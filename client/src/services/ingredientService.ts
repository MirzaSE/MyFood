import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  async getAllIngredients(): Promise<Ingredient[]> {
    const response = await apiClient.get<Ingredient[]>('/v1/ingredients');
    return response.data;
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return response.data;
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredients', data);
    return response.data;
  },

  async updateIngredient(id: number, data: IngredientUpdateDto): Promise<void> {
    await apiClient.put(`/v1/ingredients/${id}`, data);
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },

  async searchIngredients(query: string): Promise<Ingredient[]> {
    const all = await ingredientService.getAllIngredients();
    return all.filter(i => i.name.toLowerCase().includes(query.toLowerCase()));
  },
};
