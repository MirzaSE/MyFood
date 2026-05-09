export interface Ingredient {
  id: number;
  name: string;
  unit?: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  quantity: number;
  foodId?: number;
}

export interface IngredientCreateDto {
  name: string;
  unit?: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  quantity: number;
  foodId?: number;
}

export interface IngredientUpdateDto {
  name?: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
  quantity?: number;
  foodId?: number;
}

export interface SelectedIngredient {
  ingredient: Ingredient;
  quantity: number;
}
