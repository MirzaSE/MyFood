import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat } from 'lucide-react';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-gradient-to-r from-slate-900 via-purple-900 to-slate-900 border-b border-white/10 backdrop-blur-lg sticky top-0 z-50">
      <div className="container mx-auto px-6 py-4">
        <div className="flex justify-between items-center">

          {/* Logo + Nav Links */}
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

            {/* Nav Links */}
            <div className="flex space-x-2">
              <button
                onClick={() => navigate('/foods')}
                className={`px-4 py-2 rounded-lg text-sm font-medium transition ${
                  location.pathname === '/foods'
                    ? 'bg-purple-600 text-white'
                    : 'text-gray-300 hover:text-white hover:bg-white/10'
                }`}
              >
                Foods
              </button>
              <button
                onClick={() => navigate('/ingredients')}
                className={`px-4 py-2 rounded-lg text-sm font-medium transition ${
                  location.pathname === '/ingredients'
                    ? 'bg-purple-600 text-white'
                    : 'text-gray-300 hover:text-white hover:bg-white/10'
                }`}
              >
                Ingredients
              </button>
            </div>
          </div>

          {/* User Info & Logout */}
          <div className="flex flex-col items-end">
            <p className="text-sm text-gray-300">Welcome back, <span className="font-semibold text-white">{username}</span></p>
            <button
              onClick={handleLogout}
              className="flex items-center space-x-1 mt-1 text-red-400 hover:text-red-300 text-sm transition-colors"
            >
              <LogOut size={14} />
              <span>Logout</span>
            </button>
          </div>

        </div>
      </div>
    </nav>
  );
};