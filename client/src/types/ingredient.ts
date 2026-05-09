export interface Ingredient {
  id: number;
  name: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
  foodEntityId?: number;
}

export interface IngredientCreateDto {
  name: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
  foodEntityId?: number;
}

export interface IngredientUpdateDto {
  name?: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
}

export interface IngredientFormValues {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}
