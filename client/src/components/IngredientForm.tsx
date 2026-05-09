import React from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientCreateDto } from '../types/ingredient';

interface Props {
  initialData?: { name: string; quantity?: string } | null;
  onSubmit: (data: IngredientCreateDto) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export const IngredientForm: React.FC<Props> = ({ initialData, onSubmit, onCancel, isLoading }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IngredientCreateDto>({
    defaultValues: {
      name: initialData?.name ?? '',
      quantity: initialData?.quantity ?? '',
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      {/* Name */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">
          Name <span className="text-red-400">*</span>
        </label>
        <input
          {...register('name', { required: 'Name is required' })}
          className="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition"
          placeholder="e.g. Chicken Breast"
        />
        {errors.name && <p className="mt-1 text-sm text-red-400">{errors.name.message}</p>}
      </div>

      {/* Unit / Quantity */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">Unit / Quantity</label>
        <input
          {...register('quantity')}
          className="w-full bg-white/5 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:border-transparent transition"
          placeholder="e.g. 200g"
        />
      </div>

      {/* Buttons */}
      <div className="flex justify-end space-x-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="px-5 py-2.5 rounded-lg border border-white/20 text-gray-300 hover:text-white hover:border-white/40 transition disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="px-5 py-2.5 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white font-semibold transition disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-purple-500/30"
        >
          {isLoading ? 'Saving…' : 'Save'}
        </button>
      </div>
    </form>
  );
};
