import { authService } from './authService';
import type { LoginRequest, RegisterRequest } from '../types';

type AuthRequest = LoginRequest | RegisterRequest;

// Mock apiClient
jest.mock('./api', () => ({
  default: {
    post: jest.fn((url: string, data: AuthRequest) => {
      if (url.includes('login')) {
        return Promise.resolve({ data: { token: 'abc', username: data.username } });
      }
      if (url.includes('register')) {
        return Promise.resolve({ data: { token: 'xyz', username: data.username } });
      }
      return Promise.resolve({ data: {} });
    })
  }
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
