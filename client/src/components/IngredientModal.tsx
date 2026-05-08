import { X } from 'lucide-react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientModalProps {
  isOpen: boolean;
  initialData?: Ingredient | null;
  isLoading?: boolean;
  message?: string | null;
  error?: string | null;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  initialData,
  isLoading = false,
  message,
  error,
  onClose,
  onSubmit,
}) => {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-40 p-4">
      <div className="w-full max-w-2xl rounded-2xl bg-white p-6 shadow-xl">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-xl font-bold text-gray-900">
            {initialData ? 'Edit Ingredient' : 'New Ingredient'}
          </h2>
          <button
            type="button"
            onClick={onClose}
            className="rounded-full p-2 hover:bg-gray-100"
          >
            <X size={20} />
          </button>
        </div>

        {message && (
          <div className="mb-4 rounded-lg bg-green-50 p-3 text-sm text-green-700">
            {message}
          </div>
        )}

        {error && (
          <div className="mb-4 rounded-lg bg-red-50 p-3 text-sm text-red-700">
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