import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat } from 'lucide-react';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-gradient-to-r from-slate-900 via-purple-900 to-slate-900 border-b border-white/10 backdrop-blur-lg sticky top-0 z-50">
      <div className="container mx-auto px-6 py-4">
        <div className="flex justify-between items-center">
          {/* Logo */}
          <div className="flex items-center space-x-3">
            <div className="p-2 bg-gradient-to-br from-purple-500 to-blue-500 rounded-lg">
              <ChefHat size={24} className="text-white" />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-white">MyFood</h1>
              <p className="text-xs text-purple-300">Food Management</p>
            </div>
          </div>

          {/* Navigation Links */}
<div className="flex items-center space-x-3 mr-6">
  <button
    onClick={() => navigate('/foods')}
    className="px-4 py-2 text-gray-300 hover:text-white hover:bg-white/10 rounded-lg transition"
  >
    Foods
  </button>
  <button
    onClick={() => navigate('/ingredients')}
    className="px-4 py-2 text-gray-300 hover:text-white hover:bg-white/10 rounded-lg transition"
  >
    Ingredients
  </button>
</div>

{/* User Info & Logout */}
<div className="flex items-center space-x-6">
  <div className="flex flex-col items-end space-y-2">
    <div>
      <p className="text-sm text-gray-300">Welcome back, <span className="text-lg font-semibold text-white">{username}</span></p>
    </div>

    <button
      onClick={handleLogout}
      className="flex items-center space-x-2 px-4 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all duration-200 border border-red-500/30 hover:border-red-500/50"
    >
      <LogOut size={18} />
      <span className="hidden sm:inline font-medium">Logout</span>
    </button>
  </div>
</div>
        </div>
      </div>
    </nav>
  );
};

