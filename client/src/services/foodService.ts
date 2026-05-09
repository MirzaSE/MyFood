import apiClient from './api';
import type { Food, FoodCreateDto, FoodUpdateDto } from '../types';

const normalizeFood = (food: any): Food => ({
  id: food.id,
  name: food.name,
  type: food.type,
  calories: food.calories,
  created: food.created,
  ingredients: (food.ingredients ?? []).map((ingredient: any) => ({
    ingredientId: ingredient.ingredientId,
    quantity: Number(ingredient.quantity),
    name: ingredient.name,
    unit: ingredient.unit,
    caloriesPerUnit: Number(ingredient.caloriesPerUnit),
    calories: Number(ingredient.calories),
    protein: Number(ingredient.protein),
    carbs: Number(ingredient.carbs),
    fat: Number(ingredient.fat),
  })),
  nutritionTotals: {
    calories: Number(food.nutritionTotals?.calories ?? food.calories ?? 0),
    protein: Number(food.nutritionTotals?.protein ?? 0),
    carbs: Number(food.nutritionTotals?.carbs ?? 0),
    fat: Number(food.nutritionTotals?.fat ?? 0),
  },
  links: food.links,
});

export const foodService = {
  async getAllFoods(): Promise<Food[]> {
    const response = await apiClient.get('/v1/foods');
    return response.data.value.map(normalizeFood);
  },

  async getFoodById(id: number): Promise<Food> {
    const response = await apiClient.get<Food>(`/v1/foods/${id}`);
    return normalizeFood(response.data);
  },

  async createFood(data: FoodCreateDto): Promise<Food> {
    const response = await apiClient.post<Food>('/v1/foods', data);
    return normalizeFood(response.data);
  },

  async updateFood(id: number, data: FoodUpdateDto): Promise<Food> {
    const response = await apiClient.put<Food>(`/v1/foods/${id}`, data);
    return normalizeFood(response.data);
  },

  async deleteFood(id: number): Promise<void> {
    await apiClient.delete(`/v1/foods/${id}`);
  },
};
