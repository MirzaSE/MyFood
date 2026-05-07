export interface IngredientApi {
  id: number;
  name: string;
  quantity: number;
  protein: number;
  carbs: number;
  fat: number;
  foodEntityId: number;
  links?: unknown[];
}

export interface Ingredient {
  id: number;
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  foodEntityId: number;
}

export interface IngredientFormData {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface IngredientCreateDto extends IngredientFormData {}
export interface IngredientUpdateDto extends IngredientFormData {}

export interface IngredientListResponse {
  items: Ingredient[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export interface SelectedIngredient {
  ingredient: Ingredient;
  quantity: number;
}
