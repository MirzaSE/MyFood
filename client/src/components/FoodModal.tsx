import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { X } from 'lucide-react';
import { FoodIngredientsPicker } from './FoodIngredientsPicker';
import { ingredientService } from '../services/ingredientService';
import type {
  Food,
  FoodCreateDto,
  FoodIngredientSelection,
  Ingredient,
} from '../types';

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
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = useState<FoodIngredientSelection[]>([]);
  const [ingredientError, setIngredientError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<FoodCreateDto>();

  useEffect(() => {
    if (isOpen) {
      reset({
        name: initialData?.name || '',
        type: initialData?.type || '',
        calories: initialData?.calories ?? undefined,
      });

      setSelectedIngredients([]);
      loadIngredients();
    }
  }, [isOpen, initialData, reset]);

  useEffect(() => {
    const calculatedCalories = selectedIngredients.reduce(
      (total, item) => total + item.quantity * item.ingredient.caloriesPerUnit,
      0
    );

    if (calculatedCalories > 0) {
      setValue('calories', Number(calculatedCalories.toFixed(2)));
    }
  }, [selectedIngredients, setValue]);

  const loadIngredients = async () => {
    try {
      setIngredientError(null);
      const data = await ingredientService.getAllIngredients();
      setIngredients(data);
    } catch {
      setIngredientError('Could not load ingredients. You can still create food manually.');
    }
  };

  const handleClose = () => {
    reset({
      name: '',
      type: '',
      calories: undefined as unknown as number,
    });
    setSelectedIngredients([]);
    onClose();
  };

  const onSubmitForm = async (data: FoodCreateDto) => {
    await onSubmit(data);

    reset({
      name: '',
      type: '',
      calories: undefined as unknown as number,
    });

    setSelectedIngredients([]);
  };

  if (!isOpen) return null;

  const totalProtein = selectedIngredients.reduce(
    (total, item) => total + item.quantity * item.ingredient.protein,
    0
  );

  const totalCarbs = selectedIngredients.reduce(
    (total, item) => total + item.quantity * item.ingredient.carbs,
    0
  );

  const totalFat = selectedIngredients.reduce(
    (total, item) => total + item.quantity * item.ingredient.fat,
    0
  );

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
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
          <div className="mt-2">
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
              step="0.01"
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

          {ingredientError && (
            <div className="mt-6 p-3 bg-yellow-500/20 border border-yellow-500/40 rounded-lg text-yellow-200 text-sm">
              {ingredientError}
            </div>
          )}

          <FoodIngredientsPicker
            ingredients={ingredients}
            selectedIngredients={selectedIngredients}
            onChange={setSelectedIngredients}
          />

          <div className="mt-6 p-5 bg-white/5 border border-white/10 rounded-xl">
            <h3 className="text-lg font-bold text-white mb-4">
              Calculated Nutrition
            </h3>

            <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
              <div className="p-3 bg-white/10 rounded-lg">
                <p className="text-gray-400 text-xs">Protein</p>
                <p className="text-white font-bold">{totalProtein.toFixed(2)} g</p>
              </div>

              <div className="p-3 bg-white/10 rounded-lg">
                <p className="text-gray-400 text-xs">Carbs</p>
                <p className="text-white font-bold">{totalCarbs.toFixed(2)} g</p>
              </div>

              <div className="p-3 bg-white/10 rounded-lg">
                <p className="text-gray-400 text-xs">Fat</p>
                <p className="text-white font-bold">{totalFat.toFixed(2)} g</p>
              </div>
            </div>
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