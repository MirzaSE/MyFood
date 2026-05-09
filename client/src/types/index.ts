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
  FullName: string;
  password: string;
}

export interface RegisterRequest {
  FullName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  user: any;
  token: string;
  FullName: string;
}

export interface AuthContextType {
  isAuthenticated: boolean;
  username: string | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
}
