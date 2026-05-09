import apiClient from './api';
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types';

export const authService = {
  async login(username: string, password: string): Promise<AuthResponse> {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const response = await apiClient.post<any>('/auth/login', {
      FullName: username,
      password,
    } as LoginRequest);

    // Check backend success flag (note: Newtonsoft.Json uses camelCase)
    if (!response.data.success) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const error: any = new Error(response.data.message || 'Login failed');
      error.response = { data: response.data };
      throw error;
    }

    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('username', response.data.user?.username || username);
    }

    return response.data;
  },

  async register(username: string, email: string, password: string): Promise<AuthResponse> {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const response = await apiClient.post<any>('/auth/register', {
      FullName: username,
      email,
      password,
    } as RegisterRequest);

    // Check backend success flag
    if (!response.data.success) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const error: any = new Error(response.data.message || 'Registration failed');
      error.response = { data: response.data };
      throw error;
    }

    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('username', response.data.user?.username || username);
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