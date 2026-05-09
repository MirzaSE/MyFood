import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat, Utensils, Package } from 'lucide-react';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const isActive = (path: string) => location.pathname === path;

  return (
    <nav className="bg-gradient-to-r from-slate-900 via-purple-900 to-slate-900 border-b border-white/10 backdrop-blur-lg sticky top-0 z-50">
      <div className="container mx-auto px-6 py-4">
        <div className="flex justify-between items-center">
          <div className="flex items-center space-x-6">
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-gradient-to-br from-purple-500 to-blue-500 rounded-lg">
                <ChefHat size={24} className="text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-white">MyFood</h1>
                <p className="text-xs text-purple-300">Food Management</p>
              </div>
            </div>

            <div className="flex space-x-1">
              <button
                onClick={() => navigate('/foods')}
                className={`flex items-center space-x-2 px-4 py-2 rounded-lg transition-colors ${
                  isActive('/foods') ? 'bg-purple-600 text-white' : 'text-gray-300 hover:bg-white/10'
                }`}
              >
                <Utensils size={18} />
                <span>Foods</span>
              </button>
              <button
                onClick={() => navigate('/ingredients')}
                className={`flex items-center space-x-2 px-4 py-2 rounded-lg transition-colors ${
                  isActive('/ingredients') ? 'bg-purple-600 text-white' : 'text-gray-300 hover:bg-white/10'
                }`}
              >
                <Package size={18} />
                <span>Ingredients</span>
              </button>
              <button
                onClick={() => navigate('/foods/create')}
                className={`flex items-center space-x-2 px-4 py-2 rounded-lg transition-colors ${
                  isActive('/foods/create') ? 'bg-purple-600 text-white' : 'text-gray-300 hover:bg-white/10'
                }`}
              >
                <span>+ Create Food</span>
              </button>
            </div>
          </div>

          <div className="flex flex-col items-end">
            <p className="text-sm text-gray-300">Welcome back, <span className="font-semibold text-white">{username}</span></p>
          <button
            onClick={handleLogout}
            className="mt-1 flex items-center space-x-1 text-xs text-red-400 hover:text-red-300 transition-colors">
          <LogOut size={14} />
              <span>Logout</span>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

