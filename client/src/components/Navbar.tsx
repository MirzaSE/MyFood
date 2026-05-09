import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { LogOut, ChefHat } from 'lucide-react';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const linkClassName = ({ isActive }: { isActive: boolean }) =>
    `rounded-full px-4 py-2 text-sm font-semibold transition ${isActive ? 'bg-amber-500 text-slate-950' : 'text-gray-200 hover:bg-white/10'}`;

  return (
    <nav className="sticky top-0 z-50 border-b border-white/10 bg-[linear-gradient(90deg,#111827_0%,#78350f_48%,#111827_100%)] backdrop-blur-lg">
      <div className="container mx-auto px-6 py-4">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex items-center gap-3">
            <div className="rounded-2xl bg-amber-500 p-2 text-slate-950">
              <ChefHat size={24} />
            </div>
            <div>
              <h1 className="text-2xl font-black text-white">MyFood</h1>
              <p className="text-xs uppercase tracking-[0.25em] text-amber-200">Food and Ingredient Studio</p>
            </div>
          </div>

          <div className="flex flex-wrap items-center gap-2">
            <NavLink to="/foods" className={linkClassName}>Foods</NavLink>
            <NavLink to="/foods/new" className={linkClassName}>Create Food</NavLink>
            <NavLink to="/ingredients" className={linkClassName}>Ingredients</NavLink>
          </div>

          <div className="flex items-center gap-4 self-end lg:self-auto">
            <p className="text-sm text-gray-300">Signed in as {username}</p>
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 rounded-full border border-red-500/30 bg-red-500/10 px-4 py-2 text-red-200 transition hover:bg-red-500/20"
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
