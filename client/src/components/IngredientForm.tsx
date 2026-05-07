import React from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientFormValues } from '../types/ingredient';

export type IngredientFormMode = 'create' | 'edit' | 'view';

interface IngredientFormProps {
  mode: IngredientFormMode;
  defaultValues?: Partial<IngredientFormValues>;
  onSubmit: (data: IngredientFormValues) => Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
}

const emptyDefaults: IngredientFormValues = {
  name: '',
  unit: 'g',
  caloriesPerUnit: 1,
  protein: 0.1,
  carbs: 0.1,
  fat: 0.1,
  foodEntityId: null,
};

export const IngredientForm: React.FC<IngredientFormProps> = ({
  mode,
  defaultValues,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const readOnly = mode === 'view';

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IngredientFormValues>({
    defaultValues: { ...emptyDefaults, ...defaultValues },
  });

  return (
    <form
      onSubmit={readOnly ? (e) => e.preventDefault() : handleSubmit(onSubmit)}
      className="space-y-5"
    >
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
        <input
          {...register('name', { required: 'Name is required' })}
          type="text"
          readOnly={readOnly}
          disabled={isSubmitting}
          className="w-full px-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 text-white placeholder-gray-400 disabled:opacity-50"
          placeholder="e.g. Olive oil"
        />
        {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Unit</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          type="text"
          readOnly={readOnly}
          disabled={isSubmitting}
          className="w-full px-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 text-white placeholder-gray-400 disabled:opacity-50"
          placeholder="g, ml, piece..."
        />
        {errors.unit && <span className="text-red-400 text-xs mt-1 block">{errors.unit.message}</span>}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {(['caloriesPerUnit', 'protein', 'carbs', 'fat'] as const).map((field) => (
          <div key={field}>
            <label className="block text-sm font-semibold text-gray-300 mb-2 capitalize">
              {field === 'caloriesPerUnit' ? 'Calories / unit' : field}
            </label>
            <input
              {...register(field, {
                required: 'Required',
                valueAsNumber: true,
                min: { value: 0.0001, message: 'Must be greater than 0' },
              })}
              type="number"
              step="any"
              readOnly={readOnly}
              disabled={isSubmitting}
              className="w-full px-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 text-white disabled:opacity-50"
            />
            {errors[field] && (
              <span className="text-red-400 text-xs mt-1 block">{errors[field]?.message}</span>
            )}
          </div>
        ))}
      </div>

      <div className="flex gap-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmitting}
          className="flex-1 py-3 border border-white/20 text-gray-300 rounded-lg hover:border-white/40 disabled:opacity-50"
        >
          Cancel
        </button>
        {!readOnly && (
          <button
            type="submit"
            disabled={isSubmitting}
            className="flex-1 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg font-medium disabled:opacity-50"
          >
            {isSubmitting ? 'Saving…' : mode === 'edit' ? 'Save' : 'Create'}
          </button>
        )}
      </div>
    </form>
  );
};
