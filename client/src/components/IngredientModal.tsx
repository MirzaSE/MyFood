import React, { useState } from 'react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';
import { IngredientForm } from './IngredientForm';
import { ingredientService } from '..//services/ingredientService';

interface Props {
  onClose: () => void;
  onSuccess: () => void;
  existing?: Ingredient;
}

export const IngredientModal: React.FC<Props> = ({ onClose, onSuccess, existing }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (dto: IngredientCreateDto) => {
    setLoading(true);
    setError('');
    setSuccess('');
    try {
      if (existing) {
        await ingredientService.update(existing.id, dto);
        setSuccess('Ingredient updated successfully!');
      } else {
        await ingredientService.create(dto);
        setSuccess('Ingredient created successfully!');
      }
      setTimeout(() => {
        onSuccess();
        onClose();
      }, 800);
    } catch {
      setError('Something went wrong. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-slate-900 border border-white/10 rounded-2xl p-6 w-full max-w-md">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-white">
            {existing ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-white text-2xl leading-none">&times;</button>
        </div>

        {error && <p className="text-red-400 text-sm mb-3 bg-red-500/10 border border-red-500/20 rounded-lg px-3 py-2">{error}</p>}
        {success && <p className="text-green-400 text-sm mb-3 bg-green-500/10 border border-green-500/20 rounded-lg px-3 py-2">{success}</p>}

        {loading ? (
          <div className="text-center py-8 text-gray-400">Saving...</div>
        ) : (
          <IngredientForm onSubmit={handleSubmit} onCancel={onClose} existing={existing} />
        )}
      </div>
    </div>
  );
};