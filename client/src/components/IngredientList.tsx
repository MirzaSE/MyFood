import React, { useState } from 'react';
import { Edit2, Trash2, Eye, ChevronLeft, ChevronRight } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  totalCount: number;
  page: number;
  pageCount: number;
  searchQuery: string;
  isLoading: boolean;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => void;
  onView: (ingredient: Ingredient) => void;
  onPageChange: (page: number) => void;
  onSearchChange: (query: string) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  totalCount,
  page,
  pageCount,
  searchQuery,
  isLoading,
  onEdit,
  onDelete,
  onView,
  onPageChange,
  onSearchChange,
}) => {
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const totalPages = Math.ceil(totalCount / pageCount);

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this ingredient?')) return;
    setDeletingId(id);
    try {
      await onDelete(id);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div>
      <div className="mb-4">
        <input
          type="text"
          value={searchQuery}
          onChange={e => onSearchChange(e.target.value)}
          placeholder="Search ingredients..."
          className="w-full max-w-sm px-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
        />
      </div>

      {isLoading ? (
        <div className="flex flex-col items-center justify-center py-16">
          <div className="relative w-12 h-12 mb-3">
            <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>
            <div className="absolute inset-2 bg-slate-900 rounded-full"></div>
          </div>
          <p className="text-gray-400">Loading ingredients...</p>
        </div>
      ) : ingredients.length === 0 ? (
        <div className="text-center py-16 text-gray-400">
          {searchQuery ? `No ingredients match "${searchQuery}"` : 'No ingredients yet. Add your first one!'}
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-white/10">
          <table className="w-full text-sm">
            <thead>
              <tr className="bg-white/5 text-gray-400 text-left">
                <th className="px-4 py-3 font-semibold">Name</th>
                <th className="px-4 py-3 font-semibold">Unit</th>
                <th className="px-4 py-3 font-semibold">Cal/Unit</th>
                <th className="px-4 py-3 font-semibold">Protein</th>
                <th className="px-4 py-3 font-semibold">Carbs</th>
                <th className="px-4 py-3 font-semibold">Fat</th>
                <th className="px-4 py-3 font-semibold text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/5">
              {ingredients.map(ingredient => (
                <tr key={ingredient.id} className="hover:bg-white/5 transition-colors text-gray-200">
                  <td className="px-4 py-3 font-medium text-white">{ingredient.name}</td>
                  <td className="px-4 py-3 text-gray-400">{ingredient.unit}</td>
                  <td className="px-4 py-3">{ingredient.caloriesPerUnit.toFixed(2)}</td>
                  <td className="px-4 py-3">{ingredient.protein.toFixed(1)}g</td>
                  <td className="px-4 py-3">{ingredient.carbs.toFixed(1)}g</td>
                  <td className="px-4 py-3">{ingredient.fat.toFixed(1)}g</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end space-x-2">
                      <button
                        onClick={() => onView(ingredient)}
                        className="p-1.5 text-gray-400 hover:text-blue-400 hover:bg-blue-400/10 rounded-lg transition-all"
                        title="View"
                      >
                        <Eye size={15} />
                      </button>
                      <button
                        onClick={() => onEdit(ingredient)}
                        className="p-1.5 text-gray-400 hover:text-purple-400 hover:bg-purple-400/10 rounded-lg transition-all"
                        title="Edit"
                      >
                        <Edit2 size={15} />
                      </button>
                      <button
                        onClick={() => handleDelete(ingredient.id)}
                        disabled={deletingId === ingredient.id}
                        className="p-1.5 text-gray-400 hover:text-red-400 hover:bg-red-400/10 rounded-lg transition-all disabled:opacity-50"
                        title="Delete"
                      >
                        <Trash2 size={15} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {totalPages > 1 && (
        <div className="flex items-center justify-between mt-4 text-sm text-gray-400">
          <span>
            Page {page} of {totalPages} ({totalCount} total)
          </span>
          <div className="flex space-x-2">
            <button
              onClick={() => onPageChange(page - 1)}
              disabled={page <= 1}
              className="p-1.5 border border-white/20 rounded-lg hover:border-white/40 disabled:opacity-40 disabled:cursor-not-allowed transition-all"
            >
              <ChevronLeft size={16} />
            </button>
            <button
              onClick={() => onPageChange(page + 1)}
              disabled={page >= totalPages}
              className="p-1.5 border border-white/20 rounded-lg hover:border-white/40 disabled:opacity-40 disabled:cursor-not-allowed transition-all"
            >
              <ChevronRight size={16} />
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
