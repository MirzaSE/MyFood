export interface Ingredient {
  id: number;
  name: string;
  quantity: string;
  foodId: number | null;
}

export interface IngredientCreateDto {
  name: string;
  quantity: string;
  foodId?: number | null;
}

export interface IngredientUpdateDto {
  name: string;
  quantity: string;
  foodId?: number | null;
}
