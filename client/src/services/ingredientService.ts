import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export interface IngredientListResult {
  items: Ingredient[];
  totalCount: number;
  totalPages: number;
  currentPage: number;
}

const parsePaginationHeader = (raw: string | undefined) => {
  if (!raw) return { totalCount: 0, totalPages: 1, currentPage: 1 };
  try {
    const parsed = JSON.parse(raw);
    return {
      totalCount: parsed.totalCount ?? 0,
      totalPages: Math.max(1, parsed.totalPages ?? 1),
      currentPage: parsed.currentPage ?? 1,
    };
  } catch {
    return { totalCount: 0, totalPages: 1, currentPage: 1 };
  }
};

export const ingredientService = {
  async getAllIngredients(page = 1, pageCount = 10): Promise<IngredientListResult> {
    const response = await apiClient.get('/v1/ingredients', {
      params: { page, pageCount },
    });
    const meta = parsePaginationHeader(response.headers['x-pagination']);
    return {
      items: response.data.value as Ingredient[],
      ...meta,
    };
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

  async searchIngredients(name: string, page = 1, pageCount = 10): Promise<IngredientListResult> {
    const response = await apiClient.get('/v1/ingredients/search', {
      params: { name, page, pageCount },
    });
    const meta = parsePaginationHeader(response.headers['x-pagination']);
    return {
      items: response.data.value as Ingredient[],
      ...meta,
    };
  },
};
