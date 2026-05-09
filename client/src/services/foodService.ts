import apiClient from './api';
import type { Food, FoodCreateDto, FoodUpdateDto } from '../types';

const unwrapFoodList = (data: any): Food[] => {
  if (Array.isArray(data)) return data;
  if (Array.isArray(data?.value)) return data.value;
  if (Array.isArray(data?.items)) return data.items;

  return [];
};

const unwrapFood = (data: any): Food => {
  return data?.value ?? data;
};

export const foodService = {
  async getAllFoods(): Promise<Food[]> {
    const response = await apiClient.get('/v1/foods');
    return unwrapFoodList(response.data);
  },

  async getFoodById(id: number): Promise<Food> {
    const response = await apiClient.get(`/v1/foods/${id}`);
    return unwrapFood(response.data);
  },

  async createFood(data: FoodCreateDto): Promise<Food> {
    const response = await apiClient.post('/v1/foods', data);
    return unwrapFood(response.data);
  },

  async updateFood(id: number, data: FoodUpdateDto): Promise<Food> {
    const response = await apiClient.put(`/v1/foods/${id}`, data);
    return unwrapFood(response.data);
  },

  async deleteFood(id: number): Promise<void> {
    await apiClient.delete(`/v1/foods/${id}`);
  },
};
