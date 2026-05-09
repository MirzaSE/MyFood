export const formatApiError = (err: any): string => {
  if (!err) {
    return 'An unexpected error occurred.';
  }

  // Axios error with response data
  if (err.response?.data) {
    const data = err.response.data;

    // Single message string
    if (typeof data === 'string') {
      return data;
    }

    if (typeof data === 'object') {
      // Errors array
      if (Array.isArray(data.errors) && data.errors.length > 0) {
        const cleanErrors = (data.errors as string[]).map((e: string) => e.trim().replace(/\.$/, ''));
        const detail = cleanErrors.join('\n');
        if (data.message) {
          return `${data.message.trim().replace(/\.$/, '')}\n${detail}`;
        }
        return detail;
      }

      // Errors object (field-level validation)
      if (data.errors && typeof data.errors === 'object') {
        const messages = (Object.values(data.errors) as string[][]).flat();
        if (messages.length > 0) {
          const cleanMessages = messages.map((e: string) => e.trim().replace(/\.$/, ''));
          const detail = cleanMessages.join('\n');
          if (data.message) {
            return `${data.message.trim().replace(/\.$/, '')}\n${detail}`;
          }
          return detail;
        }
      }

      // Message field only
      if (data.message) {
        return data.message;
      }
    }
  }

  // Network error
  if (err.message) {
    return err.message;
  }

  return 'An unexpected error occurred.';
};
