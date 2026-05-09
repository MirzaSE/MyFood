export interface Food {
  id: number;
  name: string;
  type: string;
  calories: number;
  created: string;
}

export interface FoodCreateDto {
  name: string;
  type: string;
  calories: number;
}

export interface FoodUpdateDto {
  name: string;
  type: string;
  calories: number;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
}

export interface AuthContextType {
  isAuthenticated: boolean;
  username: string | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  register: (
    username: string,
    email: string,
    password: string,
  ) => Promise<void>;
  logout: () => void;
}

export interface Ingredient {
  id: number;
  name: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
  foodId?: number;
  quantity?: number;
}

export interface IngredientCreateDto {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  foodId: number;
}

export interface IngredientUpdateDto {
  name?: string;
  unit?: string;
  caloriesPerUnit?: number;
  protein?: number;
  carbs?: number;
  fat?: number;
  foodId?: number;
}

export interface PaginatedResponse<T> {
  value: T[];
  pageSize: number;
  pageNumber: number;
  totalCount: number;
}

export interface SearchParams {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
}
