import apiClient from './api';
import type { Food, FoodCreateDto, FoodUpdateDto } from '../types';

type FoodCollectionResponse = {
  value?: Food[];
};

const normalizeFood = (food: Food): Food => ({
  ...food,
  ingredients: food.ingredients ?? [],
});

const unwrapFood = (data: Food | { value?: Food }): Food => {
  const food = 'value' in data && data.value ? data.value : data as Food;
  return normalizeFood(food);
};

const cleanFoodPayload = <T extends FoodCreateDto | FoodUpdateDto>(data: T): T => ({
  ...data,
  name: data.name.trim(),
  type: data.type.trim(),
  ingredients: data.ingredients
    .map((ingredient) => ({
      id: ingredient.id,
      name: ingredient.name.trim(),
      quantity: ingredient.quantity,
      unit: ingredient.unit?.trim(),
      caloriesPerUnit: ingredient.caloriesPerUnit,
    }))
    .filter((ingredient) => ingredient.name.length > 0 && ingredient.quantity > 0),
});

export const foodService = {
  async getAllFoods(): Promise<Food[]> {
    const response = await apiClient.get<FoodCollectionResponse | Food[]>('/v1/foods');
    const foods = Array.isArray(response.data) ? response.data : response.data.value ?? [];
    return foods.map(normalizeFood);
  },

  async getFoodById(id: number): Promise<Food> {
    const response = await apiClient.get<Food>(`/v1/foods/${id}`);
    return unwrapFood(response.data);
  },

  async createFood(data: FoodCreateDto): Promise<Food> {
    const response = await apiClient.post<Food>('/v1/foods', cleanFoodPayload(data));
    return unwrapFood(response.data);
  },

  async updateFood(id: number, data: FoodUpdateDto): Promise<Food> {
    const response = await apiClient.put<Food>(`/v1/foods/${id}`, cleanFoodPayload(data));
    return unwrapFood(response.data);
  },

  async deleteFood(id: number): Promise<void> {
    await apiClient.delete(`/v1/foods/${id}`);
  },
};
