import React from 'react';
import { Edit, Trash2, UtensilsCrossed } from 'lucide-react';
import type { Food } from '../types';

interface FoodTableProps {
  foods: Food[];
  onEdit: (food: Food) => void;
  onDelete: (id: number) => Promise<void>;
  isLoading?: boolean;
}

export const FoodTable: React.FC<FoodTableProps> = ({
  foods,
  onEdit,
  onDelete,
  isLoading = false,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);

  const handleDelete = async (id: number) => {
    try {
      setDeletingId(id);
      await onDelete(id);
    } finally {
      setDeletingId(null);
    }
  };

  if (foods.length === 0) {
    return (
      <div className="py-16 text-center">
        <div className="mb-4 flex justify-center">
          <div className="rounded-full bg-amber-500/20 p-4">
            <UtensilsCrossed size={32} className="text-amber-300" />
          </div>
        </div>
        <p className="text-lg font-medium text-gray-300">No foods found</p>
        <p className="mt-1 text-sm text-gray-400">Create one to get started.</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
      {foods.map((food) => (
        <div
          key={food.id}
          className="rounded-[1.75rem] border border-white/10 bg-gradient-to-br from-slate-900/90 to-stone-900/80 p-6 shadow-xl"
        >
          <div className="mb-5 flex items-start justify-between gap-3">
            <div>
              <h3 className="text-2xl font-bold text-white">{food.name}</h3>
              <span className="mt-2 inline-block rounded-full bg-amber-500/20 px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-amber-200">
                {food.type}
              </span>
            </div>
            <div className="rounded-2xl bg-white/5 px-4 py-3 text-right">
              <p className="text-xs uppercase tracking-[0.2em] text-gray-400">Ingredients</p>
              <p className="text-2xl font-black text-white">{food.ingredients?.length ?? 0}</p>
            </div>
          </div>

          <div className="grid gap-3 md:grid-cols-2">
            <div className="rounded-2xl border border-white/5 bg-white/5 p-4">
              <p className="text-xs uppercase tracking-[0.18em] text-gray-400">Calories</p>
              <p className="mt-2 text-2xl font-bold text-white">{food.nutritionTotals?.calories?.toFixed?.(0) ?? food.calories} kcal</p>
            </div>
            <div className="rounded-2xl border border-white/5 bg-white/5 p-4">
              <p className="text-xs uppercase tracking-[0.18em] text-gray-400">Macros</p>
              <p className="mt-2 text-sm text-gray-200">
                P {food.nutritionTotals?.protein?.toFixed?.(1) ?? 0} / C {food.nutritionTotals?.carbs?.toFixed?.(1) ?? 0} / F {food.nutritionTotals?.fat?.toFixed?.(1) ?? 0}
              </p>
            </div>
          </div>

          {food.ingredients?.length > 0 && (
            <div className="mt-5 rounded-2xl border border-white/5 bg-black/20 p-4">
              <p className="mb-3 text-xs uppercase tracking-[0.2em] text-gray-400">Selected Ingredients</p>
              <div className="flex flex-wrap gap-2">
                {food.ingredients.map((ingredient) => (
                  <span key={`${food.id}-${ingredient.ingredientId}`} className="rounded-full border border-white/10 bg-white/5 px-3 py-1 text-xs text-gray-200">
                    {ingredient.name} x {ingredient.quantity}
                  </span>
                ))}
              </div>
            </div>
          )}

          <div className="mt-6 flex gap-3">
            <button
              onClick={() => onEdit(food)}
              className="flex flex-1 items-center justify-center gap-2 rounded-xl border border-blue-500/30 bg-blue-500/10 px-4 py-3 text-blue-100 transition hover:bg-blue-500/20"
              disabled={isLoading || deletingId === food.id}
            >
              <Edit size={16} />
              Edit
            </button>
            <button
              onClick={() => handleDelete(food.id)}
              className="flex flex-1 items-center justify-center gap-2 rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-red-100 transition hover:bg-red-500/20 disabled:opacity-50"
              disabled={isLoading || deletingId === food.id}
            >
              <Trash2 size={16} />
              {deletingId === food.id ? 'Deleting...' : 'Delete'}
            </button>
          </div>
        </div>
      ))}
    </div>
  );
};
