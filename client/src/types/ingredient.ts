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
  name?: string;
  quantity?: number;
  foodEntityId?: number;
}

export interface SelectedIngredientInput {
  ingredientId: number;
  quantity: number;
}
