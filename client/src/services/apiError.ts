import axios from 'axios';

type ApiErrorResponse = {
  message?: string;
  title?: string;
  errors?: Record<string, string[] | string | undefined>;
};

export const getApiErrorMessage = (error: unknown, fallback: string): string => {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data as ApiErrorResponse | string | undefined;

    if (typeof responseData === 'string' && responseData.trim()) {
      return responseData;
    }

    if (responseData && typeof responseData === 'object') {
      if (responseData.message) {
        return responseData.message;
      }

      if (responseData.title && responseData.title !== 'One or more validation errors occurred.') {
        return responseData.title;
      }

      if (responseData.errors) {
        const validationMessages: string[] = [];

        Object.values(responseData.errors).forEach((value) => {
          if (Array.isArray(value)) {
            validationMessages.push(...value.filter((message): message is string => Boolean(message)));
            return;
          }

          if (typeof value === 'string' && value) {
            validationMessages.push(value);
          }
        });

        if (validationMessages.length > 0) {
          return validationMessages.join(' ');
        }
      }

      if (error.response?.status === 401) {
        return 'Invalid username or password';
      }

      if (error.response?.status === 409) {
        return 'User already exists';
      }
    }
  }

  return fallback;
};