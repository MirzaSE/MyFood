import apiClient from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

const API_BASE = '/v1/ingredients';

export const ingredientService = {
  async getAll(page = 1, pageCount = 20) {
    const res = await apiClient.get(`${API_BASE}?page=${page}&pageCount=${pageCount}`);
    return res.data?.value ?? [];
  },
  async getById(id: number) {
    const res = await apiClient.get(`${API_BASE}/${id}`);
    return res.data;
  },
  async create(dto: IngredientCreateDto) {
    const res = await apiClient.post(API_BASE, dto);
    return res.data;
  },
  async update(id: number, dto: IngredientUpdateDto) {
    const res = await apiClient.put(`${API_BASE}/${id}`, dto);
    return res.data;
  },
  async remove(id: number) {
    await apiClient.delete(`${API_BASE}/${id}`);
  }
};
