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
                quantity: initialData.quantity,
                foodId: initialData.foodId
            });
        } else {
            reset({ name: '', quantity: 0, foodId: 1 });
        }
    }, [initialData, reset]);

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">
                    Name <span className="text-red-400">*</span>
                </label>
                <input
                    {...register('name', { required: 'Name is required' })}
                    className="w-full px-4 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 focus:ring-1 focus:ring-purple-500 transition-all"
                    placeholder="Enter ingredient name"
                />
                {errors.name && (
                    <p className="text-red-400 text-sm mt-1">{errors.name.message}</p>
                )}
            </div>

            <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">
                    Quantity <span className="text-red-400">*</span>
                </label>
                <input
                    type="number"
                    {...register('quantity', { 
                        required: 'Quantity is required', 
                        valueAsNumber: true,
                        min: { value: 0, message: 'Quantity must be >= 0' }
                    })}
                    className="w-full px-4 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 focus:ring-1 focus:ring-purple-500 transition-all"
                    placeholder="Enter quantity"
                />
                {errors.quantity && (
                    <p className="text-red-400 text-sm mt-1">{errors.quantity.message}</p>
                )}
            </div>

            <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">
                    Food ID <span className="text-red-400">*</span>
                </label>
                <input
                    type="number"
                    {...register('foodId', { 
                        required: 'Food ID is required', 
                        valueAsNumber: true,
                        min: { value: 1, message: 'Food ID must be at least 1' }
                    })}
                    className="w-full px-4 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 focus:ring-1 focus:ring-purple-500 transition-all"
                    placeholder="Enter Food ID (e.g., 1, 2, 3...)"
                />
                {errors.foodId && (
                    <p className="text-red-400 text-sm mt-1">{errors.foodId.message}</p>
                )}
            </div>

            <div className="flex justify-end space-x-3 pt-4">
                <button
                    type="button"
                    onClick={onCancel}
                    className="px-4 py-2 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded-lg transition-all duration-200"
                >
                    Cancel
                </button>
                <button
                    type="submit"
                    disabled={isLoading}
                    className="px-6 py-2 bg-gradient-to-r from-purple-500 to-blue-500 text-white rounded-lg hover:from-purple-600 hover:to-blue-600 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg"
                >
                    {isLoading ? 'Saving...' : initialData ? 'Update' : 'Create'}
                </button>
            </div>
        </form>
    );
};