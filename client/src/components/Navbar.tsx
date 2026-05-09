import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat, UtensilsCrossed, Leaf } from 'lucide-react';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const linkBase =
    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm font-medium border transition-all duration-200';
  const linkInactive = 'text-gray-300 hover:text-white border-white/10 hover:border-white/30';
  const linkActive = 'text-white bg-white/10 border-white/30';

  return (
    <nav className="bg-gradient-to-r from-slate-900 via-purple-900 to-slate-900 border-b border-white/10 backdrop-blur-lg sticky top-0 z-50">
      <div className="container mx-auto px-6 py-4">
        <div className="flex justify-between items-center gap-6">
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

          {/* Nav Links */}
          <div className="hidden md:flex items-center gap-2">
            <NavLink
              to="/foods"
              className={({ isActive }) => `${linkBase} ${isActive ? linkActive : linkInactive}`}
            >
              <UtensilsCrossed size={16} /> Foods
            </NavLink>
            <NavLink
              to="/ingredients"
              className={({ isActive }) => `${linkBase} ${isActive ? linkActive : linkInactive}`}
            >
              <Leaf size={16} /> Ingredients
            </NavLink>
          </div>

          {/* User Info & Logout */}
          <div className="flex flex-col items-end space-y-2">
            <p className="text-sm text-gray-300">
              Welcome back, <span className="font-semibold text-white">{username}</span>
            </p>
            <button
              onClick={handleLogout}
              className="flex items-center space-x-2 px-4 py-1.5 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all duration-200 border border-red-500/30 hover:border-red-500/50 text-sm"
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

