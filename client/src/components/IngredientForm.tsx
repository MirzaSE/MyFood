import React from 'react';
import { useForm } from 'react-hook-form';
import type { IngredientCreateDto } from '../types/ingredient';

interface Props { initial?: Partial<IngredientCreateDto>; onSubmit: (v: IngredientCreateDto) => Promise<void>; onCancel: () => void; loading?: boolean; }

export const IngredientForm: React.FC<Props> = ({ initial, onSubmit, onCancel, loading }) => {
  const { register, handleSubmit, formState: { errors } } = useForm<IngredientCreateDto>({ defaultValues: { foodEntityId: 1, name: '', unit: '', caloriesPerUnit: 1, protein: 1, carbs: 1, fat: 1, ...initial } });
  return <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
    <input {...register('name', { required: 'Name required' })} placeholder="Name" className="w-full p-2 rounded bg-slate-800 text-white" />
    {errors.name && <p className="text-red-400 text-xs">{errors.name.message}</p>}
    <input {...register('unit', { required: 'Unit required' })} placeholder="Unit" className="w-full p-2 rounded bg-slate-800 text-white" />
    <input {...register('caloriesPerUnit', { valueAsNumber: true, min: 0.01 })} type="number" step="0.01" placeholder="Calories" className="w-full p-2 rounded bg-slate-800 text-white" />
    <input {...register('protein', { valueAsNumber: true, min: 0.01 })} type="number" step="0.01" placeholder="Protein" className="w-full p-2 rounded bg-slate-800 text-white" />
    <input {...register('carbs', { valueAsNumber: true, min: 0.01 })} type="number" step="0.01" placeholder="Carbs" className="w-full p-2 rounded bg-slate-800 text-white" />
    <input {...register('fat', { valueAsNumber: true, min: 0.01 })} type="number" step="0.01" placeholder="Fat" className="w-full p-2 rounded bg-slate-800 text-white" />
    <input {...register('foodEntityId', { valueAsNumber: true, min: 1 })} type="number" placeholder="Food ID" className="w-full p-2 rounded bg-slate-800 text-white" />
    <div className="flex gap-2"><button type="button" onClick={onCancel} className="px-3 py-2 rounded border border-slate-600 text-white">Cancel</button><button disabled={loading} className="px-3 py-2 rounded bg-blue-600 text-white">{loading ? 'Saving...' : 'Submit'}</button></div>
  </form>;
};
