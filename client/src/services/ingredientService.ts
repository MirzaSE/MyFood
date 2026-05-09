import apiClient from './api';
import type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
  PagedResult,
  PaginationMetadata,
} from '../types/ingredient';

const parsePagination = (header: string | null | undefined): PaginationMetadata | null => {
  if (!header) return null;
  try {
    return JSON.parse(header) as PaginationMetadata;
  } catch {
    return null;
  }
};

export const ingredientService = {
  async getAll(page = 1, pageCount = 10): Promise<PagedResult<Ingredient>> {
    const response = await apiClient.get('/v1/ingredients', {
      params: { page, pageCount },
    });
    return {
      items: response.data?.value ?? [],
      pagination: parsePagination(response.headers['x-pagination']),
    };
  },

  async getById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return response.data;
  },

  async search(name: string): Promise<Ingredient[]> {
    const response = await apiClient.get('/v1/ingredients/search', {
      params: { name },
    });
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

  async remove(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },
};
