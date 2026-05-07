import React from 'react';
import { Eye, Pencil, Search, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  page: number;
  pageCount: number;
  search: string;
  totalCount: number;
  onDelete: (id: number) => Promise<void>;
  onEdit: (ingredient: Ingredient) => void;
  onPageChange: (page: number) => void;
  onSearchChange: (search: string) => void;
  onView: (ingredient: Ingredient) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading = false,
  page,
  pageCount,
  search,
  totalCount,
  onDelete,
  onEdit,
  onPageChange,
  onSearchChange,
  onView,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);
  const totalPages = Math.max(1, Math.ceil(totalCount / pageCount));

  const handleDelete = async (id: number) => {
    try {
      setDeletingId(id);
      await onDelete(id);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="rounded-2xl border border-white/10 bg-slate-900/70 shadow-2xl overflow-hidden">
      <div className="p-5 border-b border-white/10 flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h2 className="text-xl font-bold text-white">Ingredient Catalog</h2>
          <p className="text-sm text-gray-400">{totalCount} reusable {totalCount === 1 ? 'ingredient' : 'ingredients'}</p>
        </div>
        <div className="relative w-full md:w-80">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
          <input
            className="w-full pl-10 pr-4 py-3 rounded-xl bg-white/10 border border-white/20 text-white placeholder-gray-400 focus:outline-none focus:border-emerald-400"
            onChange={(event) => onSearchChange(event.target.value)}
            placeholder="Search ingredients..."
            type="search"
            value={search}
          />
        </div>
      </div>

      {isLoading ? (
        <div className="py-20 text-center text-gray-300">Loading ingredients...</div>
      ) : ingredients.length === 0 ? (
        <div className="py-20 text-center">
          <p className="text-lg font-semibold text-white">No ingredients found</p>
          <p className="text-sm text-gray-400 mt-1">Create one or adjust your search.</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead className="bg-white/5 text-xs uppercase tracking-wider text-gray-400">
              <tr>
                <th className="px-5 py-4">Name</th>
                <th className="px-5 py-4">Unit</th>
                <th className="px-5 py-4">Calories</th>
                <th className="px-5 py-4">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/10">
              {ingredients.map((ingredient) => (
                <tr className="text-gray-200 hover:bg-white/5" key={ingredient.id}>
                  <td className="px-5 py-4 font-semibold text-white">{ingredient.name}</td>
                  <td className="px-5 py-4">{ingredient.unit}</td>
                  <td className="px-5 py-4">{ingredient.caloriesPerUnit} kcal/{ingredient.unit}</td>
                  <td className="px-5 py-4">
                    <div className="flex flex-wrap gap-2">
                      <button className="p-2 rounded-lg bg-cyan-500/15 text-cyan-200 hover:bg-cyan-500/25" onClick={() => onView(ingredient)} title="View" type="button">
                        <Eye size={16} />
                      </button>
                      <button className="p-2 rounded-lg bg-blue-500/15 text-blue-200 hover:bg-blue-500/25" onClick={() => onEdit(ingredient)} title="Edit" type="button">
                        <Pencil size={16} />
                      </button>
                      <button
                        className="p-2 rounded-lg bg-red-500/15 text-red-200 hover:bg-red-500/25 disabled:opacity-50"
                        disabled={deletingId === ingredient.id}
                        onClick={() => handleDelete(ingredient.id)}
                        title="Delete"
                        type="button"
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
      )}

      <div className="p-5 border-t border-white/10 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 text-sm text-gray-300">
        <span>Page {page} of {totalPages}</span>
        <div className="flex gap-2">
          <button
            className="px-4 py-2 rounded-lg border border-white/20 hover:border-white/40 disabled:opacity-40"
            disabled={page <= 1}
            onClick={() => onPageChange(page - 1)}
            type="button"
          >
            Previous
          </button>
          <button
            className="px-4 py-2 rounded-lg border border-white/20 hover:border-white/40 disabled:opacity-40"
            disabled={page >= totalPages}
            onClick={() => onPageChange(page + 1)}
            type="button"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
};
