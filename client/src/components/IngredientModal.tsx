import React from 'react';
import { X } from 'lucide-react';
import type { Ingredient, IngredientCreateDto } from '../types';
import { IngredientForm } from './IngredientForm';

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
  if (!isOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-4 backdrop-blur-sm">
      <div className="w-full max-w-xl rounded-3xl border border-white/10 bg-gradient-to-br from-slate-900 via-slate-800 to-stone-900 shadow-2xl">
        <div className="flex items-center justify-between border-b border-white/10 px-6 py-5">
          <div>
            <h2 className="text-2xl font-bold text-white">{initialData ? 'Edit Ingredient' : 'New Ingredient'}</h2>
            <p className="text-sm text-gray-400">Define the ingredient once and reuse it across foods.</p>
          </div>
          <button onClick={onClose} className="rounded-full p-2 text-gray-300 transition hover:bg-white/10 hover:text-white">
            <X size={20} />
          </button>
        </div>

        <div className="space-y-4 p-6">
          {message && <div className="rounded-lg border border-emerald-500/30 bg-emerald-500/10 p-3 text-sm text-emerald-100">{message}</div>}
          {error && <div className="rounded-lg border border-red-500/30 bg-red-500/10 p-3 text-sm text-red-100">{error}</div>}

          <IngredientForm initialData={initialData} onSubmit={onSubmit} onCancel={onClose} isLoading={isLoading} />
        </div>
      </div>
    </div>
  );
};
