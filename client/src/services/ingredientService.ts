import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types';

const normalizeIngredient = (ingredient: any): Ingredient => ({
  id: ingredient.id,
  name: ingredient.name,
  unit: ingredient.unit,
  caloriesPerUnit: Number(ingredient.caloriesPerUnit),
  protein: Number(ingredient.protein),
  carbs: Number(ingredient.carbs),
  fat: Number(ingredient.fat),
});

interface IngredientQueryOptions {
  page?: number;
  pageCount?: number;
  query?: string;
}

export const ingredientService = {
  async getAllIngredients(options: IngredientQueryOptions = {}): Promise<{ items: Ingredient[]; totalCount: number }> {
    const params = new URLSearchParams();
    params.set('page', String(options.page ?? 1));
    params.set('pageCount', String(options.pageCount ?? 10));

    if (options.query?.trim()) {
      params.set('query', options.query.trim());
    }

    const response = await apiClient.get(`/v1/ingredients?${params.toString()}`);
    const paginationHeader = response.headers['x-pagination'];
    const pagination = paginationHeader ? JSON.parse(paginationHeader) : { totalCount: response.data.value?.length ?? 0 };

    return {
      items: (response.data.value ?? []).map(normalizeIngredient),
      totalCount: pagination.totalCount ?? 0,
    };
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get(`/v1/ingredients/${id}`);
    return normalizeIngredient(response.data);
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post('/v1/ingredients', data);
    return normalizeIngredient(response.data);
  },

  async updateIngredient(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put(`/v1/ingredients/${id}`, data);
    return normalizeIngredient(response.data);
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
