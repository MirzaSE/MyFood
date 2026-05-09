import React, { useState } from 'react';
import { Edit, Trash2, Eye, Search, Carrot, ChevronLeft, ChevronRight } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  search: string;
  onSearchChange: (value: string) => void;
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onEdit: (ingredient: Ingredient) => void;
  onView: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading = false,
  search,
  onSearchChange,
  page,
  totalPages,
  onPageChange,
  onEdit,
  onView,
  onDelete,
}) => {
  const [confirmId, setConfirmId] = useState<number | null>(null);
  const [deletingId, setDeletingId] = useState<number | null>(null);

  const handleDelete = async (id: number) => {
    try {
      setDeletingId(id);
      await onDelete(id);
      setConfirmId(null);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="relative">
        <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
        <input
          type="text"
          value={search}
          onChange={(e) => onSearchChange(e.target.value)}
          placeholder="Search ingredients by name…"
          className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
        />
      </div>

      {isLoading ? (
        <div className="flex flex-col items-center justify-center py-16">
          <div className="relative w-12 h-12 mb-4">
            <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>
            <div className="absolute inset-1.5 bg-slate-900 rounded-full"></div>
          </div>
          <p className="text-gray-300 text-sm">Loading ingredients…</p>
        </div>
      ) : ingredients.length === 0 ? (
        <div className="text-center py-16">
          <div className="flex justify-center mb-4">
            <div className="p-4 bg-purple-500/20 rounded-full">
              <Carrot size={32} className="text-purple-400" />
            </div>
          </div>
          <p className="text-gray-300 text-lg font-medium">No ingredients found</p>
          <p className="text-gray-400 text-sm mt-1">Create one to get started.</p>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-white/10 bg-gradient-to-br from-slate-800 to-slate-900">
          <table className="w-full text-left">
            <thead className="bg-white/5 text-gray-300 text-xs uppercase tracking-wider">
              <tr>
                <th className="px-6 py-3">Name</th>
                <th className="px-6 py-3">Unit</th>
                <th className="px-6 py-3">Calories / unit</th>
                <th className="px-6 py-3">Macros (P/C/F)</th>
                <th className="px-6 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/5">
              {ingredients.map((ingredient) => (
                <tr key={ingredient.id} className="hover:bg-white/5 transition-colors">
                  <td className="px-6 py-4 text-white font-semibold">{ingredient.name}</td>
                  <td className="px-6 py-4 text-purple-300">{ingredient.unit}</td>
                  <td className="px-6 py-4 text-gray-200">{ingredient.caloriesPerUnit}</td>
                  <td className="px-6 py-4 text-gray-300 text-sm">
                    {ingredient.protein.toFixed(1)} / {ingredient.carbs.toFixed(1)} / {ingredient.fat.toFixed(1)}
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex justify-end items-center gap-2">
                      <button
                        onClick={() => onView(ingredient)}
                        className="p-2 bg-purple-500/20 hover:bg-purple-500/30 text-purple-300 hover:text-purple-200 rounded-lg transition-all border border-purple-500/30 hover:border-purple-500/50"
                        title="View"
                      >
                        <Eye size={16} />
                      </button>
                      <button
                        onClick={() => onEdit(ingredient)}
                        className="p-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 hover:text-blue-200 rounded-lg transition-all border border-blue-500/30 hover:border-blue-500/50"
                        title="Edit"
                      >
                        <Edit size={16} />
                      </button>
                      <div className="relative">
                        <button
                          onClick={() => setConfirmId(ingredient.id)}
                          className="p-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all border border-red-500/30 hover:border-red-500/50"
                          title="Delete"
                        >
                          <Trash2 size={16} />
                        </button>
                        {confirmId === ingredient.id && (
                          <div className="absolute right-0 top-full mt-2 bg-slate-900 border border-red-500/50 rounded-lg p-3 z-20 w-56 shadow-xl">
                            <p className="text-xs text-gray-200 mb-3 font-medium">Delete this ingredient?</p>
                            <div className="flex space-x-2">
                              <button
                                onClick={() => setConfirmId(null)}
                                className="flex-1 px-2 py-1.5 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded text-xs"
                              >
                                Cancel
                              </button>
                              <button
                                onClick={() => handleDelete(ingredient.id)}
                                disabled={deletingId === ingredient.id}
                                className="flex-1 px-2 py-1.5 bg-red-500 hover:bg-red-600 text-white rounded text-xs disabled:opacity-50"
                              >
                                {deletingId === ingredient.id ? 'Deleting…' : 'Delete'}
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
      )}

      {ingredients.length > 0 && totalPages > 1 && (
        <div className="flex justify-center items-center space-x-3 text-gray-300 text-sm">
          <button
            onClick={() => onPageChange(Math.max(1, page - 1))}
            disabled={page <= 1}
            className="p-2 bg-white/10 hover:bg-white/20 rounded-lg border border-white/20 disabled:opacity-40"
          >
            <ChevronLeft size={16} />
          </button>
          <span>
            Page {page} of {totalPages}
          </span>
          <button
            onClick={() => onPageChange(Math.min(totalPages, page + 1))}
            disabled={page >= totalPages}
            className="p-2 bg-white/10 hover:bg-white/20 rounded-lg border border-white/20 disabled:opacity-40"
          >
            <ChevronRight size={16} />
          </button>
        </div>
      )}
    </div>
  );
};
