import React from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientCreateDto } from '../types/ingredient';

interface Props {
  initialData?: IngredientCreateDto;
  onSubmit: (data: IngredientCreateDto) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export const IngredientForm: React.FC<Props> = ({ initialData, onSubmit, onCancel, isLoading }) => {
  const { register, handleSubmit, formState: { errors } } = useForm<IngredientCreateDto>({
    defaultValues: initialData,
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">Name *</label>
        <input
          {...register('name', { required: 'Name is required' })}
          className="w-full bg-white/10 border border-white/20 rounded-lg px-4 py-2 text-white"
          placeholder="e.g. Sugar"
        />
        {errors.name && <p className="text-red-400 text-sm mt-1">{errors.name.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">Unit *</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          className="w-full bg-white/10 border border-white/20 rounded-lg px-4 py-2 text-white"
          placeholder="e.g. g, ml, cup"
        />
        {errors.unit && <p className="text-red-400 text-sm mt-1">{errors.unit.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">Calories per Unit *</label>
        <input
          type="number"
          {...register('caloriesPerUnit', {
            required: 'Calories is required',
            min: { value: 0, message: 'Must be 0 or more' },
          })}
          className="w-full bg-white/10 border border-white/20 rounded-lg px-4 py-2 text-white"
        />
        {errors.caloriesPerUnit && <p className="text-red-400 text-sm mt-1">{errors.caloriesPerUnit.message}</p>}
      </div>

      <div className="grid grid-cols-3 gap-3">
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Protein (g)</label>
          <input
            type="number"
            step="0.1"
            {...register('protein', { min: { value: 0, message: 'Must be 0 or more' } })}
            className="w-full bg-white/10 border border-white/20 rounded-lg px-4 py-2 text-white"
          />
          {errors.protein && <p className="text-red-400 text-sm mt-1">{errors.protein.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Carbs (g)</label>
          <input
            type="number"
            step="0.1"
            {...register('carbs', { min: { value: 0, message: 'Must be 0 or more' } })}
            className="w-full bg-white/10 border border-white/20 rounded-lg px-4 py-2 text-white"
          />
          {errors.carbs && <p className="text-red-400 text-sm mt-1">{errors.carbs.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Fat (g)</label>
          <input
            type="number"
            step="0.1"
            {...register('fat', { min: { value: 0, message: 'Must be 0 or more' } })}
            className="w-full bg-white/10 border border-white/20 rounded-lg px-4 py-2 text-white"
          />
          {errors.fat && <p className="text-red-400 text-sm mt-1">{errors.fat.message}</p>}
        </div>
      </div>

      <div className="flex space-x-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-2 bg-white/10 text-white rounded-lg hover:bg-white/20 transition"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition disabled:opacity-50"
        >
          {isLoading ? 'Saving...' : 'Save'}
        </button>
      </div>
    </form>
  );
};