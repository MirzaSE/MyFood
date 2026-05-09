export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  quantity: number;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface IngredientCreateDto {
  name: string;
  unit: string;
  quantity: number;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export type IngredientUpdateDto = IngredientCreateDto;

export interface SelectedIngredient {
  ingredient: Ingredient;
  quantity: number;
}
