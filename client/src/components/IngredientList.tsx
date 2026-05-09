import React, { useState } from 'react';
import { Edit, Trash2, Eye, Search, ChevronLeft, ChevronRight, Carrot } from 'lucide-react';
import type { Ingredient, PaginationMetadata } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  pagination: PaginationMetadata | null;
  isLoading?: boolean;
  searchQuery: string;
  onSearchChange: (q: string) => void;
  onPageChange: (page: number) => void;
  onEdit: (ing: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  onView?: (ing: Ingredient) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  pagination,
  isLoading = false,
  searchQuery,
  onSearchChange,
  onPageChange,
  onEdit,
  onDelete,
  onView,
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
    <div className="space-y-4">
      <div className="relative">
        <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
        <input
          type="text"
          value={searchQuery}
          onChange={(e) => onSearchChange(e.target.value)}
          placeholder="Search ingredients by name…"
          className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-sm transition-all"
        />
      </div>

      {isLoading ? (
        <div className="flex flex-col items-center justify-center py-16">
          <div className="relative w-12 h-12 mb-3">
            <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>
            <div className="absolute inset-2 bg-slate-900 rounded-full"></div>
          </div>
          <p className="text-gray-300 text-sm">Loading ingredients…</p>
        </div>
      ) : ingredients.length === 0 ? (
        <div className="text-center py-16">
          <div className="flex justify-center mb-3">
            <div className="p-3 bg-purple-500/20 rounded-full">
              <Carrot size={28} className="text-purple-400" />
            </div>
          </div>
          <p className="text-gray-300 font-medium">
            {searchQuery ? `No ingredients match "${searchQuery}"` : 'No ingredients yet'}
          </p>
          <p className="text-gray-400 text-sm mt-1">
            {searchQuery ? 'Try a different search term.' : 'Create one to get started.'}
          </p>
        </div>
      ) : (
        <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 rounded-xl overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead className="bg-white/5 border-b border-white/10">
                <tr className="text-gray-300 text-xs uppercase tracking-wide">
                  <th className="px-5 py-3 font-semibold">Name</th>
                  <th className="px-5 py-3 font-semibold">Unit</th>
                  <th className="px-5 py-3 font-semibold">Calories / unit</th>
                  <th className="px-5 py-3 font-semibold text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-white/5">
                {ingredients.map((ing) => (
                  <tr key={ing.id} className="hover:bg-white/5 transition-colors">
                    <td className="px-5 py-3 text-white font-medium">{ing.name}</td>
                    <td className="px-5 py-3 text-gray-300">{ing.unit}</td>
                    <td className="px-5 py-3 text-gray-300">{ing.caloriesPerUnit.toFixed(2)} kcal</td>
                    <td className="px-5 py-3">
                      <div className="flex items-center justify-end space-x-2">
                        {onView && (
                          <button
                            onClick={() => onView(ing)}
                            className="p-2 bg-slate-700/30 hover:bg-slate-700/60 text-gray-300 hover:text-white rounded-lg border border-white/10 transition-colors"
                            title="View"
                          >
                            <Eye size={16} />
                          </button>
                        )}
                        <button
                          onClick={() => onEdit(ing)}
                          className="p-2 bg-purple-500/20 hover:bg-purple-500/40 text-purple-300 hover:text-purple-200 rounded-lg border border-purple-500/30 transition-colors"
                          title="Edit"
                        >
                          <Edit size={16} />
                        </button>
                        {confirmId === ing.id ? (
                          <div className="flex items-center space-x-1">
                            <button
                              onClick={() => handleDelete(ing.id)}
                              disabled={deletingId === ing.id}
                              className="px-2 py-1 bg-red-500/30 hover:bg-red-500/50 text-red-200 text-xs rounded-md border border-red-500/40 disabled:opacity-50"
                            >
                              {deletingId === ing.id ? '…' : 'Confirm'}
                            </button>
                            <button
                              onClick={() => setConfirmId(null)}
                              className="px-2 py-1 bg-white/10 hover:bg-white/20 text-gray-300 text-xs rounded-md border border-white/20"
                            >
                              Cancel
                            </button>
                          </div>
                        ) : (
                          <button
                            onClick={() => setConfirmId(ing.id)}
                            className="p-2 bg-red-500/20 hover:bg-red-500/40 text-red-300 hover:text-red-200 rounded-lg border border-red-500/30 transition-colors"
                            title="Delete"
                          >
                            <Trash2 size={16} />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {pagination && pagination.totalPages > 1 && (
        <div className="flex items-center justify-between px-2">
          <p className="text-sm text-gray-400">
            Page {pagination.currentPage} of {pagination.totalPages} · {pagination.totalCount} total
          </p>
          <div className="flex items-center space-x-2">
            <button
              onClick={() => onPageChange(Math.max(1, pagination.currentPage - 1))}
              disabled={pagination.currentPage <= 1}
              className="flex items-center space-x-1 px-3 py-2 bg-white/5 hover:bg-white/10 text-gray-300 rounded-lg border border-white/10 disabled:opacity-40 disabled:cursor-not-allowed text-sm"
            >
              <ChevronLeft size={16} />
              <span>Prev</span>
            </button>
            <button
              onClick={() => onPageChange(Math.min(pagination.totalPages, pagination.currentPage + 1))}
              disabled={pagination.currentPage >= pagination.totalPages}
              className="flex items-center space-x-1 px-3 py-2 bg-white/5 hover:bg-white/10 text-gray-300 rounded-lg border border-white/10 disabled:opacity-40 disabled:cursor-not-allowed text-sm"
            >
              <span>Next</span>
              <ChevronRight size={16} />
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
