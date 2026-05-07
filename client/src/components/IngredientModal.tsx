import React, { useEffect, useState } from 'react';
import { X } from 'lucide-react';
import { getApiErrorMessage } from '../services/api';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';
import { IngredientForm } from './IngredientForm';

interface IngredientModalProps {
  isOpen: boolean;
  initialData?: Ingredient | null;
  onClose: () => void;
  onSaved: (ingredient: Ingredient) => void;
}

const toFormValues = (ingredient?: Ingredient | null): IngredientCreateDto => ({
  name: ingredient?.name ?? '',
  unit: ingredient?.unit ?? '',
  caloriesPerUnit: ingredient?.caloriesPerUnit ?? 0,
  protein: ingredient?.protein ?? 0,
  carbs: ingredient?.carbs ?? 0,
  fat: ingredient?.fat ?? 0,
});

export const IngredientModal: React.FC<IngredientModalProps> = ({ isOpen, initialData, onClose, onSaved }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      setMessage(null);
      setError(null);
    }
  }, [isOpen, initialData?.id]);

  if (!isOpen) return null;

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsLoading(true);
      setError(null);
      setMessage(null);
      const saved = initialData
        ? await ingredientService.updateIngredient(initialData.id, data)
        : await ingredientService.createIngredient(data);
      setMessage(initialData ? 'Ingredient updated.' : 'Ingredient created.');
      onSaved(saved);
      onClose();
    } catch (submitError: unknown) {
      setError(getApiErrorMessage(submitError, 'Failed to save ingredient'));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-950 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full overflow-hidden">
        <div className="bg-gradient-to-r from-emerald-500 to-cyan-500 px-6 py-5 flex justify-between items-center">
          <div>
            <h2 className="text-xl font-bold text-white">{initialData ? 'Edit Ingredient' : 'New Ingredient'}</h2>
            <p className="text-sm text-white/80">Reusable nutrition building block</p>
          </div>
          <button className="text-white/80 hover:text-white disabled:opacity-50" disabled={isLoading} onClick={onClose} type="button">
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          {message && <div className="mb-4 rounded-lg border border-emerald-400/40 bg-emerald-500/15 px-4 py-3 text-emerald-100">{message}</div>}
          {error && <div className="mb-4 rounded-lg border border-red-400/40 bg-red-500/15 px-4 py-3 text-red-100">{error}</div>}
          <IngredientForm
            initialData={toFormValues(initialData)}
            isLoading={isLoading}
            onCancel={onClose}
            onSubmit={handleSubmit}
          />
        </div>
      </div>
    </div>
  );
};
