import React from 'react';
import { Edit, Eye, Search, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types';

interface IngredientListProps {
  ingredients: Ingredient[];
  totalCount: number;
  page: number;
  pageCount: number;
  search: string;
  isLoading?: boolean;
  onSearchChange: (value: string) => void;
  onPageChange: (page: number) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  totalCount,
  page,
  pageCount,
  search,
  isLoading = false,
  onSearchChange,
  onPageChange,
  onEdit,
  onDelete,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);
  const [viewing, setViewing] = React.useState<Ingredient | null>(null);
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
    <div className="space-y-6">
      <div className="flex flex-col gap-4 rounded-2xl border border-white/10 bg-white/5 p-5 md:flex-row md:items-center md:justify-between">
        <div className="relative w-full md:max-w-md">
          <Search size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            value={search}
            onChange={(event) => onSearchChange(event.target.value)}
            placeholder="Search ingredients..."
            className="w-full rounded-xl border border-white/10 bg-slate-950 py-3 pl-11 pr-4 text-white"
          />
        </div>
        <p className="text-sm text-gray-400">{totalCount} ingredients</p>
      </div>

      {isLoading ? (
        <div className="rounded-2xl border border-white/10 bg-slate-900/60 p-8 text-center text-gray-300">Loading ingredients...</div>
      ) : ingredients.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-white/15 bg-slate-900/50 p-8 text-center text-gray-400">No ingredients found.</div>
      ) : (
        <div className="overflow-hidden rounded-2xl border border-white/10 bg-slate-900/60">
          <table className="min-w-full divide-y divide-white/10">
            <thead className="bg-white/5 text-left text-sm uppercase tracking-wide text-gray-400">
              <tr>
                <th className="px-5 py-4">Name</th>
                <th className="px-5 py-4">Unit</th>
                <th className="px-5 py-4">Calories</th>
                <th className="px-5 py-4">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/5">
              {ingredients.map((ingredient) => (
                <tr key={ingredient.id} className="text-sm text-gray-200">
                  <td className="px-5 py-4 font-semibold text-white">{ingredient.name}</td>
                  <td className="px-5 py-4">{ingredient.unit}</td>
                  <td className="px-5 py-4">{ingredient.caloriesPerUnit}</td>
                  <td className="px-5 py-4">
                    <div className="flex flex-wrap gap-2">
                      <button onClick={() => setViewing(ingredient)} className="rounded-lg border border-white/10 px-3 py-2 text-xs text-gray-200 transition hover:bg-white/10">
                        <span className="inline-flex items-center gap-2"><Eye size={14} />View</span>
                      </button>
                      <button onClick={() => onEdit(ingredient)} className="rounded-lg border border-blue-500/30 px-3 py-2 text-xs text-blue-200 transition hover:bg-blue-500/10">
                        <span className="inline-flex items-center gap-2"><Edit size={14} />Edit</span>
                      </button>
                      <button
                        onClick={() => handleDelete(ingredient.id)}
                        disabled={deletingId === ingredient.id}
                        className="rounded-lg border border-red-500/30 px-3 py-2 text-xs text-red-200 transition hover:bg-red-500/10 disabled:opacity-50"
                      >
                        <span className="inline-flex items-center gap-2"><Trash2 size={14} />{deletingId === ingredient.id ? 'Deleting...' : 'Delete'}</span>
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
          onClick={() => onPageChange(page - 1)}
          disabled={page <= 1}
          className="rounded-lg border border-white/10 px-4 py-2 text-sm text-gray-200 transition hover:bg-white/10 disabled:opacity-40"
        >
          Previous
        </button>
        <p className="text-sm text-gray-400">
          Page {page} of {totalPages}
        </p>
        <button
          onClick={() => onPageChange(page + 1)}
          disabled={page >= totalPages}
          className="rounded-lg border border-white/10 px-4 py-2 text-sm text-gray-200 transition hover:bg-white/10 disabled:opacity-40"
        >
          Next
        </button>
      </div>

      {viewing && (
        <div className="rounded-2xl border border-amber-500/30 bg-amber-500/10 p-5 text-sm text-amber-50">
          <div className="mb-3 flex items-center justify-between">
            <h3 className="text-lg font-bold">{viewing.name}</h3>
            <button onClick={() => setViewing(null)} className="text-amber-100 underline">Close</button>
          </div>
          <div className="grid gap-2 md:grid-cols-2">
            <p>Unit: {viewing.unit}</p>
            <p>Calories: {viewing.caloriesPerUnit}</p>
            <p>Protein: {viewing.protein}</p>
            <p>Carbs: {viewing.carbs}</p>
            <p>Fat: {viewing.fat}</p>
          </div>
        </div>
      )}
    </div>
  );
};
