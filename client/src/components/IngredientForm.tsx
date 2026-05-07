import React from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientFormData } from '../types';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  isLoading?: boolean;
  submitLabel?: string;
  onSubmit: (data: IngredientFormData) => Promise<void>;
  onCancel: () => void;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  isLoading = false,
  submitLabel = 'Save',
  onSubmit,
  onCancel,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<IngredientFormData>({
    defaultValues: initialData
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
          unit: 'g',
          caloriesPerUnit: 1,
          protein: 1,
          carbs: 1,
          fat: 1,
        },
  });

  React.useEffect(() => {
    if (!initialData) return;
    reset({
      name: initialData.name,
      unit: initialData.unit,
      caloriesPerUnit: initialData.caloriesPerUnit,
      protein: initialData.protein,
      carbs: initialData.carbs,
      fat: initialData.fat,
    });
  }, [initialData, reset]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
        <input
          {...register('name', {
            required: 'Name is required',
            setValueAs: (v) => (typeof v === 'string' ? v.trim() : v),
            validate: (v) => (v?.length ? true : 'Name is required'),
          })}
          type="text"
          disabled={isLoading}
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400"
          placeholder="Ingredient name"
        />
        {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Unit</label>
        <input
          {...register('unit', {
            required: 'Unit is required',
            setValueAs: (v) => (typeof v === 'string' ? v.trim() : v),
            validate: (v) => (v?.length ? true : 'Unit is required'),
          })}
          type="text"
          disabled={isLoading}
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400"
          placeholder="g, ml, piece..."
        />
        {errors.unit && <span className="text-red-400 text-xs mt-1 block">{errors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
        <NumericField
          label="Calories / Unit"
          register={register('caloriesPerUnit', {
            valueAsNumber: true,
            validate: (v) => (Number.isFinite(v) && v > 0 ? true : 'Must be greater than 0'),
          })}
          error={errors.caloriesPerUnit?.message}
          disabled={isLoading}
        />
        <NumericField
          label="Protein"
          register={register('protein', {
            valueAsNumber: true,
            validate: (v) => (Number.isFinite(v) && v > 0 ? true : 'Must be greater than 0'),
          })}
          error={errors.protein?.message}
          disabled={isLoading}
        />
        <NumericField
          label="Carbs"
          register={register('carbs', {
            valueAsNumber: true,
            validate: (v) => (Number.isFinite(v) && v > 0 ? true : 'Must be greater than 0'),
          })}
          error={errors.carbs?.message}
          disabled={isLoading}
        />
        <NumericField
          label="Fat"
          register={register('fat', {
            valueAsNumber: true,
            validate: (v) => (Number.isFinite(v) && v > 0 ? true : 'Must be greater than 0'),
          })}
          error={errors.fat?.message}
          disabled={isLoading}
        />
      </div>

      <div className="flex space-x-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
        >
          {isLoading ? 'Saving...' : submitLabel}
        </button>
      </div>
    </form>
  );
};

interface NumericFieldProps {
  label: string;
  register: ReturnType<typeof useForm<IngredientFormData>>['register'] extends (...args: any[]) => infer R ? R : never;
  error?: string;
  disabled?: boolean;
}

const NumericField: React.FC<NumericFieldProps> = ({ label, register, error, disabled }) => (
  <div>
    <label className="block text-sm font-semibold text-gray-300 mb-2">{label}</label>
    <input
      {...register}
      type="number"
      step="0.1"
      min="0.1"
      disabled={disabled}
      className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400"
    />
    {error && <span className="text-red-400 text-xs mt-1 block">{error}</span>}
  </div>
);
