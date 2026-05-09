export interface Food {
  id: number;
  name: string;
  type: string;
  calories: number;
  created: string;
  ingredients: FoodIngredientDto[];
}

export interface FoodCreateDto {
  name: string;
  type: string;
  calories: number;
  ingredients?: FoodIngredientCreateDto[];
}

export interface FoodUpdateDto {
  name: string;
  type: string;
  calories: number;
}

export interface FoodIngredientDto {
  ingredientId: number;
  ingredientName: string;
  unit: string;
  quantity: number;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
}

export interface FoodIngredientCreateDto {
  ingredientId: number;
  quantity: number;
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
  isLoading: boolean;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
}
