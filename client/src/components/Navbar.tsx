import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat, UtensilsCrossed, Salad } from 'lucide-react';

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
        <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-gradient-to-br from-purple-500 to-blue-500 rounded-lg">
                <ChefHat size={24} className="text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-white">MyFood</h1>
                <p className="text-xs text-purple-300">Food Management</p>
              </div>
            </div>

            <div className="flex flex-wrap gap-3">
              <NavLink
                to="/foods"
                className={({ isActive }) =>
                  `inline-flex items-center gap-2 px-4 py-2 rounded-lg border transition-colors ${
                    isActive
                      ? 'bg-white/15 border-white/30 text-white'
                      : 'bg-white/5 border-white/10 text-gray-200 hover:bg-white/10'
                  }`
                }
              >
                <UtensilsCrossed size={18} />
                Foods
              </NavLink>
              <NavLink
                to="/ingredients"
                className={({ isActive }) =>
                  `inline-flex items-center gap-2 px-4 py-2 rounded-lg border transition-colors ${
                    isActive
                      ? 'bg-white/15 border-white/30 text-white'
                      : 'bg-white/5 border-white/10 text-gray-200 hover:bg-white/10'
                  }`
                }
              >
                <Salad size={18} />
                Ingredients
              </NavLink>
            </div>
          </div>

          <div className="flex flex-wrap items-center gap-3 lg:justify-end">
            <div>
              <p className="text-sm text-gray-300">Welcome back,</p>
              <p className="text-lg font-semibold text-white">{username}</p>
            </div>

            <button
              onClick={handleLogout}
              className="inline-flex items-center space-x-2 px-4 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all duration-200 border border-red-500/30 hover:border-red-500/50"
            >
              <LogOut size={18} />
              <span className="font-medium">Logout</span>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};

