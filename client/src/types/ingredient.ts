export interface Ingredient {
  id: number;
  name: string;
  unit: string | null;
  caloriesPerUnit: number;
  protein: number | null;
  carbs: number | null;
  fat: number | null;
  foodEntityId: number | null;
}

export interface IngredientCreateDto {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein?: number | null;
  carbs?: number | null;
  fat?: number | null;
  foodEntityId?: number | null;
}

export interface IngredientUpdateDto {
  name?: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number | null;
  carbs?: number | null;
  fat?: number | null;
  foodEntityId?: number | null;
}

export interface PaginationMetadata {
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export interface PagedResult<T> {
  items: T[];
  pagination: PaginationMetadata | null;
}

/** A picked ingredient with quantity, used in the FoodIngredientsPicker. */
export interface PickedIngredient {
  ingredient: Ingredient;
  quantity: number;
}
