export interface Food {
  id: number;
  name: string;
  type: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  created: string;
}

export interface FoodIngredientSelectionDto {
  ingredientId: number;
  quantity: number;
}

export interface FoodCreateDto {
  name: string;
  type: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  ingredients: FoodIngredientSelectionDto[];
}

export interface FoodUpdateDto {
  name: string;
  type: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  ingredients: FoodIngredientSelectionDto[];
}

export type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
  SelectedIngredient,
} from './ingredient';

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
