import apiClient from './api';
import type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
} from '../types/ingredient';

export const ingredientService = {
  async getIngredients(
    pageNumber = 1,
    pageSize = 100,
    search = ''
  ): Promise<Ingredient[]> {
    const response = await apiClient.get('/v1/ingredients', {
      params: {
        pageNumber,
        pageSize,
        search: search || undefined,
      },
    });

    return response.data;
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get(`/v1/ingredients/${id}`);
    return response.data;
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post('/v1/ingredients', data);
    return response.data;
  },

  async updateIngredient(
    id: number,
    data: IngredientUpdateDto
  ): Promise<Ingredient> {
    const response = await apiClient.put(`/v1/ingredients/${id}`, data);
    return response.data;
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};