import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
  submitLabel?: string;
}

const emptyValues: IngredientCreateDto = {
  name: '',
  unit: '',
  caloriesPerUnit: 0,
  protein: 0,
  carbs: 0,
  fat: 0,
};

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isLoading = false,
  submitLabel,
}) => {
  const {
    register,
    handleSubmit,
    reset,
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
      : emptyValues,
  });

  useEffect(() => {
    if (initialData) {
      reset({
        name: initialData.name,
        unit: initialData.unit,
        caloriesPerUnit: initialData.caloriesPerUnit,
        protein: initialData.protein,
        carbs: initialData.carbs,
        fat: initialData.fat,
      });
    } else {
      reset(emptyValues);
    }
  }, [initialData, reset]);

  const onSubmitForm = async (data: IngredientCreateDto) => {
    await onSubmit(data);
  };

  const numberRules = (label: string) => ({
    valueAsNumber: true,
    required: `${label} is required`,
    min: { value: 0, message: `${label} must be 0 or greater` },
  });

  return (
    <form onSubmit={handleSubmit(onSubmitForm)} className="food-form p-8 space-y-4">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
        <input
          {...register('name', {
            required: 'Name is required',
            maxLength: { value: 100, message: 'Max 100 characters' },
          })}
          type="text"
          placeholder="e.g., Apple"
          className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-base transition-all"
          disabled={isLoading}
        />
        {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Unit</label>
        <input
          {...register('unit', {
            required: 'Unit is required',
            maxLength: { value: 50, message: 'Max 50 characters' },
          })}
          type="text"
          placeholder="e.g., g, ml, piece"
          className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-base transition-all"
          disabled={isLoading}
        />
        {errors.unit && <span className="text-red-400 text-xs mt-1 block">{errors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Calories / unit</label>
          <input
            {...register('caloriesPerUnit', numberRules('Calories per unit'))}
            type="number"
            step="0.01"
            placeholder="0.52"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading}
          />
          {errors.caloriesPerUnit && <span className="text-red-400 text-xs mt-1 block">{errors.caloriesPerUnit.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Protein (g)</label>
          <input
            {...register('protein', numberRules('Protein'))}
            type="number"
            step="0.01"
            placeholder="0.3"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading}
          />
          {errors.protein && <span className="text-red-400 text-xs mt-1 block">{errors.protein.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Carbs (g)</label>
          <input
            {...register('carbs', numberRules('Carbs'))}
            type="number"
            step="0.01"
            placeholder="14.0"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading}
          />
          {errors.carbs && <span className="text-red-400 text-xs mt-1 block">{errors.carbs.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Fat (g)</label>
          <input
            {...register('fat', numberRules('Fat'))}
            type="number"
            step="0.01"
            placeholder="0.2"
            className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading}
          />
          {errors.fat && <span className="text-red-400 text-xs mt-1 block">{errors.fat.message}</span>}
        </div>
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
            submitLabel ?? (initialData ? 'Update Ingredient' : 'Create Ingredient')
          )}
        </button>
      </div>
    </form>
  );
};
