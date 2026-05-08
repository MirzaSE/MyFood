import apiClient from './api';
import type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
  PagedIngredientsResult,
  PaginationMeta,
} from '../types';

interface PaginationParams {
  page?: number;
  pageCount?: number;
  query?: string;
}

const parsePagination = (header: string | null): PaginationMeta => {
  if (!header) {
    return { totalCount: 0, pageSize: 0, currentPage: 1, totalPages: 0 };
  }

  try {
    return JSON.parse(header) as PaginationMeta;
  } catch {
    return { totalCount: 0, pageSize: 0, currentPage: 1, totalPages: 0 };
  }
};

export const ingredientService = {
  async getAllIngredients(params: PaginationParams = {}): Promise<PagedIngredientsResult> {
    const { page = 1, pageCount = 10, query = '' } = params;
    const response = await apiClient.get('/v1/ingredients', {
      params: { page, pageCount, query },
    });
    return {
      items: response.data,
      pagination: parsePagination(response.headers['x-pagination'] ?? null),
    };
  },

  async searchIngredients(name: string, params: PaginationParams = {}): Promise<PagedIngredientsResult> {
    const { page = 1, pageCount = 10 } = params;
    const response = await apiClient.get('/v1/ingredients/search', {
      params: { name, page, pageCount },
    });
    return {
      items: response.data,
      pagination: parsePagination(response.headers['x-pagination'] ?? null),
    };
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
