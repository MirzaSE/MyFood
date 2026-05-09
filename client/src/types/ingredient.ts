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

export interface IngredientCreateDto {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  foodEntityId?: number | null;
}

export interface IngredientUpdateDto {
  name?: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
  foodEntityId?: number | null;
}

// Used inside the food picker — pairs an ingredient with an amount.
export interface FoodIngredientLine {
  ingredient: Ingredient;
  quantity: number;
}

export interface IngredientListResponse {
  value: Ingredient[];
}

export interface PaginationMetadata {
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}
