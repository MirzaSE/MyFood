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

export interface IngredientUpdateDto extends IngredientCreateDto {}

export interface FoodIngredientInput {
  ingredientId: number;
  quantity: number;
}

export interface FoodIngredient extends FoodIngredientInput {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface NutritionTotals {
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
}
