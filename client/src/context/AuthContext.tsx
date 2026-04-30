import React, { createContext, useContext, useEffect, useState } from 'react';
import type { AuthContextType } from '../types';
import { authService } from '../services/authService';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [username, setUsername] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    const storedUsername = localStorage.getItem('username');

    if (storedToken && storedUsername && authService.isAuthenticated()) {
      setToken(storedToken);
      setUsername(storedUsername);
      setIsAuthenticated(true);
    } else {
      authService.logout();
      setIsAuthenticated(false);
      setToken(null);
      setUsername(null);
    }

    setIsLoading(false);
  }, []);

  const login = async (username: string, password: string) => {
    try {
      const response = await authService.login(username, password);

      setToken(response.token!);
      setUsername(response.username || username);
      setIsAuthenticated(true);

      return response;
    } catch (error) {
      authService.logout();
      setIsAuthenticated(false);
      setToken(null);
      setUsername(null);
      throw error;
    }
  };

  const register = async (username: string, password: string) => {
    try {
      const response = await authService.register(username, password);

      setToken(response.token!);
      setUsername(response.username || username);
      setIsAuthenticated(true);

      return response;
    } catch (error) {
      authService.logout();
      setIsAuthenticated(false);
      setToken(null);
      setUsername(null);
      throw error;
    }
  };

  const logout = () => {
    authService.logout();
    setToken(null);
    setUsername(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        username,
        token,
        login,
        register,
        logout,
        isLoading,
      }}
    >
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