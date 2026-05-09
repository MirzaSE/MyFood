import React, { useState } from 'react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface Props {
  isOpen: boolean;
  ingredient?: Ingredient | null;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
}

export const IngredientModal: React.FC<Props> = ({ isOpen, ingredient, onClose, onSubmit }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  if (!isOpen) return null;

  const handleSubmit = async (data: IngredientCreateDto) => {
    setIsLoading(true);
    setMessage(null);
    try {
      await onSubmit(data);
      setMessage({ type: 'success', text: ingredient ? 'Ingredient updated!' : 'Ingredient created!' });
      setTimeout(() => {
        onClose();
        setMessage(null);
      }, 1500);
    } catch (error) {
      setMessage({ type: 'error', text: error instanceof Error ? error.message : 'An error occurred' });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-lg bg-slate-800 rounded-xl border border-white/10 shadow-2xl p-6">
        <h2 className="text-xl font-bold text-white mb-4">
          {ingredient ? 'Edit Ingredient' : 'New Ingredient'}
        </h2>
        
        {message && (
          <div className={`mb-4 p-3 rounded-lg ${message.type === 'success' ? 'bg-green-500/20 text-green-300' : 'bg-red-500/20 text-red-300'}`}>
            {message.text}
          </div>
        )}
        
        <IngredientForm
          initialData={ingredient}
          onSubmit={handleSubmit}
          onCancel={onClose}
          isLoading={isLoading}
        />
      </div>
    </div>
  );
};