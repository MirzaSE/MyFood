import React from 'react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  initialData?: Ingredient | null;
  isLoading?: boolean;
  statusMessage?: string | null;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({ isOpen, initialData, isLoading = false, statusMessage, onClose, onSubmit }) => {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4">
      <div className="w-full max-w-lg rounded-xl bg-slate-900 border border-slate-700 p-6">
        <h2 className="text-xl font-bold text-white mb-4">{initialData ? 'Edit Ingredient' : 'New Ingredient'}</h2>
        {statusMessage && <p className="text-sm text-blue-300 mb-3">{statusMessage}</p>}
        <IngredientForm initialData={initialData} onSubmit={onSubmit} onCancel={onClose} isLoading={isLoading} />
      </div>
    </div>
  );
};
