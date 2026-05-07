import React from 'react';
import { X } from 'lucide-react';
import type { Ingredient, IngredientFormData } from '../types';
import { IngredientForm } from './IngredientForm';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientFormData) => Promise<void>;
  initialData?: Ingredient | null;
  isLoading?: boolean;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
}) => {
  const [error, setError] = React.useState<string | null>(null);
  const [success, setSuccess] = React.useState<string | null>(null);

  React.useEffect(() => {
    if (!isOpen) {
      setError(null);
      setSuccess(null);
    }
  }, [isOpen]);

  if (!isOpen) return null;

  const handleSubmit = async (data: IngredientFormData) => {
    try {
      setError(null);
      setSuccess(null);
      await onSubmit(data);
      setSuccess(initialData ? 'Ingredient updated successfully.' : 'Ingredient created successfully.');
      window.setTimeout(() => onClose(), 500);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to save ingredient.');
      throw err;
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-xl w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">{initialData ? 'Edit Ingredient' : 'New Ingredient'}</h2>
          <button
            onClick={onClose}
            disabled={isLoading}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-6 space-y-4">
          {success && <div className="p-3 rounded-lg border border-green-500/40 bg-green-500/20 text-green-200">{success}</div>}
          {error && <div className="p-3 rounded-lg border border-red-500/40 bg-red-500/20 text-red-200">{error}</div>}

          <IngredientForm
            initialData={initialData}
            isLoading={isLoading}
            submitLabel={initialData ? 'Update Ingredient' : 'Create Ingredient'}
            onSubmit={handleSubmit}
            onCancel={onClose}
          />
        </div>
      </div>
    </div>
  );
};
