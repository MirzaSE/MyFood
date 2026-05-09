import React, { useState } from 'react';
import { X, AlertCircle, CheckCircle2 } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  initialData?: Ingredient | null;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  if (!isOpen) return null;

  const handleClose = () => {
    setError(null);
    setSuccess(null);
    onClose();
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsLoading(true);
      setError(null);
      setSuccess(null);
      await onSubmit(data);
      setSuccess(initialData ? 'Ingredient updated.' : 'Ingredient created.');
      // Brief delay so users see the success state, then close.
      setTimeout(() => {
        setSuccess(null);
        handleClose();
      }, 700);
    } catch (err: any) {
      setError(err?.response?.data?.message || err?.message || 'Failed to save ingredient.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-md w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-5 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>
          <button
            onClick={handleClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
            aria-label="Close"
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-2">
              <AlertCircle size={18} className="text-red-400 flex-shrink-0 mt-0.5" />
              <p className="text-red-200 text-sm">{error}</p>
            </div>
          )}
          {success && (
            <div className="mb-4 p-3 bg-emerald-500/20 border border-emerald-500/50 rounded-lg flex items-start space-x-2">
              <CheckCircle2 size={18} className="text-emerald-400 flex-shrink-0 mt-0.5" />
              <p className="text-emerald-200 text-sm">{success}</p>
            </div>
          )}

          <IngredientForm
            initialData={initialData}
            onSubmit={handleSubmit}
            onCancel={handleClose}
            isLoading={isLoading}
          />
        </div>
      </div>
    </div>
  );
};
