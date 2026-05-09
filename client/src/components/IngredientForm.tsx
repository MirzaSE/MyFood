import React from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: IngredientCreateDto;
  isLoading?: boolean;
  onCancel: () => void;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
}

const defaultValues: IngredientCreateDto = {
  name: '',
  unit: '',
  caloriesPerUnit: 0,
  protein: 0,
  carbs: 0,
  fat: 0,
};

const inputClass = 'w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-emerald-400 focus:ring-2 focus:ring-emerald-400/30 text-white placeholder-gray-400 transition-all';
const labelClass = 'block text-sm font-semibold text-gray-300 mb-2';
const errorClass = 'text-red-300 text-xs mt-1 block';

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  isLoading = false,
  onCancel,
  onSubmit,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IngredientCreateDto>({
    values: initialData ?? defaultValues,
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      <div>
        <label className={labelClass}>Name</label>
        <input
          {...register('name', {
            required: 'Name is required',
            maxLength: { value: 100, message: 'Name must be 100 characters or less' },
          })}
          className={inputClass}
          disabled={isLoading}
          placeholder="e.g., Olive oil"
          type="text"
        />
        {errors.name && <span className={errorClass}>{errors.name.message}</span>}
      </div>

      <div>
        <label className={labelClass}>Unit</label>
        <input
          {...register('unit', {
            required: 'Unit is required',
            maxLength: { value: 50, message: 'Unit must be 50 characters or less' },
          })}
          className={inputClass}
          disabled={isLoading}
          placeholder="e.g., g, ml, piece"
          type="text"
        />
        {errors.unit && <span className={errorClass}>{errors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <label className={labelClass}>Calories per Unit</label>
          <input
            {...register('caloriesPerUnit', {
              required: 'Calories per unit is required',
              valueAsNumber: true,
              min: { value: 0.0001, message: 'Calories must be greater than zero' },
            })}
            className={inputClass}
            disabled={isLoading}
            min="0.0001"
            step="any"
            type="number"
          />
          {errors.caloriesPerUnit && <span className={errorClass}>{errors.caloriesPerUnit.message}</span>}
        </div>

        <div>
          <label className={labelClass}>Protein</label>
          <input
            {...register('protein', {
              required: 'Protein is required',
              valueAsNumber: true,
              min: { value: 0.0001, message: 'Protein must be greater than zero' },
            })}
            className={inputClass}
            disabled={isLoading}
            min="0.0001"
            step="any"
            type="number"
          />
          {errors.protein && <span className={errorClass}>{errors.protein.message}</span>}
        </div>

        <div>
          <label className={labelClass}>Carbs</label>
          <input
            {...register('carbs', {
              required: 'Carbs is required',
              valueAsNumber: true,
              min: { value: 0.0001, message: 'Carbs must be greater than zero' },
            })}
            className={inputClass}
            disabled={isLoading}
            min="0.0001"
            step="any"
            type="number"
          />
          {errors.carbs && <span className={errorClass}>{errors.carbs.message}</span>}
        </div>

        <div>
          <label className={labelClass}>Fat</label>
          <input
            {...register('fat', {
              required: 'Fat is required',
              valueAsNumber: true,
              min: { value: 0.0001, message: 'Fat must be greater than zero' },
            })}
            className={inputClass}
            disabled={isLoading}
            min="0.0001"
            step="any"
            type="number"
          />
          {errors.fat && <span className={errorClass}>{errors.fat.message}</span>}
        </div>
      </div>

      <div className="flex gap-3 pt-4">
        <button
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all disabled:opacity-50 font-medium"
          disabled={isLoading}
          onClick={onCancel}
          type="button"
        >
          Cancel
        </button>
        <button
          className="flex-1 px-4 py-3 bg-gradient-to-r from-emerald-500 to-cyan-500 hover:from-emerald-600 hover:to-cyan-600 text-white rounded-lg transition-all disabled:opacity-50 font-semibold shadow-lg hover:shadow-emerald-500/30"
          disabled={isLoading}
          type="submit"
        >
          {isLoading ? 'Saving...' : 'Submit'}
        </button>
      </div>
    </form>
  );
};
