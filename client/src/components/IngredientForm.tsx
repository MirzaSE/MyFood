import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void> | void;
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
    reset,
    formState: { errors },
  } = useForm<IngredientCreateDto>();

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
      reset({
        name: '',
        unit: '',
        caloriesPerUnit: 0,
        protein: 0,
        carbs: 0,
        fat: 0,
      });
    }
  }, [initialData, reset]);

  const inputClass =
    'w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none ' +
    'focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white ' +
    'placeholder-gray-400 text-sm transition-all';

  const labelClass = 'block text-sm font-semibold text-gray-300 mb-2';
  const errClass = 'text-red-400 text-xs mt-1 block';

  const submitForm = async (data: IngredientCreateDto) => {
    await onSubmit({
      ...data,
      caloriesPerUnit: Number(data.caloriesPerUnit),
      protein: Number(data.protein),
      carbs: Number(data.carbs),
      fat: Number(data.fat),
    });
  };

  return (
    <form onSubmit={handleSubmit(submitForm)} className="space-y-5">
      <div>
        <label className={labelClass}>Name</label>
        <input
          {...register('name', { required: 'Name is required' })}
          type="text"
          placeholder="e.g., Tomato"
          className={inputClass}
          disabled={isLoading}
        />
        {errors.name && <span className={errClass}>{errors.name.message}</span>}
      </div>

      <div>
        <label className={labelClass}>Unit</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          type="text"
          placeholder="e.g., g, ml, cup"
          className={inputClass}
          disabled={isLoading}
        />
        {errors.unit && <span className={errClass}>{errors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className={labelClass}>Calories per unit</label>
          <input
            {...register('caloriesPerUnit', {
              required: 'Required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be ≥ 0' },
            })}
            type="number"
            step="0.01"
            className={inputClass}
            disabled={isLoading}
          />
          {errors.caloriesPerUnit && <span className={errClass}>{errors.caloriesPerUnit.message}</span>}
        </div>

        <div>
          <label className={labelClass}>Protein</label>
          <input
            {...register('protein', {
              required: 'Required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be ≥ 0' },
            })}
            type="number"
            step="0.01"
            className={inputClass}
            disabled={isLoading}
          />
          {errors.protein && <span className={errClass}>{errors.protein.message}</span>}
        </div>

        <div>
          <label className={labelClass}>Carbs</label>
          <input
            {...register('carbs', {
              required: 'Required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be ≥ 0' },
            })}
            type="number"
            step="0.01"
            className={inputClass}
            disabled={isLoading}
          />
          {errors.carbs && <span className={errClass}>{errors.carbs.message}</span>}
        </div>

        <div>
          <label className={labelClass}>Fat</label>
          <input
            {...register('fat', {
              required: 'Required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be ≥ 0' },
            })}
            type="number"
            step="0.01"
            className={inputClass}
            disabled={isLoading}
          />
          {errors.fat && <span className={errClass}>{errors.fat.message}</span>}
        </div>
      </div>

      <div className="flex space-x-3 pt-4">
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
        >
          {isLoading ? (
            <span className="flex items-center justify-center">
              <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
              Saving…
            </span>
          ) : (
            initialData ? 'Update Ingredient' : 'Create Ingredient'
          )}
        </button>
      </div>
    </form>
  );
};
