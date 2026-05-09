export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface IngredientCreateDto {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export type IngredientUpdateDto = IngredientCreateDto;

export interface FoodIngredientPick {
  ingredientId: number;
  quantity: number;
}
