import React, { useState } from 'react';
import { Edit, Trash2, Eye, Search, ChevronLeft, ChevronRight, Leaf } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  onView?: (ingredient: Ingredient) => void;
  searchQuery: string;
  onSearchChange: (query: string) => void;
  isLoading?: boolean;
}

const PAGE_SIZE = 6;

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  onEdit,
  onDelete,
  onView,
  searchQuery,
  onSearchChange,
  isLoading = false,
}) => {
  const [currentPage, setCurrentPage] = useState(1);
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [showConfirm, setShowConfirm] = useState<number | null>(null);

  const totalPages = Math.max(1, Math.ceil(ingredients.length / PAGE_SIZE));
  const paginatedIngredients = ingredients.slice(
    (currentPage - 1) * PAGE_SIZE,
    currentPage * PAGE_SIZE
  );

  const handleDelete = async (id: number) => {
    try {
      setDeletingId(id);
      await onDelete(id);
      setShowConfirm(null);
    } finally {
      setDeletingId(null);
    }
  };

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="relative w-16 h-16 mb-4">
          <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>
          <div className="absolute inset-2 bg-slate-900 rounded-full"></div>
        </div>
        <p className="text-gray-300 font-medium">Loading ingredients...</p>
      </div>
    );
  }

  return (
    <div>
      {/* Search Bar */}
      <div className="mb-6">
        <div className="relative max-w-md">
          <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => {
              onSearchChange(e.target.value);
              setCurrentPage(1);
            }}
            placeholder="Search ingredients..."
            className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
          />
        </div>
      </div>

      {/* Empty State */}
      {ingredients.length === 0 ? (
        <div className="text-center py-16">
          <div className="flex justify-center mb-4">
            <div className="p-4 bg-purple-500/20 rounded-full">
              <Leaf size={32} className="text-purple-400" />
            </div>
          </div>
          <p className="text-gray-300 text-lg font-medium">
            {searchQuery ? 'No ingredients match your search' : 'No ingredients found'}
          </p>
          <p className="text-gray-400 text-sm mt-1">
            {searchQuery ? 'Try a different search term' : 'Create one to get started!'}
          </p>
        </div>
      ) : (
        <>
          {/* Table */}
          <div className="overflow-x-auto rounded-xl border border-white/10">
            <table className="w-full">
              <thead>
                <tr className="bg-white/5 border-b border-white/10">
                  <th className="text-left px-6 py-4 text-sm font-semibold text-gray-300">Name</th>
                  <th className="text-left px-6 py-4 text-sm font-semibold text-gray-300">Quantity</th>
                  <th className="text-left px-6 py-4 text-sm font-semibold text-gray-300">Food ID</th>
                  <th className="text-right px-6 py-4 text-sm font-semibold text-gray-300">Actions</th>
                </tr>
              </thead>
              <tbody>
                {paginatedIngredients.map((ingredient) => (
                  <tr
                    key={ingredient.id}
                    className="border-b border-white/5 hover:bg-white/5 transition-colors"
                  >
                    <td className="px-6 py-4 text-white font-medium">{ingredient.name}</td>
                    <td className="px-6 py-4 text-gray-300">{ingredient.quantity || '—'}</td>
                    <td className="px-6 py-4 text-gray-300">{ingredient.foodId}</td>
                    <td className="px-6 py-4">
                      <div className="flex justify-end space-x-2 relative">
                        {onView && (
                          <button
                            onClick={() => onView(ingredient)}
                            className="p-2 bg-emerald-500/20 hover:bg-emerald-500/30 text-emerald-300 hover:text-emerald-200 rounded-lg transition-all border border-emerald-500/30 hover:border-emerald-500/50"
                            title="View"
                          >
                            <Eye size={16} />
                          </button>
                        )}
                        <button
                          onClick={() => onEdit(ingredient)}
                          className="p-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 hover:text-blue-200 rounded-lg transition-all border border-blue-500/30 hover:border-blue-500/50"
                          title="Edit"
                        >
                          <Edit size={16} />
                        </button>
                        <div className="relative">
                          <button
                            onClick={() => setShowConfirm(ingredient.id)}
                            className="p-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all border border-red-500/30 hover:border-red-500/50 disabled:opacity-50"
                            disabled={deletingId !== null}
                            title="Delete"
                          >
                            <Trash2 size={16} />
                          </button>
                          {showConfirm === ingredient.id && (
                            <div className="absolute right-0 top-full mt-2 bg-slate-900 border border-red-500/50 rounded-lg p-4 z-10 w-56 shadow-xl">
                              <p className="text-sm text-gray-200 mb-3 font-medium">Delete this ingredient?</p>
                              <div className="flex space-x-2">
                                <button
                                  onClick={() => setShowConfirm(null)}
                                  className="flex-1 px-3 py-2 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded-lg text-sm font-medium transition-all"
                                  disabled={deletingId === ingredient.id}
                                >
                                  Cancel
                                </button>
                                <button
                                  onClick={() => handleDelete(ingredient.id)}
                                  className="flex-1 px-3 py-2 bg-red-500 hover:bg-red-600 text-white rounded-lg text-sm font-medium transition-all"
                                  disabled={deletingId === ingredient.id}
                                >
                                  {deletingId === ingredient.id ? 'Deleting...' : 'Delete'}
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
          {totalPages > 1 && (
            <div className="flex items-center justify-between mt-6">
              <p className="text-sm text-gray-400">
                Showing {(currentPage - 1) * PAGE_SIZE + 1}–
                {Math.min(currentPage * PAGE_SIZE, ingredients.length)} of {ingredients.length}
              </p>
              <div className="flex items-center space-x-2">
                <button
                  onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                  disabled={currentPage === 1}
                  className="p-2 bg-white/10 hover:bg-white/20 text-gray-300 rounded-lg transition-all disabled:opacity-30 disabled:cursor-not-allowed border border-white/10"
                >
                  <ChevronLeft size={18} />
                </button>
                <span className="text-sm text-gray-300 px-3">
                  {currentPage} / {totalPages}
                </span>
                <button
                  onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                  disabled={currentPage === totalPages}
                  className="p-2 bg-white/10 hover:bg-white/20 text-gray-300 rounded-lg transition-all disabled:opacity-30 disabled:cursor-not-allowed border border-white/10"
                >
                  <ChevronRight size={18} />
                </button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
};
