import React, { useState } from 'react';
import { authService } from '../services/authService';
import { AuthContext } from './auth-context';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [username, setUsername] = useState<string | null>(() => localStorage.getItem('username'));
  const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'));
  const isAuthenticated = Boolean(token && username);

  const login = async (username: string, password: string) => {
    const response = await authService.login(username, password);
    setToken(response.token);
    setUsername(response.username);
  };

  const register = async (username: string, email: string, password: string) => {
    const response = await authService.register(username, email, password);
    setToken(response.token);
    setUsername(response.username);
  };

  const logout = () => {
    authService.logout();
    setToken(null);
    setUsername(null);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, username, token, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
