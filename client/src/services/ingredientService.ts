import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types';

type IngredientsEnvelope = {
  value: Ingredient[];
};

export const ingredientService = {
  async getAllIngredients(query = '', page = 1, pageCount = 10): Promise<Ingredient[]> {
    const response = await apiClient.get<IngredientsEnvelope>('/v1/ingredients', {
      params: { query, page, pageCount },
    });
    return response.data.value ?? [];
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
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
