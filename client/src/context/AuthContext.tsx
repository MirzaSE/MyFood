import React, { createContext, useContext, useEffect, useState } from 'react';
import type { AuthContextType } from '../types';
import { authService } from '../services/authService';

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [token, setToken] = useState<string | null>(null);
  const [username, setUsername] = useState<string | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isBootstrapping, setIsBootstrapping] = useState(true);

  useEffect(() => {
    const bootstrapAuth = async () => {
      const storedToken = localStorage.getItem('token');
      const storedUsername = localStorage.getItem('username');

      if (!storedToken) {
        setIsBootstrapping(false);
        return;
      }

      try {
        const currentUser = await authService.getCurrentUser();
        setToken(storedToken);
        setUsername(currentUser.username || storedUsername);
        setIsAuthenticated(true);
      } catch {
        authService.logout();
        setToken(null);
        setUsername(null);
        setIsAuthenticated(false);
      } finally {
        setIsBootstrapping(false);
      }
    };

    void bootstrapAuth();
  }, []);

  const login = async (username: string, password: string) => {
    const response = await authService.login(username, password);
    setToken(response.token);
    setUsername(response.username);
    setIsAuthenticated(true);
  };

  const register = async (username: string, email: string, password: string) => {
    const response = await authService.register(username, email, password);
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
    <AuthContext.Provider value={{ isAuthenticated, username, token, isBootstrapping, login, register, logout }}>
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
