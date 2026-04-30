import apiClient from './api';
import type { LoginRequest, RegisterRequest, AuthResponse } from '../types';
import { AxiosError } from 'axios';

export const authService = {
  async login(username: string, password: string): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/authenticate/login', {
        username,
        password,
      } as LoginRequest);

      if (!response.data?.token) {
        throw new Error('Invalid response from server');
      }

      localStorage.setItem('token', response.data.token);
      localStorage.setItem('username', response.data.username || username);

      return {
        ...response.data,
        username: response.data.username || username,
      };
    } catch (error) {
      if (error instanceof AxiosError) {
        if (error.response?.data?.message) {
          throw new Error(error.response.data.message);
        }

        if (error.response?.status === 401) {
          throw new Error('Invalid username or password');
        }
      }

      throw new Error(error instanceof Error ? error.message : 'Login failed');
    }
  },

  async register(username: string, password: string): Promise<AuthResponse> {
  try {
    const response = await apiClient.post('/authenticate/register', {
      username,
      password,
    } as RegisterRequest);

    if (!response.data) {
      throw new Error('Invalid response from server');
    }

    const loginResponse = await this.login(username, password);

    return loginResponse;
  } catch (error) {
    if (error instanceof AxiosError) {
      if (error.response?.data?.errors) {
        throw new Error(error.response.data.errors.join(' '));
      }

      if (error.response?.data?.message) {
        throw new Error(error.response.data.message);
      }
    }

    throw new Error(error instanceof Error ? error.message : 'Registration failed');
  }
},

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('userId');
    localStorage.removeItem('user');
  },

  getToken(): string | null {
    return localStorage.getItem('token');
  },

  getUsername(): string | null {
    return localStorage.getItem('username');
  },

  getUserId(): string | null {
    return localStorage.getItem('userId');
  },

  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');

    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      if (payload.exp && Date.now() >= payload.exp * 1000) {
        this.logout();
        return false;
      }

      return true;
    } catch {
      this.logout();
      return false;
    }
  },
};
