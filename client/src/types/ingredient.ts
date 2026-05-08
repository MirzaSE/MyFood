export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface IngredientCreateDto {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface IngredientUpdateDto extends IngredientCreateDto {}

export interface FoodIngredientCreateDto {
  ingredientId: number;
  quantity: number;
}

export interface PaginationMeta {
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export interface PagedIngredientsResult {
  items: Ingredient[];
  pagination: PaginationMeta;
}
