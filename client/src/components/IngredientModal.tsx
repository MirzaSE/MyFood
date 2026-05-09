import React from 'react';
import { X } from 'lucide-react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';
import { IngredientForm } from './IngredientForm';

interface IngredientModalProps {
  isOpen: boolean;
  initialData?: Ingredient | null;
  isLoading?: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  initialData,
  isLoading = false,
  onClose,
  onSubmit,
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="bg-emerald-700 px-6 py-5 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>
          <button
            onClick={onClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
            aria-label="Close"
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          <IngredientForm
            initialData={initialData}
            isLoading={isLoading}
            onCancel={onClose}
            onSubmit={onSubmit}
          />
        </div>
      </div>
    </div>
  );
};
