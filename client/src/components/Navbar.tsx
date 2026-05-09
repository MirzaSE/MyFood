import React from 'react';
import { useNavigate, NavLink } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat, UtensilsCrossed, Leaf } from 'lucide-react';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const linkClass = ({ isActive }: { isActive: boolean }) =>
    `flex items-center gap-1.5 px-3 py-2 rounded-lg text-sm font-medium transition-all border ${
      isActive
        ? 'bg-purple-500/30 text-white border-purple-400/50'
        : 'text-gray-300 hover:text-white hover:bg-white/5 border-transparent'
    }`;

  return (
    <nav className="bg-gradient-to-r from-slate-900 via-purple-900 to-slate-900 border-b border-white/10 backdrop-blur-lg sticky top-0 z-50">
      <div className="container mx-auto px-6 py-4">
        <div className="flex justify-between items-center">
          {/* Logo + nav links */}
          <div className="flex items-center gap-6">
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-gradient-to-br from-purple-500 to-blue-500 rounded-lg">
                <ChefHat size={24} className="text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-white">MyFood</h1>
                <p className="text-xs text-purple-300">Food Management</p>
              </div>
            </div>

            <div className="hidden sm:flex items-center gap-1 ml-2">
              <NavLink to="/foods" className={linkClass}>
                <UtensilsCrossed size={16} /> Foods
              </NavLink>
              <NavLink to="/ingredients" className={linkClass}>
                <Leaf size={16} /> Ingredients
              </NavLink>
            </div>
          </div>

          {/* User Info & Logout */}
          <div className="flex items-center space-x-6">
            <div className="text-right">
              <p className="text-sm font-semibold text-white">Welcome back, {username}</p>
              <button
                onClick={handleLogout}
                className="flex items-center space-x-1 mt-1 ml-auto px-3 py-1 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all duration-200 border border-red-500/30 hover:border-red-500/50 text-sm"
              >
                <LogOut size={14} />
                <span className="font-medium">Logout</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
};
