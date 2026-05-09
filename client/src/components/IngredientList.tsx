import React, { useState } from 'react';
import { Edit, Trash2, Eye, Leaf } from 'lucide-react';
import type { Ingredient } from '../types';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  onView?: (ingredient: Ingredient) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading = false,
  onEdit,
  onDelete,
  onView,
}) => {
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [showConfirm, setShowConfirm] = useState<number | null>(null);

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

  if (ingredients.length === 0) {
    return (
      <div className="text-center py-16">
        <div className="flex justify-center mb-4">
          <div className="p-4 bg-purple-500/20 rounded-full">
            <Leaf size={32} className="text-purple-400" />
          </div>
        </div>
        <p className="text-gray-300 text-lg font-medium">No ingredients found</p>
        <p className="text-gray-400 text-sm mt-1">Create one to get started!</p>
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-xl border border-white/10 bg-gradient-to-br from-slate-800 to-slate-900">
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-white/10">
          <thead className="bg-white/5">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-gray-300">Name</th>
              <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-gray-300">Unit</th>
              <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-gray-300">Cal/unit</th>
              <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-gray-300">P</th>
              <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-gray-300">C</th>
              <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-gray-300">F</th>
              <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-gray-300">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/5">
            {ingredients.map((ingredient) => (
              <tr key={ingredient.id} className="hover:bg-white/5 transition-colors">
                <td className="px-4 py-3 text-white font-medium">{ingredient.name}</td>
                <td className="px-4 py-3 text-gray-300">{ingredient.unit}</td>
                <td className="px-4 py-3 text-right text-gray-200">{Number(ingredient.caloriesPerUnit).toFixed(2)}</td>
                <td className="px-4 py-3 text-right text-gray-400">{Number(ingredient.protein).toFixed(2)}</td>
                <td className="px-4 py-3 text-right text-gray-400">{Number(ingredient.carbs).toFixed(2)}</td>
                <td className="px-4 py-3 text-right text-gray-400">{Number(ingredient.fat).toFixed(2)}</td>
                <td className="px-4 py-3 text-right">
                  <div className="flex justify-end items-center gap-2">
                    {onView && (
                      <button
                        onClick={() => onView(ingredient)}
                        className="p-2 bg-purple-500/20 hover:bg-purple-500/30 text-purple-300 hover:text-purple-200 rounded-lg transition-all border border-purple-500/30 hover:border-purple-500/50"
                        title="View"
                      >
                        <Eye size={16} />
                      </button>
                    )}
                    <button
                      onClick={() => onEdit(ingredient)}
                      className="p-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 hover:text-blue-200 rounded-lg transition-all border border-blue-500/30 hover:border-blue-500/50 disabled:opacity-50"
                      disabled={deletingId === ingredient.id}
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
                        <div className="absolute right-0 top-full mt-2 bg-slate-900 border border-red-500/50 rounded-lg p-4 z-10 w-56 shadow-xl text-left">
                          <p className="text-sm text-gray-200 mb-3 font-medium">Delete this ingredient?</p>
                          <p className="text-xs text-gray-400 mb-4">This action cannot be undone.</p>
                          <div className="flex space-x-2">
                            <button
                              onClick={() => setShowConfirm(null)}
                              className="flex-1 px-3 py-2 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded-lg text-sm font-medium transition-all disabled:opacity-50"
                              disabled={deletingId === ingredient.id}
                            >
                              Cancel
                            </button>
                            <button
                              onClick={() => handleDelete(ingredient.id)}
                              className="flex-1 px-3 py-2 bg-red-500 hover:bg-red-600 text-white rounded-lg text-sm font-medium transition-all disabled:opacity-50"
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
    </div>
  );
};
