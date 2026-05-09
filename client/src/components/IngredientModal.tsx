import { useState } from 'react';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';
import { IngredientForm } from './IngredientForm';

interface IngredientModalProps {
  isOpen: boolean;
  ingredient?: Ingredient;
  onClose: () => void;
  onSave: (ingredient: IngredientCreateDto | IngredientUpdateDto) => Promise<void>;
}

export function IngredientModal({ isOpen, ingredient, onClose, onSave }: IngredientModalProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  if (!isOpen) return null;

  const handleSubmit = async (data: IngredientCreateDto | IngredientUpdateDto) => {
    setIsLoading(true);
    setError(null);
    setSuccess(null);

    try {
      await onSave(data);
      setSuccess(ingredient ? 'Ingredient updated successfully!' : 'Ingredient created successfully!');
      setTimeout(() => {
        onClose();
      }, 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-md">
        <h2 className="text-2xl font-bold mb-4">
          {ingredient ? 'Edit Ingredient' : 'New Ingredient'}
        </h2>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        {success && (
          <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4">
            {success}
          </div>
        )}

        <IngredientForm
          ingredient={ingredient}
          onSubmit={handleSubmit}
          onCancel={onClose}
          isLoading={isLoading}
        />
      </div>
    </div>
  );
}
