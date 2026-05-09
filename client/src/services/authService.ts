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
      localStorage.setItem('username', response.data.username);
    }

    return response.data;
  },

  async register(username: string, _email: string, password: string): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>('/authenticate/register', {
      username,
      password,
    } as RegisterRequest);

    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('username', response.data.username);
    }

    return response.data;
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