import React, { useEffect, useState } from 'react';
import { X } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientFormValues } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (values: IngredientFormValues) => Promise<void>;
  initialData?: Ingredient | null;
  isLoading?: boolean;
  mode?: 'create' | 'edit' | 'view';
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
  mode = 'create',
}) => {
  const [statusMessage, setStatusMessage] = useState<string | null>(null);
  const [statusType, setStatusType] = useState<'success' | 'error' | null>(null);

  useEffect(() => {
    if (isOpen) {
      setStatusMessage(null);
      setStatusType(null);
    }
  }, [isOpen]);

  if (!isOpen) {
    return null;
  }

  const handleSubmit = async (values: IngredientFormValues) => {
    try {
      await onSubmit(values);
      setStatusType('success');
      setStatusMessage(mode === 'edit' ? 'Ingredient updated successfully.' : 'Ingredient created successfully.');
      window.setTimeout(() => {
        onClose();
      }, 700);
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Failed to save ingredient.';
      setStatusType('error');
      setStatusMessage(message);
      throw error;
    }
  };

  const title = mode === 'edit' ? 'Edit Ingredient' : mode === 'view' ? 'Ingredient Details' : 'New Ingredient';

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-5 flex items-center justify-between">
          <h2 className="text-xl font-bold text-white">{title}</h2>
          <button onClick={onClose} className="text-white/80 hover:text-white" disabled={isLoading}>
            <X size={22} />
          </button>
        </div>

        {statusMessage && (
          <div className={`mx-6 mt-6 rounded-lg border px-4 py-3 text-sm ${
            statusType === 'success'
              ? 'bg-green-500/20 border-green-500/40 text-green-200'
              : 'bg-red-500/20 border-red-500/40 text-red-200'
          }`}>
            {statusMessage}
          </div>
        )}

        <IngredientForm
          initialData={initialData}
          onSubmit={handleSubmit}
          onCancel={onClose}
          isLoading={isLoading}
          readOnly={mode === 'view'}
        />
      </div>
    </div>
  );
};
