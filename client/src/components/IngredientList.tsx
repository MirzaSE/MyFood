import React from 'react';
import { Edit, Trash2, Sparkles } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  isLoading?: boolean;
}

export const IngredientList: React.FC<IngredientListProps> = ({ ingredients, onEdit, onDelete, isLoading = false }) => {
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
      <div className="rounded-2xl border border-white/10 bg-white/5 p-10 text-center text-gray-300">
        <Sparkles className="mx-auto mb-3 text-cyan-300" />
        <p className="text-lg font-semibold text-white">No ingredients yet</p>
        <p className="mt-1 text-sm text-gray-400">Add one to build recipes and meals.</p>
      </div>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
      {ingredients.map((ingredient) => (
        <article key={ingredient.id} className="rounded-2xl border border-white/10 bg-slate-900/80 p-5 shadow-lg shadow-cyan-950/10">
          <div className="flex items-start justify-between gap-3">
            <div>
              <h3 className="text-lg font-bold text-white">{ingredient.name}</h3>
              <p className="text-sm text-cyan-200/80">{ingredient.unit || 'unit not set'}</p>
            </div>
            <span className="rounded-full border border-cyan-400/30 bg-cyan-400/10 px-3 py-1 text-xs font-semibold text-cyan-200">
              {ingredient.caloriesPerUnit ?? 0} kcal
            </span>
          </div>
          <dl className="mt-4 grid grid-cols-2 gap-3 text-sm text-gray-300">
            <div className="rounded-xl bg-white/5 p-3"><dt className="text-gray-400">Protein</dt><dd className="text-white">{ingredient.protein ?? 0}</dd></div>
            <div className="rounded-xl bg-white/5 p-3"><dt className="text-gray-400">Carbs</dt><dd className="text-white">{ingredient.carbs ?? 0}</dd></div>
            <div className="rounded-xl bg-white/5 p-3"><dt className="text-gray-400">Fat</dt><dd className="text-white">{ingredient.fat ?? 0}</dd></div>
            <div className="rounded-xl bg-white/5 p-3"><dt className="text-gray-400">Food</dt><dd className="text-white">{ingredient.foodEntityId ?? '-'}</dd></div>
          </dl>
          <div className="mt-4 flex gap-2">
            <button onClick={() => onEdit(ingredient)} disabled={isLoading || deletingId === ingredient.id} className="flex-1 rounded-lg border border-blue-400/30 bg-blue-500/10 px-3 py-2 text-sm font-medium text-blue-200 hover:bg-blue-500/20"><span className="inline-flex items-center gap-2"><Edit size={16} />Edit</span></button>
            <button onClick={() => handleDelete(ingredient.id)} disabled={isLoading || deletingId === ingredient.id} className="flex-1 rounded-lg border border-red-400/30 bg-red-500/10 px-3 py-2 text-sm font-medium text-red-200 hover:bg-red-500/20"><span className="inline-flex items-center gap-2"><Trash2 size={16} />{deletingId === ingredient.id ? 'Deleting' : 'Delete'}</span></button>
          </div>
        </article>
      ))}
    </div>
  );
};
