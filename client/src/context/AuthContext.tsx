import React, { createContext, useContext, useEffect, useState } from 'react';
import type { AuthContextType } from '../types';
import { authService } from '../services/authService';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [username, setUsername] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Load auth state from localStorage
  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    const storedUsername = localStorage.getItem('username');

    if (storedToken && storedUsername) {
      setToken(storedToken);
      setUsername(storedUsername);
      setIsAuthenticated(true);
    }

    setIsLoading(false);
  }, []);

  // LOGIN FIXED
  const login = async (username: string, password: string) => {
    const response = await authService.login(username, password);

    setToken(response.token);
    setUsername(username);
    setIsAuthenticated(true);

    localStorage.setItem('token', response.token);
    localStorage.setItem('username', username);
  };

  // REGISTER FIXED
  const register = async (username: string, email: string, password: string) => {
    const response = await authService.register(username, email, password);

    setToken(response.token);
    setUsername(username);
    setIsAuthenticated(true);

    localStorage.setItem('token', response.token);
    localStorage.setItem('username', username);
  };

  const logout = () => {
    authService.logout();
    setToken(null);
    setUsername(null);
    setIsAuthenticated(false);
  };

  if (isLoading) {
    return null;
  }

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