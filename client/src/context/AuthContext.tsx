import React, { useEffect, useState } from 'react';
import { authService } from '../services/authService';
import { AuthContext } from './AuthContextValue';

const getStoredAuthState = () => {
  const storedToken = authService.getToken();
  const storedUsername = authService.getUsername();

  return {
    token: storedToken,
    username: storedUsername,
    isAuthenticated: Boolean(storedToken && storedUsername),
  };
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [authState, setAuthState] = useState(getStoredAuthState);

  const login = async (username: string, password: string) => {
    const response = await authService.login(username, password);
    setAuthState({
      token: response.token,
      username: response.username,
      isAuthenticated: true,
    });
  };

  const register = async (username: string, email: string, password: string) => {
    const response = await authService.register(username, email, password);
    setAuthState({
      token: response.token,
      username: response.username,
      isAuthenticated: true,
    });
  };

  const logout = () => {
    authService.logout();
    setAuthState({
      token: null,
      username: null,
      isAuthenticated: false,
    });
  };

  useEffect(() => {
    const syncAuthState = () => setAuthState(getStoredAuthState());

    window.addEventListener('storage', syncAuthState);
    return () => window.removeEventListener('storage', syncAuthState);
  }, []);

  return (
    <AuthContext.Provider value={{ ...authState, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
