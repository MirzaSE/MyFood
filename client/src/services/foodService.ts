import apiClient from './api';
import type { Food, FoodCreateDto, FoodUpdateDto } from '../types';

export const foodService = {
  async getAllFoods(query?: string): Promise<Food[]> {
    const params: Record<string, string> = {};
    if (query && query.trim()) {
      params['query'] = query.trim();
    }
    const response = await apiClient.get('/v1/foods', { params });
    return response.data.value;
  },

  async getFoodById(id: number): Promise<Food> {
    const response = await apiClient.get<Food>(`/v1/foods/${id}`);
    return response.data;
  },

  async createFood(data: FoodCreateDto): Promise<Food> {
    const response = await apiClient.post<Food>('/v1/foods', data);
    return response.data;
  },

  async updateFood(id: number, data: FoodUpdateDto): Promise<Food> {
    const response = await apiClient.put<Food>(`/v1/foods/${id}`, data);
    return response.data;
  },

  async deleteFood(id: number): Promise<void> {
    await apiClient.delete(`/v1/foods/${id}`);
  },
};
