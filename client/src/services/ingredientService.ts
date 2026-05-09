import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  getAll: async (page = 1, pageCount = 10): Promise<Ingredient[]> => {
    const response = await apiClient.get(`/v1/ingredients?page=${page}&pageCount=${pageCount}`);
    return response.data;
  },

  getById: async (id: number): Promise<Ingredient> => {
    const response = await apiClient.get(`/v1/ingredients/${id}`);
    return response.data;
  },

  create: async (dto: IngredientCreateDto): Promise<Ingredient> => {
    const response = await apiClient.post('/v1/ingredients', dto);
    return response.data;
  },

  update: async (id: number, dto: IngredientUpdateDto): Promise<Ingredient> => {
    const response = await apiClient.put(`/v1/ingredients/${id}`, dto);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },

  search: async (name: string): Promise<Ingredient[]> => {
    const response = await apiClient.get(`/v1/ingredients/search?name=${name}`);
    return response.data;
  },
};