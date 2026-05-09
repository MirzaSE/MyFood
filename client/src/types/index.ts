import type { FoodIngredient, FoodIngredientInput, NutritionTotals } from './ingredient';

export * from './ingredient';

export interface Food {
  id: number;
  name: string;
  type: string;
  calories: number;
  created: string;
  ingredients: FoodIngredient[];
  nutritionTotals: NutritionTotals;
  links?: Array<{
    href: string;
    rel: string;
    method: string;
  }>;
}

export interface FoodCreateDto {
  name: string;
  type: string;
  calories: number;
  ingredients: FoodIngredientInput[];
}

export interface FoodUpdateDto {
  name: string;
  type: string;
  calories: number;
  ingredients: FoodIngredientInput[];
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

export interface ApiValidationProblem {
  message?: string;
  title?: string;
  errors?: Record<string, string[]>;
}

export interface AuthContextType {
  isAuthenticated: boolean;
  username: string | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
}
