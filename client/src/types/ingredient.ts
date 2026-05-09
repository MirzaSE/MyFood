export interface Ingredient {
  id: number;
  name: string;
  quantity: number;
  foodEntityId: number;
}

export interface IngredientCreateDto {
  name: string;
  quantity: number;
  foodEntityId: number;
}

export interface IngredientUpdateDto {
  name: string;
  quantity: number;
  foodEntityId: number;
}

export interface IngredientWithFood extends Ingredient {
  foodName?: string;
  caloriesPerUnit?: number;
}
