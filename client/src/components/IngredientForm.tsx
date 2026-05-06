import React from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({ initialData, onSubmit, onCancel, isLoading = false }) => {
  const { register, handleSubmit, formState: { errors } } = useForm<IngredientCreateDto>({
    defaultValues: initialData ? {
      name: initialData.name,
      unit: initialData.unit,
      caloriesPerUnit: initialData.caloriesPerUnit,
      protein: initialData.protein,
      carbs: initialData.carbs,
      fat: initialData.fat,
    } : {
      name: '',
      unit: '',
      caloriesPerUnit: 0,
      protein: 0,
      carbs: 0,
      fat: 0,
    },
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <input {...register('name', { required: 'Name is required' })} placeholder="Name" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
      {errors.name && <p className="text-red-400 text-xs">{errors.name.message}</p>}
      <input {...register('unit', { required: 'Unit is required' })} placeholder="Unit (e.g. g, ml)" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
      {errors.unit && <p className="text-red-400 text-xs">{errors.unit.message}</p>}
      <input type="number" step="0.01" {...register('caloriesPerUnit', { valueAsNumber: true, min: { value: 0.0001, message: 'Must be greater than 0' } })} placeholder="Calories per unit" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
      <input type="number" step="0.01" {...register('protein', { valueAsNumber: true, min: 0 })} placeholder="Protein" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
      <input type="number" step="0.01" {...register('carbs', { valueAsNumber: true, min: 0 })} placeholder="Carbs" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
      <input type="number" step="0.01" {...register('fat', { valueAsNumber: true, min: 0 })} placeholder="Fat" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
      <div className="flex gap-2">
        <button type="submit" disabled={isLoading} className="px-4 py-2 rounded bg-blue-600 text-white">{isLoading ? 'Saving...' : 'Submit'}</button>
        <button type="button" onClick={onCancel} disabled={isLoading} className="px-4 py-2 rounded bg-slate-600 text-white">Cancel</button>
      </div>
    </form>
  );
};
