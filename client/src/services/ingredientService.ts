import api from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  getAll: async (page = 1, pageCount = 10): Promise<Ingredient[]> => {
    const response = await api.get(`/api/v1/ingredients?page=${page}&pageCount=${pageCount}`);
    return response.data;
  },

  getById: async (id: number): Promise<Ingredient> => {
    const response = await api.get(`/api/v1/ingredients/${id}`);
    return response.data;
  },

  search: async (name: string): Promise<Ingredient[]> => {
    const response = await api.get(`/api/v1/ingredients/search?name=${encodeURIComponent(name)}`);
    return response.data;
  },

  create: async (dto: IngredientCreateDto): Promise<Ingredient> => {
    const response = await api.post('/api/v1/ingredients', dto);
    return response.data;
  },

  update: async (id: number, dto: IngredientUpdateDto): Promise<Ingredient> => {
    const response = await api.put(`/api/v1/ingredients/${id}`, dto);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/api/v1/ingredients/${id}`);
  },
};
