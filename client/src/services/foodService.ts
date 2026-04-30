import apiClient from './api';
import type { Food, FoodCreateDto, FoodUpdateDto } from '../types';

export const foodService = {
  async getAllFoods(): Promise<Food[]> {
    const response = await apiClient.get('/v1/foods');
    return response.data.value;
  },

  async getFoodById(id: number): Promise<Food> {
    const response = await apiClient.get<Food>(`/v1/foods/${id}`);
    return response.data;
  },

  async createFood(data: FoodCreateDto): Promise<Food> {
    if (!data.name || !data.type || data.calories === undefined) {
      throw new Error('Name, type, and calories are required to create a food item');
    }
    const response = await apiClient.post<Food>('/v1/foods', data);
    return response.data;
  },

  async updateFood(id: number, data: FoodUpdateDto): Promise<Food> {
    if (!data.name || !data.type || data.calories === undefined) {
      throw new Error('Name, type, and calories are required to update a food item');
    }
    const response = await apiClient.put<Food>(`/v1/foods/${id}`, data);
    return response.data;
  },

  async deleteFood(id: number): Promise<void> {
    await apiClient.delete(`/v1/foods/${id}`);
  },
};
