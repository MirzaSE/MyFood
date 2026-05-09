import React from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { LogOut, ChefHat, Plus } from "lucide-react";

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const isActive = (path: string) => location.pathname === path;

  return (
    <nav className="bg-gradient-to-r from-slate-900 via-purple-900 to-slate-900 border-b border-white/10 backdrop-blur-lg sticky top-0 z-50">
      <div className="container mx-auto px-6 py-4">
        <div className="flex justify-between items-center">
          {/* Logo */}
          <div className="flex items-center space-x-12">
            <div
              onClick={() => navigate("/foods")}
              className="flex items-center space-x-3 cursor-pointer hover:opacity-80 transition-opacity"
            >
              <div className="p-2 bg-gradient-to-br from-purple-500 to-blue-500 rounded-lg">
                <ChefHat size={24} className="text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-white">MyFood</h1>
                <p className="text-xs text-purple-300">Food Management</p>
              </div>
            </div>

            {/* Navigation Links */}
            <div className="flex gap-3 ml-8 lg:ml-12">
              <button
                onClick={() => navigate("/foods")}
                className={`px-4 py-2 rounded-lg font-medium transition-all duration-200 ${
                  isActive("/foods")
                    ? "bg-purple-500/30 text-purple-200 border border-purple-500/50"
                    : "text-gray-300 hover:text-white hover:bg-white/10"
                }`}
              >
                Foods
              </button>
              <button
                onClick={() => navigate("/foods/create")}
                className="flex items-center gap-2 px-4 py-2 rounded-lg font-medium transition-all duration-200 text-gray-300 hover:text-white hover:bg-white/10 border border-white/10"
              >
                <Plus size={16} />
                Create Food
              </button>
              <button
                onClick={() => navigate("/ingredients")}
                className={`px-4 py-2 rounded-lg font-medium transition-all duration-200 ${
                  isActive("/ingredients")
                    ? "bg-purple-500/30 text-purple-200 border border-purple-500/50"
                    : "text-gray-300 hover:text-white hover:bg-white/10"
                }`}
              >
                Ingredients
              </button>
            </div>
          </div>

          {/* User Info & Logout */}
          <div className="flex flex-col items-end gap-2">
            <p className="text-sm text-gray-300">
              Welcome back,{" "}
              <span className="text-white font-semibold">
                {username ?? "User"}
              </span>
            </p>
            <button
              onClick={handleLogout}
              className="flex items-center space-x-2 px-4 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all duration-200 border border-red-500/30 hover:border-red-500/50"
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
