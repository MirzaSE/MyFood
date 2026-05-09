import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
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
    reset(initialData ?? {
      name: '',
      unit: '',
      caloriesPerUnit: 0,
      protein: 0,
      carbs: 0,
      fat: 0,
    });
  }, [initialData, reset]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      <div>
        <label className="mb-2 block text-sm font-semibold text-gray-200">Name</label>
        <input
          {...register('name', { required: 'Name is required' })}
          className="w-full rounded-lg border border-white/20 bg-slate-950 px-4 py-3 text-white"
          disabled={isLoading}
        />
        {errors.name && <p className="mt-1 text-sm text-red-300">{errors.name.message}</p>}
      </div>

      <div>
        <label className="mb-2 block text-sm font-semibold text-gray-200">Unit</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          className="w-full rounded-lg border border-white/20 bg-slate-950 px-4 py-3 text-white"
          disabled={isLoading}
        />
        {errors.unit && <p className="mt-1 text-sm text-red-300">{errors.unit.message}</p>}
      </div>

      {[
        { key: 'caloriesPerUnit', label: 'Calories Per Unit' },
        { key: 'protein', label: 'Protein' },
        { key: 'carbs', label: 'Carbs' },
        { key: 'fat', label: 'Fat' },
      ].map((field) => (
        <div key={field.key}>
          <label className="mb-2 block text-sm font-semibold text-gray-200">{field.label}</label>
          <input
            {...register(field.key as keyof IngredientCreateDto, {
              valueAsNumber: true,
              min: { value: field.key === 'caloriesPerUnit' ? 0.01 : 0, message: 'Value must be greater than 0' },
              required: `${field.label} is required`,
            })}
            type="number"
            step="0.01"
            className="w-full rounded-lg border border-white/20 bg-slate-950 px-4 py-3 text-white"
            disabled={isLoading}
          />
          {errors[field.key as keyof IngredientCreateDto] && (
            <p className="mt-1 text-sm text-red-300">
              {errors[field.key as keyof IngredientCreateDto]?.message as string}
            </p>
          )}
        </div>
      ))}

      <div className="flex gap-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 rounded-lg border border-white/20 px-4 py-3 text-gray-200 transition hover:border-white/40"
          disabled={isLoading}
        >
          Cancel
        </button>
        <button
          type="submit"
          className="flex-1 rounded-lg bg-amber-500 px-4 py-3 font-semibold text-slate-900 transition hover:bg-amber-400 disabled:opacity-60"
          disabled={isLoading}
        >
          {isLoading ? 'Saving...' : initialData ? 'Update Ingredient' : 'Create Ingredient'}
        </button>
      </div>
    </form>
  );
};
