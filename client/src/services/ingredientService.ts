import apiClient from './api';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const ingredientService = {
  async getAll(): Promise<Ingredient[]> {
    const response = await apiClient.get<Ingredient[]>('/ingredients');
    return response.data;
  },

  async getById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/ingredients/${id}`);
    return response.data;
  },

  async create(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>('/ingredients', data);
    return response.data;
  },

  async update(id: number, data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(`/ingredients/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/ingredients/${id}`);
  },

  async search(name: string): Promise<Ingredient[]> {
    const response = await apiClient.get<Ingredient[]>(`/ingredients/search?name=${name}`);
    return response.data;
  },
};