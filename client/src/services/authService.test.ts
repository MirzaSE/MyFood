import { authService } from './authService';

// Mock apiClient
jest.mock('./api', () => ({
  __esModule: true,
  default: {
    post: jest.fn((url: string, data: any) => {
      if (url.includes('login')) {
        return Promise.resolve({
          data: {
            success: true,
            token: 'abc',
            user: { username: data.FullName },
          },
        });
      }

      if (url.includes('register')) {
        return Promise.resolve({
          data: {
            success: true,
            token: 'xyz',
            user: { username: data.FullName },
          },
        });
      }

      return Promise.resolve({ data: { success: true } });
    }),
  },
}));

describe('authService', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it('logs in and stores token', async () => {
    const result = await authService.login('user', 'pass');
    expect(result.token).toBe('abc');
    expect(localStorage.getItem('token')).toBe('abc');
    expect(localStorage.getItem('username')).toBe('user');
  });

  it('registers and stores token', async () => {
    const result = await authService.register('newuser', 'mail', 'pass');
    expect(result.token).toBe('xyz');
    expect(localStorage.getItem('token')).toBe('xyz');
    expect(localStorage.getItem('username')).toBe('newuser');
  });

  it('logs out and clears storage', () => {
    localStorage.setItem('token', 'abc');
    localStorage.setItem('username', 'user');
    authService.logout();
    expect(localStorage.getItem('token')).toBeNull();
    expect(localStorage.getItem('username')).toBeNull();
  });

  it('checks authentication', () => {
    expect(authService.isAuthenticated()).toBe(false);
    localStorage.setItem('token', 'abc');
    expect(authService.isAuthenticated()).toBe(true);
  });
});
