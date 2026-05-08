import React from 'react';
import { useForm } from 'react-hook-form';
import type { Food, Ingredient, IngredientCreateDto } from '../types';

export type IngredientFormValues = {
  name: string;
  unit: string;
  caloriesPerUnit: number;
  protein: number;
  carbs: number;
  fat: number;
  foodEntityId: number;
};

interface IngredientFormProps {
  foods: Food[];
  initialData?: Ingredient | null;
  isLoading?: boolean;
  onSubmit: (dto: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  foods,
  initialData,
  isLoading = false,
  onSubmit,
  onCancel,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<IngredientFormValues>({
    defaultValues: {
      name: initialData?.name ?? '',
      unit: '',
      caloriesPerUnit: initialData?.quantity ?? 1,
      protein: 1,
      carbs: 1,
      fat: 1,
      foodEntityId: initialData?.foodEntityId ?? foods[0]?.id ?? 0,
    },
  });

  const submit = async (values: IngredientFormValues) => {
    // Backend currently stores ingredient quantity as integer; we map caloriesPerUnit to quantity.
    await onSubmit({
      name: values.name.trim(),
      quantity: Math.round(values.caloriesPerUnit),
      foodEntityId: values.foodEntityId,
    });
  };

  return (
    <form onSubmit={handleSubmit(submit)} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">Name</label>
        <input
          {...register('name', { required: 'Name is required' })}
          className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
          disabled={isLoading}
        />
        {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">Unit</label>
        <input
          {...register('unit', { required: 'Unit is required' })}
          placeholder="g, ml, tbsp..."
          className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
          disabled={isLoading}
        />
        {errors.unit && <p className="text-red-400 text-xs mt-1">{errors.unit.message}</p>}
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Calories / Unit</label>
          <input
            type="number"
            {...register('caloriesPerUnit', {
              required: 'Calories is required',
              valueAsNumber: true,
              min: { value: 1, message: 'Must be > 0' },
            })}
            className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            disabled={isLoading}
          />
          {errors.caloriesPerUnit && <p className="text-red-400 text-xs mt-1">{errors.caloriesPerUnit.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Food</label>
          <select
            {...register('foodEntityId', {
              required: 'Food is required',
              valueAsNumber: true,
              validate: (v) => v > 0 || 'Food is required',
            })}
            className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            disabled={isLoading}
          >
            {foods.map((food) => (
              <option key={food.id} value={food.id} className="bg-slate-900">
                {food.name}
              </option>
            ))}
          </select>
          {errors.foodEntityId && <p className="text-red-400 text-xs mt-1">{errors.foodEntityId.message}</p>}
        </div>
      </div>

      <div className="grid grid-cols-3 gap-3">
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Protein</label>
          <input
            type="number"
            {...register('protein', { valueAsNumber: true, min: { value: 1, message: '> 0' } })}
            className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            disabled={isLoading}
          />
          {errors.protein && <p className="text-red-400 text-xs mt-1">{errors.protein.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Carbs</label>
          <input
            type="number"
            {...register('carbs', { valueAsNumber: true, min: { value: 1, message: '> 0' } })}
            className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            disabled={isLoading}
          />
          {errors.carbs && <p className="text-red-400 text-xs mt-1">{errors.carbs.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">Fat</label>
          <input
            type="number"
            {...register('fat', { valueAsNumber: true, min: { value: 1, message: '> 0' } })}
            className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            disabled={isLoading}
          />
          {errors.fat && <p className="text-red-400 text-xs mt-1">{errors.fat.message}</p>}
        </div>
      </div>

      <p className="text-xs text-gray-400">
        Note: current backend stores only name, quantity and linked food. Quantity is mapped from calories/unit.
      </p>

      <div className="flex gap-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-4 py-2 rounded-lg border border-white/20 text-gray-200"
          disabled={isLoading}
        >
          Cancel
        </button>
        <button
          type="submit"
          className="flex-1 px-4 py-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 text-white"
          disabled={isLoading}
        >
          {isLoading ? 'Saving...' : initialData ? 'Update Ingredient' : 'Create Ingredient'}
        </button>
      </div>
    </form>
  );
};
