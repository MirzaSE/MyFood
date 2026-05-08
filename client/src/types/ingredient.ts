export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  quantity?: number | null;
  foodEntityId?: number | null;
}

export interface IngredientCreateDto {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  quantity?: number | null;
  foodEntityId?: number | null;
}

export interface IngredientUpdateDto {
  name?: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
  quantity?: number | null;
  foodEntityId?: number | null;
}

export interface SelectedFoodIngredient {
  ingredientId: number;
  name: string;
  unit: string;
  quantity: number;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}