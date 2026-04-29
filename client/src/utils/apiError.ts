import { isAxiosError } from 'axios';

export function getApiErrorMessage(error: unknown, fallback = 'Something went wrong.'): string {
  if (!isAxiosError(error)) {
    return error instanceof Error ? error.message : fallback;
  }

  if (!error.response) {
    return error.message || fallback;
  }

  const { data, status } = error.response;

  if (typeof data === 'string' && data.trim()) {
    return data;
  }

  if (data && typeof data === 'object') {
    const d = data as Record<string, unknown>;

    if (typeof d.message === 'string' && d.message.trim()) {
      return d.message;
    }

    if (typeof d.title === 'string' && d.title.trim()) {
      return d.title;
    }

    if (typeof d.detail === 'string' && d.detail.trim()) {
      return d.detail;
    }

    if (d.errors && typeof d.errors === 'object') {
      const errObj = d.errors as Record<string, string[] | string>;
      const parts = Object.values(errObj).flatMap((v) => (Array.isArray(v) ? v : [String(v)]));
      if (parts.length) {
        return parts.join(' ');
      }
    }
  }

  if (status === 401) {
    return 'Invalid username or password.';
  }

  if (status === 409) {
    return 'A user with this username already exists.';
  }

  return fallback;
}
