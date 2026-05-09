import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  isLoading?: boolean;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  isLoading = false,
  onSubmit,
  onCancel,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<IngredientCreateDto>();

  useEffect(() => {
    reset({
      name: initialData?.name || '',
      unit: initialData?.unit || '',
      caloriesPerUnit: initialData?.caloriesPerUnit ?? undefined,
      protein: initialData?.protein ?? undefined,
      carbs: initialData?.carbs ?? undefined,
      fat: initialData?.fat ?? undefined,
    });
  }, [initialData, reset]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="p-8 space-y-6">
      {[
        { name: 'name', label: 'Name', type: 'text', placeholder: 'e.g., Chicken breast' },
        { name: 'unit', label: 'Unit', type: 'text', placeholder: 'e.g., g, ml, piece' },
        { name: 'caloriesPerUnit', label: 'Calories per Unit', type: 'number', placeholder: 'e.g., 1.65' },
        { name: 'protein', label: 'Protein', type: 'number', placeholder: 'e.g., 31' },
        { name: 'carbs', label: 'Carbs', type: 'number', placeholder: 'e.g., 0' },
        { name: 'fat', label: 'Fat', type: 'number', placeholder: 'e.g., 3.6' },
      ].map((field) => (
        <div key={field.name}>
          <label className="block text-sm font-semibold text-gray-300 mb-2">
            {field.label}
          </label>

          <input
            {...register(field.name as keyof IngredientCreateDto, {
              required: `${field.label} is required`,
              valueAsNumber: field.type === 'number',
              validate: (value) => {
                if (field.type === 'number') {
                  const numberValue = Number(value);
                  if (Number.isNaN(numberValue)) return `${field.label} is required`;
                  if (numberValue < 0) return `${field.label} must be 0 or greater`;
                }

                if (field.type === 'text' && String(value).trim().length === 0) {
                  return `${field.label} is required`;
                }

                return true;
              },
            })}
            type={field.type}
            step={field.type === 'number' ? '0.01' : undefined}
            className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            placeholder={field.placeholder}
            disabled={isLoading}
          />

          {errors[field.name as keyof IngredientCreateDto] && (
            <span className="text-red-400 text-xs mt-1 block">
              {errors[field.name as keyof IngredientCreateDto]?.message}
            </span>
          )}
        </div>
      ))}

      <div className="flex space-x-3 pt-4">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all disabled:opacity-50 font-medium"
          disabled={isLoading}
        >
          Cancel
        </button>

        <button
          type="submit"
          className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all disabled:opacity-50 font-medium shadow-lg"
          disabled={isLoading}
        >
          {isLoading ? 'Saving...' : initialData ? 'Update Ingredient' : 'Create Ingredient'}
        </button>
      </div>
    </form>
  );
};