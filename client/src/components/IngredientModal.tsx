import React, { useEffect, useState } from 'react';
import { X, CheckCircle2, AlertCircle } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSaved: (ingredient: Ingredient) => void;
  initialData?: Ingredient | null;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSaved,
  initialData,
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      setError(null);
      setSuccess(null);
    }
  }, [isOpen]);

  const handleSubmit = async (data: IngredientCreateDto) => {
    setIsSubmitting(true);
    setError(null);
    setSuccess(null);
    try {
      const saved = initialData
        ? await ingredientService.update(initialData.id, data)
        : await ingredientService.create(data);
      setSuccess(initialData ? 'Ingredient updated.' : 'Ingredient created.');
      onSaved(saved);
      // close after a short delay so the user sees the success state
      setTimeout(() => onClose(), 600);
    } catch (err: unknown) {
      const e = err as { response?: { data?: { message?: string } } };
      setError(e.response?.data?.message ?? 'Failed to save ingredient.');
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-lg w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-5 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>
          <button
            onClick={onClose}
            disabled={isSubmitting}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            aria-label="Close"
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-2">
              <AlertCircle size={18} className="text-red-400 flex-shrink-0 mt-0.5" />
              <p className="text-red-200 text-sm">{error}</p>
            </div>
          )}
          {success && (
            <div className="mb-4 p-3 bg-emerald-500/20 border border-emerald-500/50 rounded-lg flex items-start space-x-2">
              <CheckCircle2 size={18} className="text-emerald-400 flex-shrink-0 mt-0.5" />
              <p className="text-emerald-200 text-sm">{success}</p>
            </div>
          )}

          <IngredientForm
            initialData={initialData}
            onSubmit={handleSubmit}
            onCancel={onClose}
            isSubmitting={isSubmitting}
          />
        </div>
      </div>
    </div>
  );
};
