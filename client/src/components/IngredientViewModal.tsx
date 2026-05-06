import React from 'react';
import { X } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientViewModalProps {
  isOpen: boolean;
  ingredient: Ingredient | null;
  onClose: () => void;
}

export const IngredientViewModal: React.FC<IngredientViewModalProps> = ({ isOpen, ingredient, onClose }) => {
  if (!isOpen || !ingredient) {
    return null;
  }

  const rows: { label: string; value: string | number }[] = [
    { label: 'ID', value: ingredient.id },
    { label: 'Name', value: ingredient.name },
    { label: 'Unit', value: ingredient.unit },
    { label: 'Calories per unit', value: ingredient.caloriesPerUnit },
    { label: 'Protein', value: ingredient.protein },
    { label: 'Carbs', value: ingredient.carbs },
    { label: 'Fat', value: ingredient.fat },
  ];

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4"
      role="dialog"
      aria-modal="true"
      aria-labelledby="ingredient-view-title"
      onClick={onClose}
    >
      <div className="w-full max-w-md rounded-2xl border border-white/20 bg-gradient-to-br from-slate-800 to-slate-900 shadow-2xl overflow-hidden" onClick={(e) => e.stopPropagation()}>
        <div className="flex items-center justify-between bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-4">
          <h2 id="ingredient-view-title" className="text-lg font-bold text-white">
            Ingredient details
          </h2>
          <button type="button" onClick={onClose} className="text-white/90 hover:text-white transition-colors" aria-label="Close">
            <X size={22} />
          </button>
        </div>
        <div className="px-6 py-5 space-y-3">
          {rows.map(({ label, value }) => (
            <div key={label} className="flex justify-between gap-4 border-b border-white/10 pb-2 last:border-0 last:pb-0">
              <span className="text-sm font-medium text-gray-400">{label}</span>
              <span className="text-sm text-white text-right break-all">{value}</span>
            </div>
          ))}
        </div>
        <div className="px-6 pb-6">
          <button
            type="button"
            onClick={onClose}
            className="w-full rounded-lg border border-white/20 bg-white/10 py-2.5 text-sm font-semibold text-gray-200 hover:bg-white/15 transition-colors"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};
