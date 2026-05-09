import React from 'react';
import { Eye, Pencil, Search, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  searchTerm: string;
  onSearchChange: (value: string) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  onView: (ingredient: Ingredient) => void;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading = false,
  searchTerm,
  onSearchChange,
  onEdit,
  onDelete,
  onView,
  page,
  pageSize,
  totalCount,
  onPageChange,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  const handleDelete = async (id: number) => {
    try {
      setDeletingId(id);
      await onDelete(id);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row gap-4 md:items-center md:justify-between">
        <div className="relative w-full md:max-w-md">
          <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            value={searchTerm}
            onChange={(event) => onSearchChange(event.target.value)}
            placeholder="Search ingredients..."
            className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-purple-500"
          />
        </div>
        <div className="text-sm text-gray-400">{totalCount} ingredient{totalCount === 1 ? '' : 's'}</div>
      </div>

      {isLoading ? (
        <div className="rounded-xl border border-white/10 bg-white/5 px-6 py-10 text-center text-gray-300">
          Loading ingredients...
        </div>
      ) : ingredients.length === 0 ? (
        <div className="rounded-xl border border-white/10 bg-white/5 px-6 py-10 text-center text-gray-300">
          No ingredients found.
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-white/10 bg-slate-900/60">
          <table className="min-w-full text-left text-sm text-gray-200">
            <thead className="bg-white/5 text-gray-300 uppercase text-xs tracking-wider">
              <tr>
                <th className="px-4 py-4">Name</th>
                <th className="px-4 py-4">Unit</th>
                <th className="px-4 py-4">Calories</th>
                <th className="px-4 py-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              {ingredients.map((ingredient) => (
                <tr key={ingredient.id} className="border-t border-white/10">
                  <td className="px-4 py-4 font-medium text-white">{ingredient.name}</td>
                  <td className="px-4 py-4">{ingredient.unit}</td>
                  <td className="px-4 py-4">{ingredient.caloriesPerUnit.toFixed(2)}</td>
                  <td className="px-4 py-4">
                    <div className="flex flex-wrap gap-2">
                      <button
                        onClick={() => onView(ingredient)}
                        className="inline-flex items-center gap-1 rounded-lg border border-white/20 px-3 py-2 text-xs text-gray-200 hover:border-white/40"
                      >
                        <Eye size={14} /> View
                      </button>
                      <button
                        onClick={() => onEdit(ingredient)}
                        className="inline-flex items-center gap-1 rounded-lg border border-blue-500/40 px-3 py-2 text-xs text-blue-200 hover:bg-blue-500/10"
                      >
                        <Pencil size={14} /> Edit
                      </button>
                      <button
                        onClick={() => handleDelete(ingredient.id)}
                        disabled={deletingId === ingredient.id}
                        className="inline-flex items-center gap-1 rounded-lg border border-red-500/40 px-3 py-2 text-xs text-red-200 hover:bg-red-500/10 disabled:opacity-50"
                      >
                        <Trash2 size={14} /> {deletingId === ingredient.id ? 'Deleting...' : 'Delete'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="flex items-center justify-between">
        <button
          onClick={() => onPageChange(Math.max(1, page - 1))}
          disabled={page === 1}
          className="rounded-lg border border-white/20 px-4 py-2 text-sm text-gray-200 disabled:opacity-40"
        >
          Previous
        </button>
        <span className="text-sm text-gray-400">Page {page} of {totalPages}</span>
        <button
          onClick={() => onPageChange(Math.min(totalPages, page + 1))}
          disabled={page >= totalPages}
          className="rounded-lg border border-white/20 px-4 py-2 text-sm text-gray-200 disabled:opacity-40"
        >
          Next
        </button>
      </div>
    </div>
  );
};