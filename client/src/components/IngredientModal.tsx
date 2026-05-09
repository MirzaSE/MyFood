import React, { useState } from 'react';
import { X, CheckCircle, AlertCircle } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: (ingredient: Ingredient) => void;
  initialData?: Ingredient | null;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  initialData,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  if (!isOpen) return null;

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsLoading(true);
      setErrorMessage(null);
      setSuccessMessage(null);

      let result: Ingredient;
      if (initialData) {
        result = await ingredientService.updateIngredient(initialData.id, data);
        setSuccessMessage('Ingredient updated successfully!');
      } else {
        result = await ingredientService.createIngredient(data);
        setSuccessMessage('Ingredient created successfully!');
      }

      onSuccess(result);
      setTimeout(() => onClose(), 800);
    } catch (err: any) {
      setErrorMessage(err.response?.data || 'Failed to save ingredient.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    setSuccessMessage(null);
    setErrorMessage(null);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-lg w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-5 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Ingredient' : 'Add New Ingredient'}
          </h2>
          <button
            onClick={handleClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          {successMessage && (
            <div className="mb-4 p-3 bg-green-500/20 border border-green-500/50 rounded-lg flex items-center space-x-2">
              <CheckCircle size={18} className="text-green-400 flex-shrink-0" />
              <p className="text-green-200 text-sm">{successMessage}</p>
            </div>
          )}
          {errorMessage && (
            <div className="mb-4 p-3 bg-red-500/20 border border-red-500/50 rounded-lg flex items-center space-x-2">
              <AlertCircle size={18} className="text-red-400 flex-shrink-0" />
              <p className="text-red-200 text-sm">{errorMessage}</p>
            </div>
          )}

          <IngredientForm
            initialData={initialData}
            onSubmit={handleSubmit}
            onCancel={handleClose}
            isLoading={isLoading}
          />
        </div>
      </div>
    </div>
  );
};
