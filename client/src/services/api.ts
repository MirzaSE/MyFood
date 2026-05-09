import axios from 'axios';
import type { AxiosError, AxiosInstance } from 'axios';
import type { ApiValidationProblem } from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080';

const createApiClient = (): AxiosInstance => {
  const client = axios.create({
    baseURL: `${API_BASE_URL}/api`,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Add auth token to all requests
  client.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  });

  // Handle 401 responses
  client.interceptors.response.use(
    (response) => response,
    (error) => {
      console.log("API ERROR:", error.response?.data);
      const requestUrl = error.config?.url as string | undefined;
      const isAuthRequest = requestUrl?.includes('/authenticate/login') || requestUrl?.includes('/authenticate/register');

      if (error.response?.status === 401 && !isAuthRequest) {
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );

  return client;
};

export const extractApiErrorMessage = (error: unknown): string => {
  const axiosError = error as AxiosError<ApiValidationProblem>;
  const data = axiosError.response?.data;

  if (data?.message) {
    return data.message;
  }

  if (data?.errors) {
    const validationMessages = Object.values(data.errors).flat();
    if (validationMessages.length > 0) {
      return validationMessages.join(' ');
    }
  }

  if (data?.title) {
    return data.title;
  }

  return 'Something went wrong. Please try again.';
};

export const apiClient = createApiClient();

export default apiClient;
