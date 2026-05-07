export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  foodEntityId?: number | null;
}

export type IngredientFormValues = {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  foodEntityId?: number | null;
};

/** One row in FoodIngredientsPicker: ingredient + amount in its unit */
export interface IngredientPickerLine {
  ingredient: Ingredient;
  quantity: number;
}
