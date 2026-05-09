import React, { useState } from 'react';
import { X, CheckCircle, AlertCircle } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  initialData?: Ingredient | null;
  isLoading?: boolean;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
}) => {
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setErrorMessage(null);
      setSuccessMessage(null);
      await onSubmit(data);
      setSuccessMessage(initialData ? 'Ingredient updated!' : 'Ingredient created!');
      setTimeout(() => {
        setSuccessMessage(null);
        onClose();
      }, 1000);
    } catch (err: any) {
      setErrorMessage(err.response?.data?.message || 'Failed to save ingredient');
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-md w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Ingredient' : 'Add New Ingredient'}
          </h2>
          <button
            onClick={onClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-8">
          {successMessage && (
            <div className="mb-4 p-3 bg-green-500/20 border border-green-500/50 rounded-lg flex items-center space-x-2">
              <CheckCircle size={16} className="text-green-400" />
              <p className="text-green-200 text-sm">{successMessage}</p>
            </div>
          )}

          {errorMessage && (
            <div className="mb-4 p-3 bg-red-500/20 border border-red-500/50 rounded-lg flex items-center space-x-2">
              <AlertCircle size={16} className="text-red-400" />
              <p className="text-red-200 text-sm">{errorMessage}</p>
            </div>
          )}

          <IngredientForm
            onSubmit={handleSubmit}
            onCancel={onClose}
            initialData={
              initialData
                ? { name: initialData.name, quantity: initialData.quantity, foodId: initialData.foodId }
                : undefined
            }
            isLoading={isLoading}
          />
        </div>
      </div>
    </div>
  );
};
