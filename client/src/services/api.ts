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
      const isAuthRequest = requestUrl.includes('/authenticate/login') || requestUrl.includes('/authenticate/register');

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

export const getApiErrorMessage = (error: unknown, fallbackMessage: string): string => {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data;

    if (typeof responseData?.message === 'string' && responseData.message.length > 0) {
      return responseData.message;
    }

    if (responseData?.errors && typeof responseData.errors === 'object') {
      const validationMessages = Object.values(responseData.errors)
        .flatMap((value) => Array.isArray(value) ? value : [])
        .filter((value): value is string => typeof value === 'string' && value.length > 0);

      if (validationMessages.length > 0) {
        return validationMessages[0];
      }
    }
  }

  return fallbackMessage;
};

export default apiClient;
