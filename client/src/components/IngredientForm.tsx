import React from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
  initialData?: Partial<IngredientCreateDto>;
  isLoading?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  onSubmit,
  onCancel,
  initialData,
  isLoading = false,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IngredientCreateDto>({
    defaultValues: {
      name: initialData?.name || '',
      quantity: initialData?.quantity || '',
    },
  });

  const onFormSubmit = async (data: IngredientCreateDto) => {
    try {
      // Don't send foodId from standalone ingredient form (would cause FK violation if 0)
      const payload: IngredientCreateDto = {
        name: data.name,
        quantity: data.quantity,
      };
      await onSubmit(payload);
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit(onFormSubmit)} className="space-y-5">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">
          Name
        </label>
        <input
          {...register('name', { required: 'Name is required' })}
          type="text"
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
          placeholder="e.g., Salt"
          disabled={isLoading}
        />
        {errors.name && (
          <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>
        )}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">
          Quantity
        </label>
        <input
          {...register('quantity', { required: 'Quantity is required' })}
          type="text"
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
          placeholder="e.g., 10g"
          disabled={isLoading}
        />
        {errors.quantity && (
          <span className="text-red-400 text-xs mt-1 block">{errors.quantity.message}</span>
        )}
      </div>

      <div className="flex space-x-3 pt-4">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
          disabled={isLoading}
        >
          Cancel
        </button>
        <button
          type="submit"
          className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
          disabled={isLoading}
        >
          {isLoading ? (
            <span className="flex items-center justify-center">
              <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
              Saving...
            </span>
          ) : (
            'Save'
          )}
        </button>
      </div>
    </form>
  );
};
