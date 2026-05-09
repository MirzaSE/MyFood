import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
    initialData?: Ingredient | null;
    onSubmit: (data: IngredientCreateDto) => void;
    onCancel: () => void;
    isLoading: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
    initialData,
    onSubmit,
    onCancel,
    isLoading
}) => {
    const { register, handleSubmit, reset, formState: { errors } } = useForm<IngredientCreateDto>();

    useEffect(() => {
        if (initialData) {
            reset({
                name: initialData.name,
                unit: initialData.unit,
                caloriesPerUnit: initialData.caloriesPerUnit,
                protein: initialData.protein,
                carbs: initialData.carbs,
                fat: initialData.fat,
                quantity: initialData.quantity,
                foodId: initialData.foodId
            });
        } else {
            reset({ 
                name: '', 
                unit: 'g',
                caloriesPerUnit: 0,
                protein: 0,
                carbs: 0,
                fat: 0,
                quantity: 0, 
                foodId: 1 
            });
        }
    }, [initialData, reset]);

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            {/* Name - Required */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                    Name <span className="text-red-400">*</span>
                </label>
                <input
                    {...register('name', { required: 'Name is required' })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                />
                {errors.name && <p className="text-red-400 text-sm mt-1">{errors.name.message}</p>}
            </div>

            {/* Unit - Required */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                    Unit <span className="text-red-400">*</span>
                </label>
                <select
                    {...register('unit', { required: 'Unit is required' })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                >
                    <option value="g">g (grams)</option>
                    <option value="kg">kg (kilograms)</option>
                    <option value="ml">ml (milliliters)</option>
                    <option value="l">l (liters)</option>
                    <option value="cup">cup</option>
                    <option value="tbsp">tbsp (tablespoon)</option>
                    <option value="tsp">tsp (teaspoon)</option>
                    <option value="piece">piece</option>
                </select>
                {errors.unit && <p className="text-red-400 text-sm mt-1">{errors.unit.message}</p>}
            </div>

            {/* Calories per Unit - Required, > 0 */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                    Calories per Unit <span className="text-red-400">*</span>
                </label>
                <input
                    type="number"
                    step="0.1"
                    {...register('caloriesPerUnit', { 
                        required: 'Calories per unit is required',
                        valueAsNumber: true, 
                        min: { value: 0.01, message: 'Must be greater than 0' }
                    })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                />
                {errors.caloriesPerUnit && <p className="text-red-400 text-sm mt-1">{errors.caloriesPerUnit.message}</p>}
            </div>

            {/* Protein - Required, > 0 */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                    Protein (g) <span className="text-red-400">*</span>
                </label>
                <input
                    type="number"
                    step="0.1"
                    {...register('protein', { 
                        required: 'Protein is required',
                        valueAsNumber: true, 
                        min: { value: 0.01, message: 'Must be greater than 0' }
                    })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                />
                {errors.protein && <p className="text-red-400 text-sm mt-1">{errors.protein.message}</p>}
            </div>

            {/* Carbs - Required, > 0 */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                    Carbs (g) <span className="text-red-400">*</span>
                </label>
                <input
                    type="number"
                    step="0.1"
                    {...register('carbs', { 
                        required: 'Carbs is required',
                        valueAsNumber: true, 
                        min: { value: 0.01, message: 'Must be greater than 0' }
                    })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                />
                {errors.carbs && <p className="text-red-400 text-sm mt-1">{errors.carbs.message}</p>}
            </div>

            {/* Fat - Required, > 0 */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                    Fat (g) <span className="text-red-400">*</span>
                </label>
                <input
                    type="number"
                    step="0.1"
                    {...register('fat', { 
                        required: 'Fat is required',
                        valueAsNumber: true, 
                        min: { value: 0.01, message: 'Must be greater than 0' }
                    })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                />
                {errors.fat && <p className="text-red-400 text-sm mt-1">{errors.fat.message}</p>}
            </div>

            {/* Quantity - Optional, default 0 */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">Quantity</label>
                <input
                    type="number"
                    {...register('quantity', { valueAsNumber: true, min: 0 })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                />
            </div>

            {/* Food ID - Required */}
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">
                    Food ID <span className="text-red-400">*</span>
                </label>
                <input
                    type="number"
                    {...register('foodId', { required: 'Food ID is required', valueAsNumber: true })}
                    className="w-full px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                />
                {errors.foodId && <p className="text-red-400 text-sm mt-1">{errors.foodId.message}</p>}
            </div>

            {/* Buttons */}
            <div className="flex justify-end space-x-3 pt-4">
                <button
                    type="button"
                    onClick={onCancel}
                    className="px-4 py-2 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded-lg transition"
                >
                    Cancel
                </button>
                <button
                    type="submit"
                    disabled={isLoading}
                    className="px-6 py-2 bg-gradient-to-r from-purple-500 to-blue-500 text-white rounded-lg hover:from-purple-600 hover:to-blue-600 disabled:opacity-50"
                >
                    {isLoading ? 'Saving...' : initialData ? 'Update' : 'Create'}
                </button>
            </div>
        </form>
    );
};