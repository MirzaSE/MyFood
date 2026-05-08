import React from 'react';
import { X } from 'lucide-react';
import type { Ingredient, IngredientCreateDto } from '../types';
import { IngredientForm } from './IngredientForm';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  initialData?: Ingredient | null;
  title: string;
  isLoading?: boolean;
  readOnly?: boolean;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  title,
  isLoading = false,
  readOnly = false,
}) => {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-slate-900 border border-white/10 rounded-xl shadow-2xl w-full max-w-2xl relative">
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-white transition-colors"
        >
          <X size={20} />
        </button>

        <div className="p-6">
          <div className="mb-6">
            <h2 className="text-2xl font-bold text-white">{title}</h2>
            <p className="text-gray-400">{readOnly ? 'Ingredient details' : 'Enter ingredient information.'}</p>
          </div>

          <IngredientForm
            initialData={initialData}
            onSubmit={onSubmit}
            onCancel={onClose}
            isLoading={isLoading}
            readOnly={readOnly}
          />
        </div>
      </div>
    </div>
  );
};
