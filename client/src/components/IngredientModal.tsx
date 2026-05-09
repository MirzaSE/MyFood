import React, { useState } from 'react';
import { X, AlertCircle, CheckCircle } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  ingredient?: Ingredient | null;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  isSubmitting?: boolean;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  ingredient,
  onClose,
  onSubmit,
  isSubmitting = false,
}) => {
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setError(null);
      setSuccess(null);
      await onSubmit(data);
      setSuccess(ingredient ? 'Ingredient updated successfully' : 'Ingredient created successfully');
      setTimeout(() => {
        onClose();
        setSuccess(null);
      }, 1000);
    } catch (err: any) {
      setError(err.message || 'An error occurred');
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-gray-800 rounded-lg shadow-xl max-w-md w-full">
        {/* Modal Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-700">
          <h2 className="text-xl font-bold text-white">
            {ingredient ? 'Edit Ingredient' : 'Create Ingredient'}
          </h2>
          <button
            onClick={onClose}
            disabled={isSubmitting}
            className="text-gray-400 hover:text-gray-200 transition"
          >
            <X size={24} />
          </button>
        </div>

        {/* Modal Body */}
        <div className="p-6">
          {/* Success Message */}
          {success && (
            <div className="mb-4 p-4 bg-green-500/20 border border-green-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
              <CheckCircle size={20} className="text-green-400 flex-shrink-0 mt-0.5" />
              <p className="text-green-200">{success}</p>
            </div>
          )}

          {/* Error Message */}
          {error && (
            <div className="mb-4 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
              <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
              <p className="text-red-200">{error}</p>
            </div>
          )}

          {/* Form */}
          <IngredientForm
            ingredient={ingredient}
            onSubmit={handleSubmit}
            onCancel={onClose}
            isSubmitting={isSubmitting}
          />
        </div>
      </div>
    </div>
  );
};
