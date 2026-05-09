import React from 'react';
import { Edit, PackageSearch, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading = false,
  onEdit,
  onDelete,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);

  const handleDelete = async (id: number) => {
    setDeletingId(id);
    try {
      await onDelete(id);
    } finally {
      setDeletingId(null);
    }
  };

  if (ingredients.length === 0) {
    return (
      <div className="text-center py-16 border border-white/10 rounded-lg bg-white/5">
        <div className="flex justify-center mb-4">
          <div className="p-4 bg-emerald-500/20 rounded-full">
            <PackageSearch size={32} className="text-emerald-300" />
          </div>
        </div>
        <p className="text-gray-200 text-lg font-medium">No ingredients found</p>
        <p className="text-gray-400 text-sm mt-1">Create one to start building recipes.</p>
      </div>
    );
  }

  return (
    <div className="overflow-hidden border border-white/10 rounded-lg bg-slate-900/70">
      <div className="overflow-x-auto">
        <table className="w-full min-w-[920px] text-left">
          <thead className="bg-white/5 text-gray-300 text-sm">
            <tr>
              <th className="px-5 py-4 font-semibold">Name</th>
              <th className="px-5 py-4 font-semibold">Unit</th>
              <th className="px-5 py-4 font-semibold">Stock</th>
              <th className="px-5 py-4 font-semibold">Calories</th>
              <th className="px-5 py-4 font-semibold">Protein</th>
              <th className="px-5 py-4 font-semibold">Carbs</th>
              <th className="px-5 py-4 font-semibold">Fat</th>
              <th className="px-5 py-4 font-semibold text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/10">
            {ingredients.map((ingredient) => (
              <tr key={ingredient.id} className="hover:bg-white/[0.03] transition-colors">
                <td className="px-5 py-4 text-white font-medium">{ingredient.name}</td>
                <td className="px-5 py-4 text-gray-300">{ingredient.unit}</td>
                <td className="px-5 py-4 text-gray-300">{ingredient.quantity}</td>
                <td className="px-5 py-4 text-gray-300">{ingredient.caloriesPerUnit}</td>
                <td className="px-5 py-4 text-gray-300">{ingredient.protein}g</td>
                <td className="px-5 py-4 text-gray-300">{ingredient.carbs}g</td>
                <td className="px-5 py-4 text-gray-300">{ingredient.fat}g</td>
                <td className="px-5 py-4">
                  <div className="flex justify-end gap-2">
                    <button
                      onClick={() => onEdit(ingredient)}
                      disabled={isLoading || deletingId !== null}
                      className="inline-flex items-center justify-center w-10 h-10 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 rounded-lg border border-blue-500/30 disabled:opacity-50"
                      title="Edit ingredient"
                      aria-label="Edit ingredient"
                    >
                      <Edit size={16} />
                    </button>
                    <button
                      onClick={() => handleDelete(ingredient.id)}
                      disabled={isLoading || deletingId !== null}
                      className="inline-flex items-center justify-center w-10 h-10 bg-red-500/20 hover:bg-red-500/30 text-red-300 rounded-lg border border-red-500/30 disabled:opacity-50"
                      title="Delete ingredient"
                      aria-label="Delete ingredient"
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
    </div>
  );
};
