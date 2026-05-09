import axios from 'axios';
import type { AxiosInstance } from 'axios';

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
      const requestUrl = error.config?.url ?? '';
      const isAuthRequest = requestUrl.includes('/authenticate/');

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

export const apiClient = createApiClient();

export const getApiErrorMessage = (error: unknown, fallback: string): string => {
  if (!axios.isAxiosError(error)) {
    return fallback;
  }

  const data = error.response?.data;

  if (typeof data?.message === 'string') {
    return data.message;
  }

  if (data?.errors && typeof data.errors === 'object') {
    const validationMessages = Object.values(data.errors)
      .flat()
      .filter((message): message is string => typeof message === 'string');

    if (validationMessages.length > 0) {
      return validationMessages.join(' ');
    }
  }

  return fallback;
};

export default apiClient;
