import React, { useState } from 'react';
import { X } from 'lucide-react';
import { IngredientForm, type IngredientFormMode } from './IngredientForm';
import type { Ingredient, IngredientFormValues } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  mode: IngredientFormMode;
  ingredient?: Ingredient | null;
  onClose: () => void;
  onSave: (data: IngredientFormValues) => Promise<void>;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  mode,
  ingredient,
  onClose,
  onSave,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [banner, setBanner] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  if (!isOpen) return null;

  const title =
    mode === 'create' ? 'New ingredient' : mode === 'edit' ? 'Edit ingredient' : 'Ingredient';

  const defaults: Partial<IngredientFormValues> | undefined = ingredient
    ? {
        name: ingredient.name,
        unit: ingredient.unit,
        caloriesPerUnit: ingredient.caloriesPerUnit,
        protein: ingredient.protein,
        carbs: ingredient.carbs,
        fat: ingredient.fat,
        foodEntityId: ingredient.foodEntityId ?? null,
      }
    : undefined;

  const handleSubmit = async (data: IngredientFormValues) => {
    if (mode === 'view') return;
    try {
      setIsLoading(true);
      setBanner(null);
      await onSave(data);
      setBanner({ type: 'success', text: 'Saved successfully.' });
      setTimeout(() => {
        onClose();
      }, 600);
    } catch (e: unknown) {
      const msg = e instanceof Error ? e.message : 'Something went wrong.';
      setBanner({ type: 'error', text: msg });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl max-w-lg w-full max-h-[90vh] overflow-y-auto shadow-2xl">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-4 flex justify-between items-center rounded-t-2xl">
          <h2 className="text-lg font-bold text-white">{title}</h2>
          <button
            type="button"
            onClick={onClose}
            className="text-white/80 hover:text-white disabled:opacity-50"
            disabled={isLoading}
            aria-label="Close"
          >
            <X size={22} />
          </button>
        </div>

        <div className="p-6">
          {banner && (
            <div
              className={`mb-4 p-3 rounded-lg text-sm ${
                banner.type === 'success'
                  ? 'bg-emerald-500/20 text-emerald-200 border border-emerald-500/40'
                  : 'bg-red-500/20 text-red-200 border border-red-500/40'
              }`}
            >
              {banner.text}
            </div>
          )}

          <IngredientForm
            mode={mode}
            defaultValues={defaults}
            onSubmit={handleSubmit}
            onCancel={onClose}
            isSubmitting={isLoading}
          />
        </div>
      </div>
    </div>
  );
};
