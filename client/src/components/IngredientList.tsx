import React from 'react';
import { Eye, Pencil, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types';

interface IngredientListProps {
  ingredients: Ingredient[];
  onView: (ingredient: Ingredient) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (ingredient: Ingredient) => void;
  isLoading?: boolean;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  onView,
  onEdit,
  onDelete,
  isLoading = false,
}) => {
  if (ingredients.length === 0) {
    return (
      <div className="bg-white/5 border border-white/10 rounded-xl p-8 text-center">
        <p className="text-gray-300">No ingredients found.</p>
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-xl border border-white/10">
      <table className="w-full text-left">
        <thead className="bg-white/5 text-gray-300">
          <tr>
            <th className="px-6 py-4 font-semibold">Name</th>
            <th className="px-6 py-4 font-semibold">Unit</th>
            <th className="px-6 py-4 font-semibold">Calories</th>
            <th className="px-6 py-4 font-semibold">Protein</th>
            <th className="px-6 py-4 font-semibold">Carbs</th>
            <th className="px-6 py-4 font-semibold">Fat</th>
            <th className="px-6 py-4 font-semibold text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-white/10">
          {ingredients.map((ingredient) => (
            <tr key={ingredient.id} className="hover:bg-white/5">
              <td className="px-6 py-4 text-white font-medium">{ingredient.name}</td>
              <td className="px-6 py-4 text-gray-300">{ingredient.unit}</td>
              <td className="px-6 py-4 text-gray-300">{ingredient.caloriesPerUnit}</td>
              <td className="px-6 py-4 text-gray-300">{ingredient.protein}</td>
              <td className="px-6 py-4 text-gray-300">{ingredient.carbs}</td>
              <td className="px-6 py-4 text-gray-300">{ingredient.fat}</td>
              <td className="px-6 py-4">
                <div className="flex justify-end gap-2">
                  <button
                    onClick={() => onView(ingredient)}
                    className="inline-flex items-center gap-2 px-3 py-2 text-sm rounded-lg bg-slate-700/40 text-slate-200 hover:bg-slate-600/60 transition-colors"
                    disabled={isLoading}
                  >
                    <Eye size={16} />
                    View
                  </button>
                  <button
                    onClick={() => onEdit(ingredient)}
                    className="inline-flex items-center gap-2 px-3 py-2 text-sm rounded-lg bg-blue-600/30 text-blue-200 hover:bg-blue-600/50 transition-colors"
                    disabled={isLoading}
                  >
                    <Pencil size={16} />
                    Edit
                  </button>
                  <button
                    onClick={() => onDelete(ingredient)}
                    className="inline-flex items-center gap-2 px-3 py-2 text-sm rounded-lg bg-red-600/30 text-red-200 hover:bg-red-600/50 transition-colors"
                    disabled={isLoading}
                  >
                    <Trash2 size={16} />
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
