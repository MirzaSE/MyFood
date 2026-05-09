export interface Ingredient {
  id: number;
  name: string;
  quantity?: string;
  foodId?: number;
  created?: string;
}

export interface IngredientCreateDto {
  name: string;
  quantity?: string;
}

export interface IngredientUpdateDto {
  name?: string;
  quantity?: string;
}
