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

  const inputClass = "w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-sm transition-all";
  const labelClass = "block text-sm font-semibold text-gray-300 mb-1";
  const errorClass = "text-red-400 text-xs mt-1 block";

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      {errors && errors.length > 0 && (
        <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-3">
          {errors.map((e, i) => <p key={i} className="text-red-300 text-sm">{e}</p>)}
        </div>
      )}

      <div>
        <label className={labelClass}>Name *</label>
        <input
          {...register('name', { required: 'Name is required' })}
          className={inputClass}
          placeholder="e.g. Chicken Breast"
          disabled={isLoading}
        />
        {formErrors.name && <span className={errorClass}>{formErrors.name.message}</span>}
      </div>

      <div>
        <label className={labelClass}>Unit *</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          className={inputClass}
          placeholder="e.g. g, ml, piece"
          disabled={isLoading}
        />
        {formErrors.unit && <span className={errorClass}>{formErrors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className={labelClass}>Calories per unit</label>
          <input
            type="number"
            step="0.01"
            {...register('caloriesPerUnit', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className={inputClass}
            disabled={isLoading}
          />
          {formErrors.caloriesPerUnit && <span className={errorClass}>{formErrors.caloriesPerUnit.message}</span>}
        </div>
        <div>
          <label className={labelClass}>Quantity</label>
          <input
            type="number"
            {...register('quantity', { min: { value: 1, message: 'Must be ≥ 1' }, valueAsNumber: true })}
            className={inputClass}
            disabled={isLoading}
          />
          {formErrors.quantity && <span className={errorClass}>{formErrors.quantity.message}</span>}
        </div>
      </div>

      <div className="grid grid-cols-3 gap-4">
        <div>
          <label className={labelClass}>Protein (g)</label>
          <input
            type="number"
            step="0.01"
            {...register('protein', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className={inputClass}
            disabled={isLoading}
          />
          {formErrors.protein && <span className={errorClass}>{formErrors.protein.message}</span>}
        </div>
        <div>
          <label className={labelClass}>Carbs (g)</label>
          <input
            type="number"
            step="0.01"
            {...register('carbs', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className={inputClass}
            disabled={isLoading}
          />
          {formErrors.carbs && <span className={errorClass}>{formErrors.carbs.message}</span>}
        </div>
        <div>
          <label className={labelClass}>Fat (g)</label>
          <input
            type="number"
            step="0.01"
            {...register('fat', { min: { value: 0, message: 'Must be ≥ 0' }, valueAsNumber: true })}
            className={inputClass}
            disabled={isLoading}
          />
          {formErrors.fat && <span className={errorClass}>{formErrors.fat.message}</span>}
        </div>
      </div>

      <div className="flex gap-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="flex-1 px-4 py-2.5 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium text-sm"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 px-4 py-2.5 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium text-sm shadow-lg hover:shadow-purple-500/50"
        >
          {isLoading ? (
            <span className="flex items-center justify-center">
              <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
              Saving...
            </span>
          ) : (
            defaultValues ? 'Update Ingredient' : 'Create Ingredient'
          )}
        </button>
      </div>
    </form>
  );
};
