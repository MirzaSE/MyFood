import React, { useState } from 'react';
import { X } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface Props {
  ingredient?: Ingredient;
  onClose: () => void;
  onSaved: () => void;
}

export const IngredientModal: React.FC<Props> = ({ ingredient, onClose, onSaved }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const [successMessage, setSuccessMessage] = useState('');

  const isEdit = !!ingredient;

  const handleSubmit = async (data: IngredientCreateDto) => {
    setIsLoading(true);
    setErrors([]);
    setSuccessMessage('');

    try {
      if (isEdit) {
        await ingredientService.update(ingredient.id, data);
        setSuccessMessage('Ingredient updated successfully!');
      } else {
        await ingredientService.create(data);
        setSuccessMessage('Ingredient created successfully!');
      }
      onSaved();
      setTimeout(onClose, 800);
    } catch (err: any) {
      const msg = err?.response?.data;
      setErrors([typeof msg === 'string' ? msg : 'An error occurred.']);
    } finally {
      setIsLoading(false);
    }
  };

  const defaultValues = ingredient
    ? {
        name: ingredient.name,
        unit: ingredient.unit,
        caloriesPerUnit: ingredient.caloriesPerUnit,
        protein: ingredient.protein,
        carbs: ingredient.carbs,
        fat: ingredient.fat,
        quantity: ingredient.quantity,
      }
    : undefined;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-5 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {isEdit ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>
          <button onClick={onClose} className="text-white/80 hover:text-white transition-colors" disabled={isLoading}>
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          {successMessage && (
            <div className="bg-green-500/20 border border-green-500/50 rounded-lg p-3 mb-4">
              <p className="text-green-300 text-sm">{successMessage}</p>
            </div>
          )}
          <IngredientForm
            defaultValues={defaultValues}
            onSubmit={handleSubmit}
            onCancel={onClose}
            isLoading={isLoading}
            errors={errors}
          />
        </div>
      </div>
    </div>
  );
};
