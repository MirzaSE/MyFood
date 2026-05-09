import React from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface Props {
  initialData?: Ingredient | null;
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
      unit: initialData?.unit ?? 'g',
      caloriesPerUnit: initialData?.caloriesPerUnit ?? 0,
      protein: initialData?.protein ?? 0,
      carbs: initialData?.carbs ?? 0,
      fat: initialData?.fat ?? 0,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">
          Name <span className="text-red-400">*</span>
        </label>
        <input
          {...register('name', { required: 'Name is required' })}
          className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
          placeholder="e.g. Chicken breast"
        />
        {errors.name && <p className="mt-1 text-xs text-red-400">{errors.name.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">
          Unit <span className="text-red-400">*</span>
        </label>
        <select
          {...register('unit', { required: 'Unit is required' })}
          className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-purple-500/60"
        >
          <option value="g">g (grams)</option>
          <option value="ml">ml (milliliters)</option>
          <option value="oz">oz (ounces)</option>
          <option value="cup">cup</option>
          <option value="tbsp">tbsp</option>
          <option value="tsp">tsp</option>
          <option value="piece">piece</option>
        </select>
        {errors.unit && <p className="mt-1 text-xs text-red-400">{errors.unit.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">
          Calories per Unit <span className="text-red-400">*</span>
        </label>
        <input
          type="number"
          step="0.01"
          {...register('caloriesPerUnit', {
            required: 'Calories is required',
            min: { value: 0, message: 'Calories must be greater than 0' },
            valueAsNumber: true,
          })}
          className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
          placeholder="100"
        />
        {errors.caloriesPerUnit && <p className="mt-1 text-xs text-red-400">{errors.caloriesPerUnit.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">
          Protein (g) <span className="text-red-400">*</span>
        </label>
        <input
          type="number"
          step="0.01"
          {...register('protein', {
            required: 'Protein is required',
            min: { value: 0, message: 'Protein must be 0 or greater' },
            valueAsNumber: true,
          })}
          className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
          placeholder="0"
        />
        {errors.protein && <p className="mt-1 text-xs text-red-400">{errors.protein.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">
          Carbs (g) <span className="text-red-400">*</span>
        </label>
        <input
          type="number"
          step="0.01"
          {...register('carbs', {
            required: 'Carbs is required',
            min: { value: 0, message: 'Carbs must be 0 or greater' },
            valueAsNumber: true,
          })}
          className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
          placeholder="0"
        />
        {errors.carbs && <p className="mt-1 text-xs text-red-400">{errors.carbs.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">
          Fat (g) <span className="text-red-400">*</span>
        </label>
        <input
          type="number"
          step="0.01"
          {...register('fat', {
            required: 'Fat is required',
            min: { value: 0, message: 'Fat must be 0 or greater' },
            valueAsNumber: true,
          })}
          className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
          placeholder="0"
        />
        {errors.fat && <p className="mt-1 text-xs text-red-400">{errors.fat.message}</p>}
      </div>

      <div className="flex gap-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-2.5 rounded-lg border border-white/10 text-gray-300 hover:bg-white/5 transition-colors"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 px-4 py-2.5 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white font-semibold transition-all disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isLoading ? 'Saving...' : initialData ? 'Update' : 'Create'}
        </button>
      </div>
    </form>
  );
};