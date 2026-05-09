import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientFormValues } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (values: IngredientFormValues) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
  readOnly?: boolean;
}

const defaultValues: IngredientFormValues = {
  name: '',
  unit: 'g',
  caloriesPerUnit: 1,
  protein: 1,
  carbs: 1,
  fat: 1,
};

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
  } = useForm<IngredientFormValues>({ defaultValues });

  useEffect(() => {
    if (initialData) {
      reset({
        name: initialData.name,
        unit: initialData.unit,
        caloriesPerUnit: initialData.caloriesPerUnit,
        protein: initialData.protein,
        carbs: initialData.carbs,
        fat: initialData.fat,
        foodEntityId: initialData.foodEntityId,
      });
      return;
    }

    reset(defaultValues);
  }, [initialData, reset]);

  const submitHandler = async (values: IngredientFormValues) => {
    if (readOnly) {
      return;
    }

    await onSubmit(values);
  };

  return (
    <form onSubmit={handleSubmit(submitHandler)} className="food-form space-y-4 p-6">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
        <input
          {...register('name', { required: 'Name is required' })}
          type="text"
          disabled={isLoading || readOnly}
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-purple-500"
          placeholder="Ingredient name"
        />
        {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Unit</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          type="text"
          disabled={isLoading || readOnly}
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-purple-500"
          placeholder="g, ml, cup..."
        />
        {errors.unit && <span className="text-red-400 text-xs mt-1 block">{errors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {[
          { key: 'caloriesPerUnit', label: 'Calories / Unit' },
          { key: 'protein', label: 'Protein' },
          { key: 'carbs', label: 'Carbs' },
          { key: 'fat', label: 'Fat' },
        ].map((field) => (
          <div key={field.key}>
            <label className="block text-sm font-semibold text-gray-300 mb-2">{field.label}</label>
            <input
              {...register(field.key as keyof IngredientFormValues, {
                valueAsNumber: true,
                required: `${field.label} is required`,
                min: { value: 0.01, message: `${field.label} must be greater than 0` },
              })}
              type="number"
              step="0.01"
              disabled={isLoading || readOnly}
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-purple-500"
            />
            {errors[field.key as keyof IngredientFormValues] && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors[field.key as keyof IngredientFormValues]?.message as string}
              </span>
            )}
          </div>
        ))}
      </div>

      <div className="flex gap-3 pt-4">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-3 border border-white/20 text-gray-300 rounded-lg hover:border-white/40"
          disabled={isLoading}
        >
          {readOnly ? 'Close' : 'Cancel'}
        </button>
        {!readOnly && (
          <button
            type="submit"
            className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg disabled:opacity-50"
            disabled={isLoading}
          >
            {isLoading ? 'Saving...' : 'Submit'}
          </button>
        )}
      </div>
    </form>
  );
};