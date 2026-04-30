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

  useEffect(() => {
    if (isOpen) {
      reset({
        name: initialData?.name || '',
        type: initialData?.type || '',
        calories: initialData?.calories ?? undefined,
      });
    }
  }, [isOpen, initialData, reset]);

  const handleClose = () => {
    reset({
      name: '',
      type: '',
      calories: undefined as unknown as number,
    });
    onClose();
  };

  const onSubmitForm = async (data: FoodCreateDto) => {
    await onSubmit(data);
    reset({
      name: '',
      type: '',
      calories: undefined as unknown as number,
    });
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-md w-full overflow-hidden">
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Food' : 'Add New Food'}
          </h2>

          <button
            type="button"
            onClick={handleClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmitForm)} className="food-form p-8">
          <div className="mt-8">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Name
            </label>

            <input
              {...register('name', {
                required: 'Food name is required',
                minLength: {
                  value: 3,
                  message: 'Food name must be at least 3 characters',
                },
                maxLength: {
                  value: 100,
                  message: 'Food name cannot be longer than 100 characters',
                },
              })}
              type="text"
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-base transition-all"
              placeholder="e.g., Grilled Chicken"
              disabled={isLoading}
            />

            {errors.name && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors.name.message}
              </span>
            )}
          </div>

          <div className="mt-8">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Type
            </label>

            <input
              {...register('type', {
                required: 'Food type is required',
                minLength: {
                  value: 3,
                  message: 'Food type must be at least 3 characters',
                },
                maxLength: {
                  value: 100,
                  message: 'Food type cannot be longer than 100 characters',
                },
              })}
              type="text"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., Protein, Vegetable"
              disabled={isLoading}
            />

            {errors.type && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors.type.message}
              </span>
            )}
          </div>

          <div className="mt-8">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Calories
            </label>

            <input
              {...register('calories', {
                required: 'Calories are required',
                valueAsNumber: true,
                validate: (value) => {
                  if (Number.isNaN(value)) {
                    return 'Calories are required';
                  }

                  if (value <= 0) {
                    return 'Calories must be greater than 0';
                  }

                  if (value > 10000) {
                    return 'Calories cannot be greater than 10000';
                  }

                  return true;
                },
              })}
              type="number"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., 250"
              disabled={isLoading}
            />

            {errors.calories && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors.calories.message}
              </span>
            )}
          </div>

          <div className="flex space-x-3 pt-6">
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
              disabled={isLoading}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
              disabled={isLoading}
            >
              {isLoading ? 'Saving...' : initialData ? 'Update Food' : 'Create Food'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};