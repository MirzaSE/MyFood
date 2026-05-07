import React from 'react';
import { Edit, Eye, Search, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  search: string;
  onSearchChange: (value: string) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (ingredient: Ingredient) => Promise<void>;
  onView: (ingredient: Ingredient) => void;
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading = false,
  search,
  onSearchChange,
  onEdit,
  onDelete,
  onView,
  currentPage,
  totalPages,
  onPageChange,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);

  const handleDelete = async (ingredient: Ingredient) => {
    try {
      setDeletingId(ingredient.id);
      await onDelete(ingredient);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-5">
      <div className="flex items-center gap-3 bg-slate-800/70 border border-white/10 rounded-xl p-3">
        <Search size={18} className="text-gray-400" />
        <input
          type="text"
          value={search}
          onChange={(e) => onSearchChange(e.target.value)}
          placeholder="Search ingredients..."
          className="w-full bg-transparent text-white placeholder-gray-400 focus:outline-none"
        />
      </div>

      <div className="overflow-x-auto border border-white/10 rounded-xl bg-slate-900/60">
        <table className="w-full text-left">
          <thead className="bg-slate-800/90 text-gray-300 text-sm uppercase tracking-wider">
            <tr>
              <th className="px-4 py-3">Name</th>
              <th className="px-4 py-3">Unit</th>
              <th className="px-4 py-3">Calories</th>
              <th className="px-4 py-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/10">
            {!isLoading &&
              ingredients.map((ingredient) => (
                <tr key={ingredient.id} className="hover:bg-white/5 transition-colors">
                  <td className="px-4 py-3 text-white font-medium">{ingredient.name}</td>
                  <td className="px-4 py-3 text-gray-300">{ingredient.unit}</td>
                  <td className="px-4 py-3 text-gray-300">{ingredient.caloriesPerUnit.toFixed(1)}</td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-2">
                      <button
                        onClick={() => onView(ingredient)}
                        className="px-3 py-2 rounded-lg bg-indigo-500/20 hover:bg-indigo-500/30 text-indigo-300 border border-indigo-500/40"
                      >
                        <Eye size={16} />
                      </button>
                      <button
                        onClick={() => onEdit(ingredient)}
                        className="px-3 py-2 rounded-lg bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 border border-blue-500/40"
                      >
                        <Edit size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(ingredient)}
                        disabled={deletingId === ingredient.id}
                        className="px-3 py-2 rounded-lg bg-red-500/20 hover:bg-red-500/30 text-red-300 border border-red-500/40 disabled:opacity-60"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>

        {isLoading && <div className="p-6 text-center text-gray-300">Loading ingredients...</div>}
        {!isLoading && ingredients.length === 0 && (
          <div className="p-6 text-center text-gray-300">No ingredients found.</div>
        )}
      </div>

      <div className="flex items-center justify-end gap-3">
        <button
          onClick={() => onPageChange(Math.max(1, currentPage - 1))}
          disabled={currentPage <= 1}
          className="px-4 py-2 rounded-lg bg-slate-700 text-white disabled:opacity-50"
        >
          Previous
        </button>
        <span className="text-sm text-gray-300">
          Page {currentPage} / {Math.max(totalPages, 1)}
        </span>
        <button
          onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
          disabled={currentPage >= totalPages}
          className="px-4 py-2 rounded-lg bg-slate-700 text-white disabled:opacity-50"
        >
          Next
        </button>
      </div>
    </div>
  );
};
