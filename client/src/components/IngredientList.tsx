import React from 'react';
import { Edit, Trash2, Eye, Search, PackageOpen } from 'lucide-react';
import type { Ingredient } from '../types';

interface IngredientListProps {
  ingredients: Ingredient[];
  searchTerm: string;
  currentPage: number;
  pageSize: number;
  isLoading?: boolean;
  onSearchChange: (value: string) => void;
  onPageChange: (page: number) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  onView: (ingredient: Ingredient) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  searchTerm,
  currentPage,
  pageSize,
  isLoading = false,
  onSearchChange,
  onPageChange,
  onEdit,
  onDelete,
  onView,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);

  const totalPages = Math.max(1, Math.ceil(ingredients.length / pageSize));
  const start = (currentPage - 1) * pageSize;
  const visibleIngredients = ingredients.slice(start, start + pageSize);

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
      <div className="relative max-w-md">
        <Search size={18} className="absolute left-3 top-3.5 text-gray-400" />
        <input
          value={searchTerm}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder="Search ingredients..."
          className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400"
        />
      </div>

      {isLoading ? (
        <div className="text-center py-16 text-gray-300">Loading ingredients...</div>
      ) : visibleIngredients.length === 0 ? (
        <div className="text-center py-16">
          <div className="flex justify-center mb-4">
            <div className="p-4 bg-purple-500/20 rounded-full">
              <PackageOpen size={32} className="text-purple-400" />
            </div>
          </div>
          <p className="text-gray-300 text-lg font-medium">No ingredients found</p>
          <p className="text-gray-400 text-sm mt-1">Create one to get started.</p>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-white/10">
          <table className="min-w-full bg-slate-900/60">
            <thead className="bg-white/10">
              <tr>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-300">Name</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-300">Unit</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-300">Calories</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-300">Actions</th>
              </tr>
            </thead>

            <tbody>
              {visibleIngredients.map((ingredient) => (
                <tr key={ingredient.id} className="border-t border-white/10 hover:bg-white/5">
                  <td className="px-6 py-4 text-white font-medium">{ingredient.name}</td>
                  <td className="px-6 py-4 text-gray-300">{ingredient.unit}</td>
                  <td className="px-6 py-4 text-gray-300">
                    {ingredient.caloriesPerUnit} kcal
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex flex-wrap gap-2">
                      <button
                        onClick={() => onView(ingredient)}
                        className="px-3 py-2 bg-slate-700 hover:bg-slate-600 text-gray-200 rounded-lg text-sm flex items-center gap-1"
                      >
                        <Eye size={15} /> View
                      </button>

                      <button
                        onClick={() => onEdit(ingredient)}
                        className="px-3 py-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 rounded-lg text-sm flex items-center gap-1"
                      >
                        <Edit size={15} /> Edit
                      </button>

                      <button
                        onClick={() => handleDelete(ingredient.id)}
                        disabled={deletingId === ingredient.id}
                        className="px-3 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 rounded-lg text-sm flex items-center gap-1 disabled:opacity-50"
                      >
                        <Trash2 size={15} />
                        {deletingId === ingredient.id ? 'Deleting...' : 'Delete'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="flex items-center justify-between text-gray-300">
        <button
          onClick={() => onPageChange(Math.max(1, currentPage - 1))}
          disabled={currentPage === 1}
          className="px-4 py-2 bg-white/10 rounded-lg disabled:opacity-40"
        >
          Previous
        </button>

        <span>
          Page {currentPage} of {totalPages}
        </span>

        <button
          onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
          disabled={currentPage === totalPages}
          className="px-4 py-2 bg-white/10 rounded-lg disabled:opacity-40"
        >
          Next
        </button>
      </div>
    </div>
  );
};