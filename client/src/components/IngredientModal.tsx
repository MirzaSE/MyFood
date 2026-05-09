import React, { useState } from 'react';
import { X } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  editingIngredient?: Ingredient | null;
}

export const IngredientModal: React.FC<Props> = ({ isOpen, onClose, onSuccess, editingIngredient }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  if (!isOpen) return null;

  const handleSubmit = async (data: IngredientCreateDto) => {
    setIsLoading(true);
    setError('');
    setSuccess('');
    try {
      if (editingIngredient) {
        await ingredientService.update(editingIngredient.id, data);
        setSuccess('Ingredient updated!');
      } else {
        await ingredientService.create(data);
        setSuccess('Ingredient created!');
      }
      setTimeout(() => {
        onSuccess();
        onClose();
      }, 800);
    } catch {
      setError('Something went wrong. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4">
      <div className="bg-slate-800 rounded-2xl w-full max-w-md border border-white/10">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 p-5 rounded-t-2xl flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {editingIngredient ? 'Edit Ingredient' : 'Add Ingredient'}
          </h2>
          <button onClick={onClose} className="text-white hover:text-gray-200">
            <X size={22} />
          </button>
        </div>

        <div className="p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-500/20 border border-red-500/40 rounded-lg text-red-300 text-sm">
              {error}
            </div>
          )}
          {success && (
            <div className="mb-4 p-3 bg-green-500/20 border border-green-500/40 rounded-lg text-green-300 text-sm">
              {success}
            </div>
          )}

          <IngredientForm
            initialData={editingIngredient ?? undefined}
            onSubmit={handleSubmit}
            onCancel={onClose}
            isLoading={isLoading}
          />
        </div>
      </div>
    </div>
  );
};