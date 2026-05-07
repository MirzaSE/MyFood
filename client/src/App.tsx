import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { LoginPage } from './pages/LoginPage';
import { FoodPage } from './pages/FoodPage';
import { IngredientsPage } from './pages/IngredientsPage';
import { CreateFoodPage } from './pages/CreateFoodPage';
import './App.css';

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

const AppRoutes: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      <Route path="/" element={isAuthenticated ? <Navigate to="/foods" replace /> : <LoginPage />} />
      <Route
        path="/login"
        element={isAuthenticated ? <Navigate to="/foods" replace /> : <LoginPage />}
      />
      <Route
        path="/foods"
        element={
          <ProtectedRoute>
            <FoodPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/foods/create"
        element={
          <ProtectedRoute>
            <CreateFoodPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/ingredients"
        element={
          <ProtectedRoute>
            <IngredientsPage />
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<Navigate to={isAuthenticated ? '/foods' : '/'} replace />} />
    </Routes>
  );
};

function App() {
  return (
    <Router>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </Router>
  );
}

export default App;
