export interface Ingredient {
  id: number;
  foodEntityId: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface IngredientCreateDto {
  foodEntityId: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export type IngredientUpdateDto = IngredientCreateDto;
