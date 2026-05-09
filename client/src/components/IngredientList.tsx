import React from 'react';
import { Pencil, Trash2, ChevronLeft, ChevronRight } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface Props {
  ingredients: Ingredient[];
  total: number;
  page: number;
  pageCount: number;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => void;
  onPageChange: (page: number) => void;
  isLoading?: boolean;
}

export const IngredientList: React.FC<Props> = ({
  ingredients,
  total,
  page,
  pageCount,
  onEdit,
  onDelete,
  onPageChange,
  isLoading,
}) => {
  const totalPages = Math.ceil(total / pageCount) || 1;

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="relative w-16 h-16 mb-4">
          <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin" />
          <div className="absolute inset-2 bg-slate-900 rounded-full" />
        </div>
        <p className="text-gray-300 font-medium">Loading ingredients…</p>
      </div>
    );
  }

  if (ingredients.length === 0) {
    return (
      <div className="text-center py-20">
        <p className="text-gray-400 text-lg">No ingredients found.</p>
        <p className="text-gray-500 text-sm mt-1">Add your first ingredient above.</p>
      </div>
    );
  }

  return (
    <div>
      <div className="overflow-x-auto rounded-xl border border-white/10">
        <table className="w-full text-sm text-left">
          <thead className="bg-white/5 text-gray-400 uppercase text-xs tracking-wider">
            <tr>
              <th className="px-6 py-4">Name</th>
              <th className="px-6 py-4">Unit / Quantity</th>
              <th className="px-6 py-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/5">
            {ingredients.map((ing) => (
              <tr key={ing.id} className="bg-white/[0.02] hover:bg-white/5 transition">
                <td className="px-6 py-4 font-medium text-white">{ing.name}</td>
                <td className="px-6 py-4 text-gray-400">{ing.quantity ?? '—'}</td>
                <td className="px-6 py-4">
                  <div className="flex justify-end space-x-2">
                    <button
                      onClick={() => onEdit(ing)}
                      className="p-2 rounded-lg text-blue-400 hover:text-blue-300 hover:bg-blue-500/10 transition"
                      title="Edit"
                    >
                      <Pencil size={16} />
                    </button>
                    <button
                      onClick={() => onDelete(ing.id)}
                      className="p-2 rounded-lg text-red-400 hover:text-red-300 hover:bg-red-500/10 transition"
                      title="Delete"
                    >
                      <Trash2 size={16} />
                    </button>
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
            Page {page} of {totalPages} &nbsp;·&nbsp; {total} total
          </p>
          <div className="flex space-x-2">
            <button
              onClick={() => onPageChange(page - 1)}
              disabled={page <= 1}
              className="p-2 rounded-lg text-gray-400 hover:text-white hover:bg-white/10 transition disabled:opacity-30 disabled:cursor-not-allowed"
            >
              <ChevronLeft size={18} />
            </button>
            <button
              onClick={() => onPageChange(page + 1)}
              disabled={page >= totalPages}
              className="p-2 rounded-lg text-gray-400 hover:text-white hover:bg-white/10 transition disabled:opacity-30 disabled:cursor-not-allowed"
            >
              <ChevronRight size={18} />
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
