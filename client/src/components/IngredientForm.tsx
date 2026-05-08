import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientCreateDto, Ingredient } from '../types';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
  readOnly?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isLoading = false,
  readOnly = false,
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
      caloriesPerUnit: 0,
      protein: 0,
      carbs: 0,
      fat: 0,
    },
  });

  useEffect(() => {
    reset(
      initialData
        ? {
            name: initialData.name,
            unit: initialData.unit,
            caloriesPerUnit: initialData.caloriesPerUnit,
            protein: initialData.protein,
            carbs: initialData.carbs,
            fat: initialData.fat,
          }
        : {
            name: '',
            unit: '',
            caloriesPerUnit: 0,
            protein: 0,
            carbs: 0,
            fat: 0,
          }
    );
  }, [initialData, reset]);

  const onSubmitForm = async (data: IngredientCreateDto) => {
    if (readOnly) {
      return;
    }

    await onSubmit(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmitForm)} className="space-y-6">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
        <input
          {...register('name', {
            required: 'Name is required',
            maxLength: { value: 100, message: 'Name cannot be longer than 100 characters' },
          })}
          type="text"
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
          placeholder="e.g., Tomato"
          disabled={isLoading || readOnly}
        />
        {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Unit</label>
        <input
          {...register('unit', {
            required: 'Unit is required',
            maxLength: { value: 50, message: 'Unit cannot be longer than 50 characters' },
          })}
          type="text"
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
          placeholder="e.g., g, ml"
          disabled={isLoading || readOnly}
        />
        {errors.unit && <span className="text-red-400 text-xs mt-1 block">{errors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Calories / Unit</label>
          <input
            {...register('caloriesPerUnit', {
              required: 'Calories are required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be zero or greater' },
            })}
            type="number"
            step="0.01"
            className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading || readOnly}
          />
          {errors.caloriesPerUnit && (
            <span className="text-red-400 text-xs mt-1 block">{errors.caloriesPerUnit.message}</span>
          )}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Protein</label>
          <input
            {...register('protein', {
              required: 'Protein is required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be zero or greater' },
            })}
            type="number"
            step="0.01"
            className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading || readOnly}
          />
          {errors.protein && (
            <span className="text-red-400 text-xs mt-1 block">{errors.protein.message}</span>
          )}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Carbs</label>
          <input
            {...register('carbs', {
              required: 'Carbs are required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be zero or greater' },
            })}
            type="number"
            step="0.01"
            className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading || readOnly}
          />
          {errors.carbs && (
            <span className="text-red-400 text-xs mt-1 block">{errors.carbs.message}</span>
          )}
        </div>

        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Fat</label>
          <input
            {...register('fat', {
              required: 'Fat is required',
              valueAsNumber: true,
              min: { value: 0, message: 'Must be zero or greater' },
            })}
            type="number"
            step="0.01"
            className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            disabled={isLoading || readOnly}
          />
          {errors.fat && (
            <span className="text-red-400 text-xs mt-1 block">{errors.fat.message}</span>
          )}
        </div>
      </div>

      <div className="flex space-x-3 pt-4">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
          disabled={isLoading}
        >
          {readOnly ? 'Close' : 'Cancel'}
        </button>
        {!readOnly && (
          <button
            type="submit"
            className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
            disabled={isLoading}
          >
            {isLoading ? 'Saving...' : 'Save'}
          </button>
        )}
      </div>
    </form>
  );
};
