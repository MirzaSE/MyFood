import React from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isLoading = false,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IngredientCreateDto>({
    defaultValues: initialData
      ? {
          name: initialData.name,
          unit: initialData.unit,
          caloriesPerUnit: initialData.caloriesPerUnit,
          protein: initialData.protein,
          carbs: initialData.carbs,
          fat: initialData.fat,
        }
      : { protein: 0, carbs: 0, fat: 0 },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="col-span-2">
          <label className="block text-sm font-semibold text-gray-300 mb-1">Name *</label>
          <input
            {...register('name', { required: 'Name is required' })}
            type="text"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            placeholder="e.g., Chicken Breast"
            disabled={isLoading}
          />
          {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
        </div>

        <div className="col-span-2">
          <label className="block text-sm font-semibold text-gray-300 mb-1">Unit *</label>
          <input
            {...register('unit', { required: 'Unit is required' })}
            type="text"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            placeholder="e.g., g, ml, tsp"
            disabled={isLoading}
          />
          {errors.unit && <span className="text-red-400 text-xs mt-1 block">{errors.unit.message}</span>}
        </div>

        <div className="col-span-2">
          <label className="block text-sm font-semibold text-gray-300 mb-1">Calories per Unit *</label>
          <input
            {...register('caloriesPerUnit', {
              required: 'Calories per unit is required',
              valueAsNumber: true,
              min: { value: 0.01, message: 'Must be greater than 0' },
            })}
            type="number"
            step="0.01"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            placeholder="e.g., 1.65"
            disabled={isLoading}
          />
          {errors.caloriesPerUnit && <span className="text-red-400 text-xs mt-1 block">{errors.caloriesPerUnit.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-1">Protein (g)</label>
          <input
            {...register('protein', {
              valueAsNumber: true,
              min: { value: 0, message: 'Must be ≥ 0' },
            })}
            type="number"
            step="0.01"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            placeholder="0"
            disabled={isLoading}
          />
          {errors.protein && <span className="text-red-400 text-xs mt-1 block">{errors.protein.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-1">Carbs (g)</label>
          <input
            {...register('carbs', {
              valueAsNumber: true,
              min: { value: 0, message: 'Must be ≥ 0' },
            })}
            type="number"
            step="0.01"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            placeholder="0"
            disabled={isLoading}
          />
          {errors.carbs && <span className="text-red-400 text-xs mt-1 block">{errors.carbs.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-1">Fat (g)</label>
          <input
            {...register('fat', {
              valueAsNumber: true,
              min: { value: 0, message: 'Must be ≥ 0' },
            })}
            type="number"
            step="0.01"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            placeholder="0"
            disabled={isLoading}
          />
          {errors.fat && <span className="text-red-400 text-xs mt-1 block">{errors.fat.message}</span>}
        </div>
      </div>

      <div className="flex space-x-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-2.5 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
          disabled={isLoading}
        >
          Cancel
        </button>
        <button
          type="submit"
          className="flex-1 px-4 py-2.5 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg"
          disabled={isLoading}
        >
          {isLoading ? (
            <span className="flex items-center justify-center">
              <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
              Saving...
            </span>
          ) : initialData ? 'Update' : 'Create'}
        </button>
      </div>
    </form>
  );
};
