import { Apple, ChefHat, LogOut, PlusCircle } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { username, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="border-b border-gray-200 bg-white px-6 py-4 shadow-sm">
      <div className="mx-auto flex max-w-6xl flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div
          className="flex cursor-pointer items-center gap-3"
          onClick={() => navigate('/foods')}
        >
          <ChefHat className="text-blue-600" size={28} />
          <div>
            <h1 className="text-xl font-bold text-gray-900">MyFood</h1>
            <p className="text-sm text-gray-500">Food Management</p>
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-3">
          <button
            type="button"
            onClick={() => navigate('/foods')}
            className="rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-700 hover:bg-gray-50"
          >
            Foods
          </button>

          <button
            type="button"
            onClick={() => navigate('/ingredients')}
            className="flex items-center gap-2 rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-700 hover:bg-gray-50"
          >
            <Apple size={16} />
            Ingredients
          </button>

          <button
            type="button"
            onClick={() => navigate('/foods/create')}
            className="flex items-center gap-2 rounded-lg bg-blue-600 px-3 py-2 text-sm text-white hover:bg-blue-700"
          >
            <PlusCircle size={16} />
            Create Food
          </button>

          <span className="text-sm text-gray-600">
            Welcome back, {username}
          </span>

          <button
            type="button"
            onClick={handleLogout}
            className="flex items-center gap-2 rounded-lg border border-gray-300 px-3 py-2 text-sm text-gray-700 hover:bg-gray-50"
          >
            <LogOut size={16} />
            Logout
          </button>
        </div>
      </div>
    </nav>
  );
};