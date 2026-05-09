import React from 'react';
import { X } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  initialData?: Ingredient | null;
  isLoading?: boolean;
  message?: string | null;
  error?: string | null;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
  message,
  error,
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-xl w-full max-h-[90vh] overflow-y-auto">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>

          <button
            type="button"
            onClick={onClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        {message && (
          <div className="mx-8 mt-6 p-3 bg-green-500/20 border border-green-500/40 rounded-lg text-green-200 text-sm">
            {message}
          </div>
        )}

        {error && (
          <div className="mx-8 mt-6 p-3 bg-red-500/20 border border-red-500/40 rounded-lg text-red-200 text-sm">
            {error}
          </div>
        )}

        <IngredientForm
          initialData={initialData}
          isLoading={isLoading}
          onSubmit={onSubmit}
          onCancel={onClose}
        />
      </div>
    </div>
  );
};