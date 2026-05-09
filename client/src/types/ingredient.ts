export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  quantity?: number;
  foodEntityId?: number | null;
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

export interface FoodIngredientInput {
  id?: number;
  name: string;
  quantity: number;
  unit?: string;
  caloriesPerUnit?: number;
}
