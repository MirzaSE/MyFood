import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  async getAll(page = 1, pageCount = 20, query = ''): Promise<Ingredient[]> {
    const response = await apiClient.get('/v1/ingredients', { params: { page, pageCount, query } });
    return response.data;
  },
  async create(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post('/v1/foods/' + data.foodEntityId + '/ingredients', data);
    return response.data;
  },
  async update(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put(`/v1/foods/${data.foodEntityId}/ingredients/${id}`, data);
    return response.data;
  },
  async delete(id: number, foodEntityId: number): Promise<void> {
    await apiClient.delete(`/v1/foods/${foodEntityId}/ingredients/${id}`);
  },
};
