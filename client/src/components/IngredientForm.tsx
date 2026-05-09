import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type {
  Ingredient,
  IngredientCreateDto,
} from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  isLoading?: boolean;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  isLoading = false,
  onSubmit,
  onCancel,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<IngredientCreateDto>({
    defaultValues: {
      name: '',
      unit: '',
      quantity: 1,
      caloriesPerUnit: 1,
      protein: 1,
      carbs: 1,
      fat: 1,
    },
  });

  useEffect(() => {
    reset({
      name: initialData?.name ?? '',
      unit: initialData?.unit ?? '',
      quantity: initialData?.quantity ?? 1,
      caloriesPerUnit: initialData?.caloriesPerUnit ?? 1,
      protein: initialData?.protein ?? 1,
      carbs: initialData?.carbs ?? 1,
      fat: initialData?.fat ?? 1,
    });
  }, [initialData, reset]);

  const submitForm = async (data: IngredientCreateDto) => {
    await onSubmit({
      name: data.name.trim(),
      unit: data.unit.trim(),
      quantity: data.quantity,
      caloriesPerUnit: data.caloriesPerUnit,
      protein: data.protein,
      carbs: data.carbs,
      fat: data.fat,
    });
  };

  return (
    <form onSubmit={handleSubmit(submitForm)} className="space-y-5">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">
          Ingredient Name
        </label>
        <input
          {...register('name', {
            required: 'Name is required',
            validate: (value) => value.trim().length > 0 || 'Name cannot be empty',
          })}
          type="text"
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 text-white placeholder-gray-400 transition-all"
          placeholder="e.g., Tomato"
          disabled={isLoading}
        />
        {errors.name && (
          <span className="text-red-400 text-xs mt-1 block">
            {errors.name.message}
          </span>
        )}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">
          Unit
        </label>
        <input
          {...register('unit', {
            required: 'Unit is required',
            validate: (value) => value.trim().length > 0 || 'Unit cannot be empty',
          })}
          type="text"
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 text-white placeholder-gray-400 transition-all"
          placeholder="e.g., gram, cup, piece"
          disabled={isLoading}
        />
        {errors.unit && (
          <span className="text-red-400 text-xs mt-1 block">
            {errors.unit.message}
          </span>
        )}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">
          Quantity in stock
        </label>
        <input
          {...register('quantity', {
            required: 'Quantity is required',
            valueAsNumber: true,
            validate: (value) => Number.isFinite(value) && value > 0 || 'Quantity must be greater than 0',
          })}
          type="number"
          min="1"
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 text-white placeholder-gray-400 transition-all"
          disabled={isLoading}
        />
        {errors.quantity && (
          <span className="text-red-400 text-xs mt-1 block">
            {errors.quantity.message}
          </span>
        )}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {[
          ['caloriesPerUnit', 'Calories per unit'],
          ['protein', 'Protein'],
          ['carbs', 'Carbs'],
          ['fat', 'Fat'],
        ].map(([field, label]) => (
          <div key={field}>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              {label}
            </label>
            <input
              {...register(field as keyof IngredientCreateDto, {
                required: `${label} is required`,
                valueAsNumber: true,
                validate: (value) =>
                  typeof value === 'number' && Number.isFinite(value) && value > 0
                    ? true
                    : `${label} must be greater than 0`,
              })}
              type="number"
              min="0.01"
              step="0.01"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 text-white placeholder-gray-400 transition-all"
              disabled={isLoading}
            />
            {errors[field as keyof IngredientCreateDto] && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors[field as keyof IngredientCreateDto]?.message}
              </span>
            )}
          </div>
        ))}
      </div>

      <div className="flex gap-3 pt-2">
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
          className="flex-1 px-4 py-3 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-emerald-500/30"
          disabled={isLoading}
        >
          {isLoading ? 'Saving...' : initialData ? 'Update' : 'Create'}
        </button>
      </div>
    </form>
  );
};
