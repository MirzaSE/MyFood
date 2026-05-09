import apiClient from './api';
import type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
  PaginationMetadata,
} from '../types/ingredient';

interface PaginatedResult<T> {
  items: T[];
  pagination: PaginationMetadata | null;
}

export const ingredientService = {
  async getAll(page = 1, pageCount = 10, query = ''): Promise<PaginatedResult<Ingredient>> {
    const params: Record<string, string | number> = { page, pageCount };
    if (query) params.query = query;

    const response = await apiClient.get('/v1/ingredients', { params });

    let pagination: PaginationMetadata | null = null;
    const header = response.headers['x-pagination'];
    if (header) {
      try {
        pagination = JSON.parse(header) as PaginationMetadata;
      } catch {
        pagination = null;
      }
    }

    return {
      items: response.data?.value ?? [],
      pagination,
    };
  },

  async getById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return response.data;
  },

  async search(name: string): Promise<Ingredient[]> {
    const response = await apiClient.get(`/v1/ingredients/search`, { params: { name } });
    return response.data?.value ?? [];
  },

  async create(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/v1/ingredients', data);
    return response.data;
  },

  async update(id: number, data: IngredientUpdateDto): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/v1/ingredients/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
