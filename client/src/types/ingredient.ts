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

export interface IngredientUpdateDto {
  name?: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
}

// Used in FoodIngredientsPicker
export interface SelectedIngredient {
  ingredientId: number;
  quantity: number;
}

// FoodIngredient as returned from API (within FoodDto)
export interface FoodIngredientDto {
  ingredientId: number;
  ingredientName: string;
  unit: string;
  quantity: number;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}
