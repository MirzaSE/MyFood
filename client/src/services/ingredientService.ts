import api from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const ingredientService = {
  getAll: async (page = 1, pageCount = 10): Promise<{ data: Ingredient[]; totalPages: number }> => {
    const response = await api.get(`/v1/ingredients?page=${page}&pageCount=${pageCount}`);
    const pagination = response.headers['x-pagination'];
    let totalPages = 1;
    if (pagination) {
      const p = JSON.parse(pagination);
      totalPages = Math.ceil(p.totalCount / pageCount) || 1;
    }
    return { data: response.data, totalPages };
  },

  getById: async (id: number): Promise<Ingredient> => {
    const response = await api.get(`/v1/ingredients/${id}`);
    return response.data;
  },

  search: async (name: string): Promise<Ingredient[]> => {
    const response = await api.get(`/v1/ingredients/search?name=${encodeURIComponent(name)}`);
    return response.data;
  },

  create: async (dto: IngredientCreateDto): Promise<Ingredient> => {
    const response = await api.post('/v1/ingredients', dto);
    return response.data;
  },

  update: async (id: number, dto: IngredientUpdateDto): Promise<Ingredient> => {
    const response = await api.put(`/v1/ingredients/${id}`, dto);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await api.delete(`/v1/ingredients/${id}`);
  },
};
