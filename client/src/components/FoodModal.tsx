import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { X } from 'lucide-react';
import type { Food, FoodCreateDto } from '../types';

interface FoodModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: FoodCreateDto) => Promise<void>;
  initialData?: Food | null;
  isLoading?: boolean;
}

export const FoodModal: React.FC<FoodModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FoodCreateDto>();

  // ✅ FIX: Properly populate form when editing
  useEffect(() => {
    if (isOpen && initialData) {
      reset({
        name: initialData.name,
        type: initialData.type,
        calories: initialData.calories,
      });
    } else if (isOpen && !initialData) {
      reset({
        name: '',
        type: '',
        calories: undefined,
      });
    }
  }, [isOpen, initialData, reset]);

  const handleClose = () => {
    reset();
    onClose();
  };

  const onSubmitForm = async (data: FoodCreateDto) => {
    try {
      await onSubmit(data);
      reset();
      onClose();
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-md w-full overflow-hidden">

        {/* Header */}
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Food' : 'Add New Food'}
          </h2>
          <button
            onClick={handleClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmitForm)} className="food-form p-8">

          {/* Name */}
          <div className="mt-4">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Name
            </label>
            <input
              {...register('name', {
                required: 'Food name is required',
                minLength: { value: 2, message: 'Minimum 2 characters' },
                maxLength: { value: 100, message: 'Max 100 characters' },
              })}
              type="text"
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
              disabled={isLoading}
            />
            {errors.name && (
              <span className="text-red-400 text-xs">{errors.name.message}</span>
            )}
          </div>

          {/* Type */}
          <div className="mt-4">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Type
            </label>
            <select
              {...register('type', { required: 'Food type is required' })}
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
              disabled={isLoading}
            >
              <option value="">Select type</option>
              <option value="Protein">Protein</option>
              <option value="Vegetable">Vegetable</option>
              <option value="Fruit">Fruit</option>
              <option value="Carbohydrate">Carbohydrate</option>
              <option value="Dairy">Dairy</option>
              <option value="Snack">Snack</option>
              <option value="Dessert">Dessert</option>
            </select>
            {errors.type && (
              <span className="text-red-400 text-xs">{errors.type.message}</span>
            )}
          </div>

          {/* Calories */}
          <div className="mt-4">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Calories
            </label>
            <input
              {...register('calories', {
                required: 'Calories required',
                valueAsNumber: true,
                min: { value: 0, message: 'Cannot be negative' },
                max: { value: 5000, message: 'Max 5000' },
              })}
              type="number"
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
              disabled={isLoading}
            />
            {errors.calories && (
              <span className="text-red-400 text-xs">{errors.calories.message}</span>
            )}
          </div>

          {/* Buttons */}
          <div className="flex space-x-3 pt-6">
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 px-4 py-2 border border-white/20 text-gray-300 rounded-lg"
              disabled={isLoading}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="flex-1 px-4 py-2 bg-purple-600 text-white rounded-lg"
              disabled={isLoading}
            >
              {isLoading
                ? 'Saving...'
                : initialData
                ? 'Update Food'
                : 'Create Food'}
            </button>
          </div>

        </form>
      </div>
    </div>
  );
};