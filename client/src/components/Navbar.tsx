import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat, User, List, Utensils } from 'lucide-react';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { username, logout, isAuthenticated } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (!isAuthenticated) {
    return null;
  }

  return (
    <nav className="bg-gradient-to-r from-slate-900 via-purple-900 to-slate-900 border-b border-white/10 backdrop-blur-lg sticky top-0 z-50">
      <div className="container mx-auto px-6 py-4">
        <div className="flex justify-between items-center">
          {/* Logo - Left side */}
          <div className="flex items-center space-x-3">
            <div className="p-2 bg-gradient-to-br from-purple-500 to-blue-500 rounded-lg">
              <ChefHat size={24} className="text-white" />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-white">MyFood</h1>
              <p className="text-xs text-purple-300">Food Management</p>
            </div>
          </div>

          {/* Navigation Links - Center */}
          <div className="flex items-center space-x-8">
            <Link
              to="/foods"
              className="flex items-center space-x-2 text-gray-300 hover:text-white transition-colors duration-200 px-3 py-2 rounded-lg hover:bg-white/10"
            >
              <Utensils size={18} />
              <span className="font-medium">Foods</span>
            </Link>
            <Link
              to="/ingredients"
              className="flex items-center space-x-2 text-gray-300 hover:text-white transition-colors duration-200 px-3 py-2 rounded-lg hover:bg-white/10"
            >
              <List size={18} />
              <span className="font-medium">Ingredients</span>
            </Link>
          </div>

          {/* User Section */}
          <div className="flex items-center space-x-6">
            <div className="flex items-center space-x-2">
              <User size={16} className="text-purple-400" />
              <span className="text-gray-300 text-sm">
                Welcome back, <span className="font-semibold text-transparent bg-clip-text bg-gradient-to-r from-purple-400 to-blue-400">{username}</span>
              </span>
            </div>
            
            <button
              onClick={handleLogout}
              className="flex items-center space-x-2 px-4 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all duration-200 border border-red-500/30 hover:border-red-500/50"
            >
              <LogOut size={16} />
              <span className="font-medium">Logout</span>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};