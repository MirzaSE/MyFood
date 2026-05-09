import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void> | void;
  onCancel: () => void;
  isSubmitting?: boolean;
  submitLabel?: string;
}

type FormValues = {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein?: number | null;
  carbs?: number | null;
  fat?: number | null;
};

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isSubmitting = false,
  submitLabel,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      name: '',
      unit: '',
      caloriesPerUnit: 0,
      protein: undefined,
      carbs: undefined,
      fat: undefined,
    },
  });

  useEffect(() => {
    if (initialData) {
      reset({
        name: initialData.name,
        unit: initialData.unit ?? '',
        caloriesPerUnit: initialData.caloriesPerUnit,
        protein: initialData.protein ?? undefined,
        carbs: initialData.carbs ?? undefined,
        fat: initialData.fat ?? undefined,
      });
    } else {
      reset({
        name: '',
        unit: '',
        caloriesPerUnit: 0,
        protein: undefined,
        carbs: undefined,
        fat: undefined,
      });
    }
  }, [initialData, reset]);

  const submit = async (values: FormValues) => {
    const payload: IngredientCreateDto = {
      name: values.name.trim(),
      unit: values.unit.trim(),
      caloriesPerUnit: Number(values.caloriesPerUnit),
      protein: values.protein === undefined || values.protein === null ? null : Number(values.protein),
      carbs: values.carbs === undefined || values.carbs === null ? null : Number(values.carbs),
      fat: values.fat === undefined || values.fat === null ? null : Number(values.fat),
    };
    await onSubmit(payload);
  };

  const fieldClass =
    'w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all';

  return (
    <form onSubmit={handleSubmit(submit)} className="space-y-5">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
        <input
          {...register('name', {
            required: 'Name is required',
            maxLength: { value: 100, message: 'Name must be 100 characters or less' },
          })}
          className={fieldClass}
          placeholder="e.g., Tomato"
          disabled={isSubmitting}
        />
        {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Unit</label>
        <input
          {...register('unit', {
            required: 'Unit is required',
            maxLength: { value: 20, message: 'Unit must be 20 characters or less' },
          })}
          className={fieldClass}
          placeholder="e.g., g, ml, piece"
          disabled={isSubmitting}
        />
        {errors.unit && <p className="text-red-400 text-xs mt-1">{errors.unit.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Calories per unit</label>
        <input
          {...register('caloriesPerUnit', {
            required: 'Calories per unit is required',
            valueAsNumber: true,
            validate: (v) => (v > 0) || 'Must be greater than 0',
          })}
          type="number"
          step="0.01"
          className={fieldClass}
          placeholder="e.g., 0.18"
          disabled={isSubmitting}
        />
        {errors.caloriesPerUnit && (
          <p className="text-red-400 text-xs mt-1">{errors.caloriesPerUnit.message}</p>
        )}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Protein (g)</label>
          <input
            {...register('protein', {
              valueAsNumber: true,
              validate: (v) => v == null || isNaN(v as number) || (v as number) >= 0 || 'Must be ≥ 0',
            })}
            type="number"
            step="0.01"
            className={fieldClass}
            placeholder="optional"
            disabled={isSubmitting}
          />
          {errors.protein && <p className="text-red-400 text-xs mt-1">{errors.protein.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Carbs (g)</label>
          <input
            {...register('carbs', {
              valueAsNumber: true,
              validate: (v) => v == null || isNaN(v as number) || (v as number) >= 0 || 'Must be ≥ 0',
            })}
            type="number"
            step="0.01"
            className={fieldClass}
            placeholder="optional"
            disabled={isSubmitting}
          />
          {errors.carbs && <p className="text-red-400 text-xs mt-1">{errors.carbs.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Fat (g)</label>
          <input
            {...register('fat', {
              valueAsNumber: true,
              validate: (v) => v == null || isNaN(v as number) || (v as number) >= 0 || 'Must be ≥ 0',
            })}
            type="number"
            step="0.01"
            className={fieldClass}
            placeholder="optional"
            disabled={isSubmitting}
          />
          {errors.fat && <p className="text-red-400 text-xs mt-1">{errors.fat.message}</p>}
        </div>
      </div>

      <div className="flex space-x-3 pt-4">
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmitting}
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isSubmitting}
          className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
        >
          {isSubmitting ? (
            <span className="flex items-center justify-center">
              <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2" />
              Saving…
            </span>
          ) : (
            submitLabel ?? (initialData ? 'Update Ingredient' : 'Create Ingredient')
          )}
        </button>
      </div>
    </form>
  );
};
