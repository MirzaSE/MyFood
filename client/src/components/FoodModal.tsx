import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { X } from 'lucide-react';
import type { Food, FoodCreateDto } from '../types';
import type { Ingredient } from '../types/ingredient';
import { FoodIngredientsPicker } from './FoodIngredientsPicker';
import { ingredientService } from '../services/ingredientService';

interface SelectedIngredient {
  ingredient: Ingredient;
  quantity: number;
}

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
    setValue,
    formState: { errors },
  } = useForm<FoodCreateDto>();

  const [availableIngredients, setAvailableIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredient[]>([]);

  useEffect(() => {
    if (isOpen) {
      reset(initialData
        ? { name: initialData.name, type: initialData.type, calories: initialData.calories }
        : { name: '', type: '', calories: undefined }
      );
      setSelectedIngredients([]);
      ingredientService.getAllIngredients().then(setAvailableIngredients).catch(() => {});
    }
  }, [isOpen, initialData, reset]);

  const totalCalories = selectedIngredients.reduce(
    (sum, si) => sum + si.ingredient.caloriesPerUnit * si.quantity,
    0
  );
  const totalProtein = selectedIngredients.reduce(
    (sum, si) => sum + si.ingredient.protein * si.quantity,
    0
  );
  const totalCarbs = selectedIngredients.reduce(
    (sum, si) => sum + si.ingredient.carbs * si.quantity,
    0
  );
  const totalFat = selectedIngredients.reduce(
    (sum, si) => sum + si.ingredient.fat * si.quantity,
    0
  );

  const handleAddIngredient = (ingredient: Ingredient, quantity: number) => {
    const updated = [...selectedIngredients, { ingredient, quantity }];
    setSelectedIngredients(updated);
    const newTotal = updated.reduce((s, si) => s + si.ingredient.caloriesPerUnit * si.quantity, 0);
    setValue('calories', newTotal);
  };

  const handleRemoveIngredient = (ingredientId: number) => {
    const updated = selectedIngredients.filter(si => si.ingredient.id !== ingredientId);
    setSelectedIngredients(updated);
    const newTotal = updated.reduce((s, si) => s + si.ingredient.caloriesPerUnit * si.quantity, 0);
    setValue('calories', newTotal);
  };

  const handleClose = () => {
    reset();
    setSelectedIngredients([]);
    onClose();
  };

  const onSubmitForm = async (data: FoodCreateDto) => {
    try {
      await onSubmit(data);
      reset();
      setSelectedIngredients([]);
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full overflow-hidden max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center sticky top-0 z-10">
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

        <form onSubmit={handleSubmit(onSubmitForm)} className="food-form p-8 space-y-6">
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Name
            </label>
            <input
              {...register('name', {
                required: 'Name is required',
                minLength: { value: 2, message: 'Name must be at least 2 characters' },
                maxLength: { value: 100, message: 'Name must be at most 100 characters' },
              })}
              type="text"
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-base transition-all"
              placeholder="e.g., Grilled Chicken"
              disabled={isLoading}
            />
            {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Type
            </label>
            <input
              {...register('type', { required: 'Type is required' })}
              type="text"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., Protein, Vegetable"
              disabled={isLoading}
            />
            {errors.type && <span className="text-red-400 text-xs mt-1 block">{errors.type.message}</span>}
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Calories
            </label>
            <input
              {...register('calories', {
                required: 'Calories is required',
                valueAsNumber: true,
                min: { value: 0, message: 'Calories cannot be negative' },
                max: { value: 10000, message: 'Calories cannot exceed 10000' },
              })}
              type="number"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., 250"
              disabled={isLoading}
            />
            {errors.calories && <span className="text-red-400 text-xs mt-1 block">{errors.calories.message}</span>}
          </div>

          {/* Ingredients Picker */}
          <div className="border-t border-white/10 pt-6">
            <FoodIngredientsPicker
              availableIngredients={availableIngredients}
              selectedIngredients={selectedIngredients}
              onAdd={handleAddIngredient}
              onRemove={handleRemoveIngredient}
            />
          </div>

          {/* Calculated Nutrition Summary */}
          {selectedIngredients.length > 0 && (
            <div className="bg-white/5 border border-white/10 rounded-lg p-4 space-y-2">
              <h4 className="text-sm font-semibold text-gray-300 uppercase tracking-wider">Calculated Nutrition</h4>
              <div className="grid grid-cols-4 gap-3 text-center">
                <div>
                  <p className="text-lg font-bold text-white">{totalCalories}</p>
                  <p className="text-xs text-gray-400">Calories</p>
                </div>
                <div>
                  <p className="text-lg font-bold text-blue-400">{totalProtein.toFixed(1)}g</p>
                  <p className="text-xs text-gray-400">Protein</p>
                </div>
                <div>
                  <p className="text-lg font-bold text-yellow-400">{totalCarbs.toFixed(1)}g</p>
                  <p className="text-xs text-gray-400">Carbs</p>
                </div>
                <div>
                  <p className="text-lg font-bold text-red-400">{totalFat.toFixed(1)}g</p>
                  <p className="text-xs text-gray-400">Fat</p>
                </div>
              </div>
            </div>
          )}

          <div className="flex space-x-3 pt-2">
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
              {isLoading ? (
                <span className="flex items-center justify-center">
                  <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
                  Saving...
                </span>
              ) : (
                initialData ? 'Update Food' : 'Create Food'
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
