import React from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto, IngredientFormValues } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto | IngredientUpdateDto) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isLoading = false,
}) => {
  const { register, handleSubmit, reset, formState: { errors } } = useForm<IngredientFormValues>({
    defaultValues: {
      name: initialData?.name ?? '',
      unit: initialData?.unit ?? '',
      caloriesPerUnit: initialData?.caloriesPerUnit ?? 0,
      protein: initialData?.protein ?? 0,
      carbs: initialData?.carbs ?? 0,
      fat: initialData?.fat ?? 0,
    },
  });

  const submit = async (data: IngredientFormValues) => {
    await onSubmit({
      name: data.name,
      unit: data.unit,
      caloriesPerUnit: data.caloriesPerUnit,
      protein: data.protein,
      carbs: data.carbs,
      fat: data.fat,
    });
    reset(data);
  };

  return (
    <form onSubmit={handleSubmit(submit)} className="space-y-4 p-6">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Ingredient Name</label>
        <input
          {...register('name', { required: 'Name is required' })}
          disabled={isLoading}
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-cyan-400"
          placeholder="e.g. Chicken Breast"
        />
        {errors.name && <p className="mt-1 text-xs text-red-400">{errors.name.message}</p>}
      </div>

      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Unit</label>
        <input
          {...register('unit')}
          disabled={isLoading}
          className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-cyan-400"
          placeholder="g, ml, piece"
        />
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Calories</label>
          <input {...register('caloriesPerUnit', { valueAsNumber: true })} type="number" step="0.01" disabled={isLoading} className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-cyan-400" />
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Protein</label>
          <input {...register('protein', { valueAsNumber: true })} type="number" step="0.01" disabled={isLoading} className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-cyan-400" />
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Carbs</label>
          <input {...register('carbs', { valueAsNumber: true })} type="number" step="0.01" disabled={isLoading} className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-cyan-400" />
        </div>
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">Fat</label>
          <input {...register('fat', { valueAsNumber: true })} type="number" step="0.01" disabled={isLoading} className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-cyan-400" />
        </div>
      </div>

      <div className="flex gap-3 pt-2">
        <button type="button" onClick={onCancel} disabled={isLoading} className="flex-1 px-4 py-3 border border-white/20 rounded-lg text-gray-300 hover:text-white">Cancel</button>
        <button type="submit" disabled={isLoading} className="flex-1 px-4 py-3 rounded-lg bg-gradient-to-r from-cyan-500 to-blue-500 text-white font-semibold">{initialData ? 'Update Ingredient' : 'Create Ingredient'}</button>
      </div>
    </form>
  );
};
