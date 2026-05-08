import React, { useState } from 'react';
import { X } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import type { Food, Ingredient, IngredientCreateDto } from '../types';

interface IngredientModalProps {
  isOpen: boolean;
  foods: Food[];
  initialData?: Ingredient | null;
  onClose: () => void;
  onSubmit: (dto: IngredientCreateDto) => Promise<void>;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  foods,
  initialData,
  onClose,
  onSubmit,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  if (!isOpen) return null;

  const handleSubmit = async (dto: IngredientCreateDto) => {
    try {
      setIsLoading(true);
      setError(null);
      setMessage(null);
      await onSubmit(dto);
      const successText = initialData ? 'Ingredient updated successfully.' : 'Successful';
      setMessage(successText);
      window.alert(successText);
      onClose();
    } catch {
      setError('Failed to save ingredient.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4">
      <div className="w-full max-w-xl bg-slate-900 border border-white/20 rounded-xl p-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold text-white">{initialData ? 'Edit Ingredient' : 'New Ingredient'}</h2>
          <button onClick={onClose} className="text-gray-300 hover:text-white">
            <X size={20} />
          </button>
        </div>

        {message && <div className="mb-3 text-sm text-emerald-300">{message}</div>}
        {error && <div className="mb-3 text-sm text-red-300">{error}</div>}

        <IngredientForm
          foods={foods}
          initialData={initialData}
          isLoading={isLoading}
          onSubmit={handleSubmit}
          onCancel={onClose}
        />
      </div>
    </div>
  );
};
