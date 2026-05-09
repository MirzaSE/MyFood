import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export interface PaginatedIngredients {
  items: Ingredient[];
  totalCount: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
}

export const ingredientService = {
  async getAll(page = 1, pageCount = 10): Promise<PaginatedIngredients> {
    const response = await apiClient.get('/v1/ingredient', { params: { page, pageCount } });
    const pagination = response.headers['x-pagination']
      ? JSON.parse(response.headers['x-pagination'])
      : { totalCount: 0, totalPages: 1, currentPage: page, pageSize: pageCount };
    return {
      items: response.data,
      totalCount: pagination.totalCount,
      totalPages: pagination.totalPages,
      currentPage: pagination.currentPage,
      pageSize: pagination.pageSize,
    };
  },

  async getById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredient/${id}`);
    return response.data;
  },

  async create(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredient', data);
    return response.data;
  },

  async update(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/v1/ingredient/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredient/${id}`);
  },

  async search(query: string, page = 1, pageCount = 10): Promise<PaginatedIngredients> {
    const response = await apiClient.get('/v1/ingredient', { params: { query, page, pageCount } });
    const pagination = response.headers['x-pagination']
      ? JSON.parse(response.headers['x-pagination'])
      : { totalCount: 0, totalPages: 1, currentPage: page, pageSize: pageCount };
    return {
      items: response.data,
      totalCount: pagination.totalCount,
      totalPages: pagination.totalPages,
      currentPage: pagination.currentPage,
      pageSize: pagination.pageSize,
    };
  },
};