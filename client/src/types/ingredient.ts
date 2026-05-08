export interface IngredientApiDto {
  id: number;
  name?: string;
  foodEntityId: number;
}

export interface IngredientMetadata {
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface Ingredient extends IngredientMetadata {
  id: number;
  name: string;
  foodEntityId: number;
}

export interface IngredientFormValues extends IngredientMetadata {
  name: string;
  foodEntityId?: number;
}

export interface IngredientSelection {
  ingredientId: number;
  quantity: number;
}

export interface NutritionTotals {
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
}
