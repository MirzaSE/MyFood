import React from 'react';
import { Edit, Trash2, Search, Package, ChevronLeft, ChevronRight } from 'lucide-react';
import type { Ingredient } from '../../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  onSearch: (query: string) => void;
  isLoading?: boolean;
  currentPage?: number;
  totalPages?: number;
  onPageChange?: (page: number) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  onEdit,
  onDelete,
  onSearch,
  isLoading = false,
  currentPage = 1,
  totalPages = 1,
  onPageChange,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);
  const [showConfirm, setShowConfirm] = React.useState<number | null>(null);
  const [searchQuery, setSearchQuery] = React.useState('');

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch(searchQuery);
  };

  const handleDelete = async (id: number) => {
    try {
      setDeletingId(id);
      await onDelete(id);
      setShowConfirm(null);
    } finally {
      setDeletingId(null);
    }
  };

  const handleClearSearch = () => {
    setSearchQuery('');
    onSearch('');
  };

  if (isLoading && ingredients.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="relative w-16 h-16 mb-4">
          <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin" />
          <div className="absolute inset-2 bg-slate-900 rounded-full" />
        </div>
        <p className="text-gray-300 font-medium">Loading ingredients...</p>
      </div>
    );
  }

  if (ingredients.length === 0) {
    return (
      <div>
        {/* Search bar */}
        <form onSubmit={handleSearch} className="mb-6">
          <div className="flex gap-2">
            <div className="relative flex-1">
              <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Search ingredients..."
                className="w-full pl-10 pr-4 py-2.5 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-purple-500/50 focus:ring-2 focus:ring-purple-500/30 transition-all"
              />
            </div>
            <button
              type="submit"
              className="px-4 py-2.5 bg-purple-600/30 hover:bg-purple-600/50 text-purple-300 rounded-lg transition-all border border-purple-500/30"
            >
              Search
            </button>
          </div>
        </form>
        <div className="text-center py-16">
          <div className="flex justify-center mb-4">
            <div className="p-4 bg-purple-500/20 rounded-full">
              <Package size={32} className="text-purple-400" />
            </div>
          </div>
          <p className="text-gray-300 text-lg font-medium">No ingredients found</p>
          <p className="text-gray-400 text-sm mt-1">Create one to get started!</p>
        </div>
      </div>
    );
  }

  return (
    <div>
      {/* Search bar */}
      <form onSubmit={handleSearch} className="mb-6">
        <div className="flex gap-2">
          <div className="relative flex-1">
            <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Search ingredients..."
              className="w-full pl-10 pr-4 py-2.5 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-purple-500/50 focus:ring-2 focus:ring-purple-500/30 transition-all"
            />
          </div>
          <button
            type="submit"
            className="px-4 py-2.5 bg-purple-600/30 hover:bg-purple-600/50 text-purple-300 rounded-lg transition-all border border-purple-500/30"
          >
            Search
          </button>
          {searchQuery && (
            <button
              type="button"
              onClick={handleClearSearch}
              className="px-4 py-2.5 bg-white/5 hover:bg-white/10 text-gray-400 rounded-lg transition-all border border-white/10"
            >
              Clear
            </button>
          )}
        </div>
      </form>

      {/* Table */}
      <div className="overflow-x-auto rounded-xl border border-white/10">
        <table className="w-full">
          <thead>
            <tr className="bg-white/5">
              <th className="text-left px-4 py-3 text-sm font-semibold text-gray-300">Name</th>
              <th className="text-left px-4 py-3 text-sm font-semibold text-gray-300">Unit</th>
              <th className="text-left px-4 py-3 text-sm font-semibold text-gray-300">Calories</th>
              <th className="text-left px-4 py-3 text-sm font-semibold text-gray-300">Protein</th>
              <th className="text-left px-4 py-3 text-sm font-semibold text-gray-300">Carbs</th>
              <th className="text-left px-4 py-3 text-sm font-semibold text-gray-300">Fat</th>
              <th className="text-right px-4 py-3 text-sm font-semibold text-gray-300">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/5">
            {ingredients.map((ingredient) => (
              <tr
                key={ingredient.id}
                className="hover:bg-white/5 transition-colors"
              >
                <td className="px-4 py-3 text-white font-medium">{ingredient.name}</td>
                <td className="px-4 py-3">
                  <span className="px-2 py-1 bg-purple-500/20 text-purple-300 text-xs rounded-full border border-purple-500/30">
                    {ingredient.unit}
                  </span>
                </td>
                <td className="px-4 py-3 text-gray-300">{ingredient.caloriesPerUnit}</td>
                <td className="px-4 py-3 text-gray-300">{ingredient.protein}g</td>
                <td className="px-4 py-3 text-gray-300">{ingredient.carbs}g</td>
                <td className="px-4 py-3 text-gray-300">{ingredient.fat}g</td>
                <td className="px-4 py-3">
                  <div className="flex items-center justify-end gap-2">
                    <button
                      onClick={() => onEdit(ingredient)}
                      className="p-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 rounded-lg transition-all border border-blue-500/30 disabled:opacity-50"
                      disabled={isLoading || deletingId === ingredient.id}
                      title="Edit"
                    >
                      <Edit size={16} />
                    </button>
                    <div className="relative">
                      <button
                        onClick={() => setShowConfirm(ingredient.id)}
                        className="p-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 rounded-lg transition-all border border-red-500/30 disabled:opacity-50"
                        disabled={isLoading || deletingId !== null}
                        title="Delete"
                      >
                        <Trash2 size={16} />
                      </button>
                      {showConfirm === ingredient.id && (
                        <div className="absolute right-0 top-full mt-2 bg-slate-900 border border-red-500/50 rounded-lg p-4 z-10 w-48 shadow-xl">
                          <p className="text-sm text-gray-200 mb-3">Delete this item?</p>
                          <div className="flex gap-2">
                            <button
                              onClick={() => setShowConfirm(null)}
                              className="flex-1 px-2 py-1.5 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded text-xs font-medium"
                              disabled={deletingId === ingredient.id}
                            >
                              Cancel
                            </button>
                            <button
                              onClick={() => handleDelete(ingredient.id)}
                              className="flex-1 px-2 py-1.5 bg-red-500 hover:bg-red-600 text-white rounded text-xs font-medium"
                              disabled={deletingId === ingredient.id}
                            >
                              {deletingId === ingredient.id ? '...' : 'Delete'}
                            </button>
                          </div>
                        </div>
                      )}
                    </div>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {totalPages > 1 && onPageChange && (
        <div className="flex items-center justify-center gap-2 mt-6">
          <button
            onClick={() => onPageChange(currentPage - 1)}
            disabled={currentPage <= 1}
            className="p-2 bg-white/5 hover:bg-white/10 text-gray-300 rounded-lg transition-all border border-white/10 disabled:opacity-30 disabled:cursor-not-allowed"
          >
            <ChevronLeft size={18} />
          </button>
          <span className="text-sm text-gray-400 px-3">
            Page {currentPage} of {totalPages}
          </span>
          <button
            onClick={() => onPageChange(currentPage + 1)}
            disabled={currentPage >= totalPages}
            className="p-2 bg-white/5 hover:bg-white/10 text-gray-300 rounded-lg transition-all border border-white/10 disabled:opacity-30 disabled:cursor-not-allowed"
          >
            <ChevronRight size={18} />
          </button>
        </div>
      )}
    </div>
  );
};
