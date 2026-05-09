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
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg">
        <div className="flex items-center justify-between p-6 border-b">
          <h2 className="text-xl font-bold text-gray-800">
            {isEdit ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          {successMessage && (
            <div className="bg-green-50 border border-green-200 rounded p-3 mb-4">
              <p className="text-green-700 text-sm">{successMessage}</p>
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
