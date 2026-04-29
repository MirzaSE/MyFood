import React, { createContext, useContext, useEffect, useState } from 'react';
import type { AuthContextType } from '../types';
import { authService } from '../services/authService';

function readStoredAuth(): { token: string | null; username: string | null; isAuthenticated: boolean } {
  const token = localStorage.getItem('token');
  const username = localStorage.getItem('username');
  return {
    token,
    username,
    isAuthenticated: Boolean(token && username),
  };
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const initial = readStoredAuth();
  const [isAuthenticated, setIsAuthenticated] = useState(initial.isAuthenticated);
  const [username, setUsername] = useState<string | null>(initial.username);
  const [token, setToken] = useState<string | null>(initial.token);

  useEffect(() => {
    const { token: t, username: u, isAuthenticated: ok } = readStoredAuth();
    setToken(t);
    setUsername(u);
    setIsAuthenticated(ok);
  }, []);

  const login = async (user: string, password: string) => {
    const response = await authService.login(user.trim(), password);
    setToken(response.token);
    setUsername(response.username);
    setIsAuthenticated(true);
  };

  const register = async (user: string, email: string, password: string) => {
    const response = await authService.register(user.trim(), email.trim(), password);
    setToken(response.token);
    setUsername(response.username);
    setIsAuthenticated(true);
  };

  const logout = () => {
    authService.logout();
    setToken(null);
    setUsername(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, username, token, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};
