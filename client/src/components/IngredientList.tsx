import React from 'react';
import type { Ingredient } from '../types/ingredient';

interface Props {
  ingredients: Ingredient[];
  isLoading: boolean;
  currentPage: number;
  totalPages: number;
  searchQuery: string;
  onSearchChange: (query: string) => void;
  onPageChange: (page: number) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (ingredient: Ingredient) => void;
  onView: (ingredient: Ingredient) => void;
}

export const IngredientList: React.FC<Props> = ({
  ingredients,
  isLoading,
  currentPage,
  totalPages,
  searchQuery,
  onSearchChange,
  onPageChange,
  onEdit,
  onDelete,
  onView,
}) => {
  return (
    <div className="space-y-4">
      <div className="flex items-center gap-2">
        <input
          type="text"
          value={searchQuery}
          onChange={(e) => onSearchChange(e.target.value)}
          placeholder="Search ingredients..."
          className="flex-1 bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
        />
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center py-12">
          <div className="w-8 h-8 border-4 border-purple-500 border-t-transparent rounded-full animate-spin"></div>
        </div>
      ) : ingredients.length === 0 ? (
        <div className="text-center py-12 text-gray-400">
          {searchQuery ? 'No ingredients found matching your search.' : 'No ingredients yet. Create one!'}
        </div>
      ) : (
        <>
          <div className="bg-slate-800/50 rounded-xl border border-white/10 overflow-hidden">
            <table className="w-full">
              <thead className="bg-slate-700/30">
                <tr>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-gray-300">Name</th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-gray-300">Unit</th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-gray-300">Calories</th>
                  <th className="px-4 py-3 text-left text-sm font-semibold text-gray-300">P / C / F</th>
                  <th className="px-4 py-3 text-right text-sm font-semibold text-gray-300">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-white/5">
                {ingredients.map((ingredient) => (
                  <tr key={ingredient.id} className="hover:bg-white/5">
                    <td className="px-4 py-3 text-white">{ingredient.name}</td>
                    <td className="px-4 py-3 text-gray-400">{ingredient.unit}</td>
                    <td className="px-4 py-3 text-gray-400">{ingredient.caloriesPerUnit}</td>
                    <td className="px-4 py-3 text-gray-400">
                      {ingredient.protein} / {ingredient.carbs} / {ingredient.fat}
                    </td>
                    <td className="px-4 py-3 text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => onView(ingredient)}
                          className="px-3 py-1.5 text-xs text-gray-400 hover:text-white hover:bg-white/10 rounded-lg transition-colors"
                        >
                          View
                        </button>
                        <button
                          onClick={() => onEdit(ingredient)}
                          className="px-3 py-1.5 text-xs text-blue-400 hover:text-blue-300 hover:bg-blue-500/20 rounded-lg transition-colors"
                        >
                          Edit
                        </button>
                        <button
                          onClick={() => onDelete(ingredient)}
                          className="px-3 py-1.5 text-xs text-red-400 hover:text-red-300 hover:bg-red-500/20 rounded-lg transition-colors"
                        >
                          Delete
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {totalPages > 1 && (
            <div className="flex items-center justify-center gap-2">
              <button
                onClick={() => onPageChange(currentPage - 1)}
                disabled={currentPage === 1}
                className="px-3 py-1.5 text-sm text-gray-300 bg-slate-700/60 rounded-lg hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Previous
              </button>
              <span className="text-gray-400 text-sm">
                Page {currentPage} of {totalPages}
              </span>
              <button
                onClick={() => onPageChange(currentPage + 1)}
                disabled={currentPage === totalPages}
                className="px-3 py-1.5 text-sm text-gray-300 bg-slate-700/60 rounded-lg hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Next
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
};