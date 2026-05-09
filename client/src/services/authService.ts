import apiClient from './api';
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types';

export const authService = {
  async login(username: string, password: string): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/authenticate/login', {
      username,
      password,
    } as LoginRequest);

    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('username', response.data.username || username);
      return { token: response.data.token, username: response.data.username || username } as AuthResponse;
    }

    throw new Error('Login failed');
  },

  async register(username: string, email: string, password: string): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/authenticate/register', {
      username,
      email,
      password,
    } as RegisterRequest);

    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('username', response.data.username || username);
      return { token: response.data.token, username: response.data.username || username } as AuthResponse;
    }

    throw new Error('Registration failed');
  },

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
  },

  getToken(): string | null {
    return localStorage.getItem('token');
  },

  getUsername(): string | null {
    return localStorage.getItem('username');
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  },
};
