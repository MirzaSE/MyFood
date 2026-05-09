import React from 'react';
import { X, AlertCircle, CheckCircle2 } from 'lucide-react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';
import { IngredientForm } from './IngredientForm';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  initialData?: Ingredient | null;
  isLoading?: boolean;
  errorMessage?: string | null;
  successMessage?: string | null;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
  errorMessage,
  successMessage,
}) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-lg w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Ingredient' : 'Add New Ingredient'}
          </h2>
          <button
            onClick={onClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        {errorMessage && (
          <div className="mx-6 mt-6 p-3 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-2">
            <AlertCircle size={18} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200 text-sm">{errorMessage}</p>
          </div>
        )}
        {successMessage && (
          <div className="mx-6 mt-6 p-3 bg-emerald-500/20 border border-emerald-500/50 rounded-lg flex items-start space-x-2">
            <CheckCircle2 size={18} className="text-emerald-400 flex-shrink-0 mt-0.5" />
            <p className="text-emerald-200 text-sm">{successMessage}</p>
          </div>
        )}

        <IngredientForm
          initialData={initialData}
          onSubmit={onSubmit}
          onCancel={onClose}
          isLoading={isLoading}
        />
      </div>
    </div>
  );
};
