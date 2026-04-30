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

  client.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  });

  client.interceptors.response.use(
    (response) => response,
    (error) => {
      const requestUrl = error.config?.url || '';

      const isAuthRequest =
        requestUrl.includes('/authenticate/login') ||
        requestUrl.includes('/authenticate/register');

      if (error.response?.status === 401 && !isAuthRequest) {
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('userId');
        localStorage.removeItem('user');

        window.location.href = '/login';
      }

      return Promise.reject(error);
    }
  );

  return client;
};

export const apiClient = createApiClient();

export default apiClient;