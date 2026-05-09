import React from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { LogOut, ChefHat, Leaf, UtensilsCrossed } from "lucide-react";

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate("/login");
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

          <div className="flex items-center gap-4">
            <div className="hidden md:flex items-center gap-2">
              <NavLink
                to="/foods"
                className={({ isActive }) =>
                  `inline-flex items-center gap-2 px-3 py-2 rounded-lg border transition-all ${
                    isActive
                      ? "bg-white/15 border-white/25 text-white"
                      : "bg-white/5 border-white/10 text-gray-300 hover:text-white hover:bg-white/10"
                  }`
                }
              >
                <UtensilsCrossed size={17} />
                <span className="text-sm font-medium">Foods</span>
              </NavLink>
              <NavLink
                to="/ingredients"
                className={({ isActive }) =>
                  `inline-flex items-center gap-2 px-3 py-2 rounded-lg border transition-all ${
                    isActive
                      ? "bg-white/15 border-white/25 text-white"
                      : "bg-white/5 border-white/10 text-gray-300 hover:text-white hover:bg-white/10"
                  }`
                }
              >
                <Leaf size={17} />
                <span className="text-sm font-medium">Ingredients</span>
              </NavLink>
            </div>

            {/* User Info & Logout */}
            <div className="flex flex-col items-end gap-2">
              <p className="text-sm font-semibold text-white">
                Welcome back, {username}
              </p>
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
        <div className="md:hidden mt-4 grid grid-cols-2 gap-2">
          <NavLink
            to="/foods"
            className={({ isActive }) =>
              `inline-flex items-center justify-center gap-2 px-3 py-2 rounded-lg border transition-all ${
                isActive
                  ? "bg-white/15 border-white/25 text-white"
                  : "bg-white/5 border-white/10 text-gray-300"
              }`
            }
          >
            <UtensilsCrossed size={17} />
            <span className="text-sm font-medium">Foods</span>
          </NavLink>
          <NavLink
            to="/ingredients"
            className={({ isActive }) =>
              `inline-flex items-center justify-center gap-2 px-3 py-2 rounded-lg border transition-all ${
                isActive
                  ? "bg-white/15 border-white/25 text-white"
                  : "bg-white/5 border-white/10 text-gray-300"
              }`
            }
          >
            <Leaf size={17} />
            <span className="text-sm font-medium">Ingredients</span>
          </NavLink>
        </div>
      </div>
    </nav>
  );
};
