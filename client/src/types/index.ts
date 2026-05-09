export type {
  FoodIngredientInput,
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
} from './ingredient';

import type { FoodIngredientInput, Ingredient } from './ingredient';

export interface Food {
  id: number;
  name: string;
  type: string;
  calories: number;
  created: string;
  ingredients: Ingredient[];
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

export type IngredientInput = FoodIngredientInput;

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
  expiration: string;
}

export interface AuthContextType {
  isAuthenticated: boolean;
  username: string | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
}
