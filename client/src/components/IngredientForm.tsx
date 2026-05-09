import React from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientCreateDto } from '../types/ingredient';

interface Props {
  defaultValues?: Partial<IngredientCreateDto>;
  onSubmit: (data: IngredientCreateDto) => void;
  onCancel: () => void;
  isLoading?: boolean;
  errors?: string[];
}

export const IngredientForm: React.FC<Props> = ({ defaultValues, onSubmit, onCancel, isLoading, errors }) => {
  const { register, handleSubmit, formState: { errors: formErrors } } = useForm<IngredientCreateDto>({
    defaultValues: {
      name: '',
      unit: '',
      caloriesPerUnit: 0,
      protein: 0,
      carbs: 0,
      fat: 0,
      quantity: 1,
      ...defaultValues,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      {errors && errors.length > 0 && (
        <div className="bg-red-50 border border-red-200 rounded p-3">
          {errors.map((e, i) => <p key={i} className="text-red-600 text-sm">{e}</p>)}
        </div>
      )}

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Name *</label>
        <input
          {...register('name', { required: 'Name is required' })}
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="e.g. Chicken Breast"
        />
        {formErrors.name && <p className="text-red-500 text-sm mt-1">{formErrors.name.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Unit *</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="e.g. g, ml, piece"
        />
        {formErrors.unit && <p className="text-red-500 text-sm mt-1">{formErrors.unit.message}</p>}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Calories per unit</label>
          <input
            type="number"
            step="0.01"
            {...register('caloriesPerUnit', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {formErrors.caloriesPerUnit && <p className="text-red-500 text-sm mt-1">{formErrors.caloriesPerUnit.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Quantity</label>
          <input
            type="number"
            {...register('quantity', { min: { value: 1, message: 'Must be ≥ 1' }, valueAsNumber: true })}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {formErrors.quantity && <p className="text-red-500 text-sm mt-1">{formErrors.quantity.message}</p>}
        </div>
      </div>

      <div className="grid grid-cols-3 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Protein (g)</label>
          <input
            type="number"
            step="0.01"
            {...register('protein', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {formErrors.protein && <p className="text-red-500 text-sm mt-1">{formErrors.protein.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Carbs (g)</label>
          <input
            type="number"
            step="0.01"
            {...register('carbs', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {formErrors.carbs && <p className="text-red-500 text-sm mt-1">{formErrors.carbs.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Fat (g)</label>
          <input
            type="number"
            step="0.01"
            {...register('fat', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {formErrors.fat && <p className="text-red-500 text-sm mt-1">{formErrors.fat.message}</p>}
        </div>
      </div>

      <div className="flex gap-3 pt-2">
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50 font-medium"
        >
          {isLoading ? 'Saving...' : 'Save'}
        </button>
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 bg-gray-100 text-gray-700 py-2 rounded-lg hover:bg-gray-200 font-medium"
        >
          Cancel
        </button>
      </div>
    </form>
  );
};
