import React from 'react';
import { X } from 'lucide-react';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';
import { IngredientForm } from './IngredientForm';

interface IngredientModalProps {
  isOpen: boolean;
  initialData?: Ingredient | null;
  isLoading?: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto | IngredientUpdateDto) => Promise<void>;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({ isOpen, initialData, isLoading = false, onClose, onSubmit }) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4 backdrop-blur-sm">
      <div className="w-full max-w-2xl overflow-hidden rounded-2xl border border-white/10 bg-slate-900 shadow-2xl">
        <div className="flex items-center justify-between border-b border-white/10 bg-slate-950/60 px-6 py-4">
          <div>
            <h2 className="text-xl font-bold text-white">{initialData ? 'Edit Ingredient' : 'Add Ingredient'}</h2>
            <p className="text-sm text-gray-400">Keep your pantry data in sync.</p>
          </div>
          <button onClick={onClose} disabled={isLoading} className="rounded-full p-2 text-gray-300 hover:bg-white/10 hover:text-white">
            <X size={20} />
          </button>
        </div>
        <IngredientForm initialData={initialData} onSubmit={onSubmit} onCancel={onClose} isLoading={isLoading} />
      </div>
    </div>
  );
};
