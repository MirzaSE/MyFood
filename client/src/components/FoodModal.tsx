import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { X } from 'lucide-react';
import type { Food, FoodCreateDto } from '../types';
import { FoodIngredientsPicker } from './FoodIngredientsPicker';

interface SelectedIngredient {
    ingredientId: number;
    name: string;
    unit: string;
    caloriesPerUnit: number;
    protein: number;
    carbs: number;
    fat: number;
    quantity: number;
    totalCalories: number;
    totalProtein: number;
    totalCarbs: number;
    totalFat: number;
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
    formState: { errors },
  } = useForm<FoodCreateDto>();

  const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredient[]>([]);
  const [totalNutrition, setTotalNutrition] = useState({
    calories: 0,
    protein: 0,
    carbs: 0,
    fat: 0
  });

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
      setSelectedIngredients([]);
      setTotalNutrition({ calories: 0, protein: 0, carbs: 0, fat: 0 });
    }
  }, [isOpen, initialData, reset]);

  const handleClose = () => {
    reset();
    setSelectedIngredients([]);
    setTotalNutrition({ calories: 0, protein: 0, carbs: 0, fat: 0 });
    onClose();
  };

  const handleIngredientsChange = (ingredients: SelectedIngredient[], nutrition: {
    calories: number;
    protein: number;
    carbs: number;
    fat: number;
  }) => {
    setSelectedIngredients(ingredients);
    setTotalNutrition(nutrition);
  };

  const onSubmitForm = async (data: FoodCreateDto) => {
    try {
      const foodData = {
        name: data.name,
        type: data.type,
        calories: Math.round(totalNutrition.calories),  // Auto-calculated from ingredients
      };
      
      await onSubmit(foodData);
      reset();
      setSelectedIngredients([]);
      setTotalNutrition({ calories: 0, protein: 0, carbs: 0, fat: 0 });
      onClose();
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4 overflow-y-auto">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">

        {/* Header */}
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center sticky top-0">
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

          {/* Name */}
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Name <span className="text-red-400">*</span>
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
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Type <span className="text-red-400">*</span>
            </label>
            <select
              {...register('type', { required: 'Food type is required' })}
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
              disabled={isLoading}
            >
              <option value="">Select type</option>
              <option value="Starter">Starter</option>
              <option value="Main">Main</option>
              <option value="Dessert">Dessert</option>
            </select>
            {errors.type && (
              <span className="text-red-400 text-xs">{errors.type.message}</span>
            )}
          </div>

          {/* Total Calories - Auto-calculated */}
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Total Calories
            </label>
            <input
              type="number"
              value={Math.round(totalNutrition.calories)}
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white font-semibold"
              disabled
              readOnly
            />
            <p className="text-xs text-gray-400 mt-1">
              Auto-calculated from selected ingredients
            </p>
          </div>

          {/* Ingredients Picker */}
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Ingredients
            </label>
            <FoodIngredientsPicker onIngredientsChange={handleIngredientsChange} />
          </div>

          {/* Nutrition Summary */}
          {selectedIngredients.length > 0 && (
            <div className="bg-gradient-to-r from-purple-500/20 to-blue-500/20 rounded-lg p-4 border border-white/10">
              <h4 className="text-sm font-semibold text-white mb-2">Nutrition Summary</h4>
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
                <div>
                  <p className="text-xs text-gray-400">Calories</p>
                  <p className="text-lg font-bold text-yellow-400">{Math.round(totalNutrition.calories)}</p>
                </div>
                <div>
                  <p className="text-xs text-gray-400">Protein</p>
                  <p className="text-lg font-bold text-green-400">{Math.round(totalNutrition.protein)}g</p>
                </div>
                <div>
                  <p className="text-xs text-gray-400">Carbs</p>
                  <p className="text-lg font-bold text-blue-400">{Math.round(totalNutrition.carbs)}g</p>
                </div>
                <div>
                  <p className="text-xs text-gray-400">Fat</p>
                  <p className="text-lg font-bold text-orange-400">{Math.round(totalNutrition.fat)}g</p>
                </div>
              </div>
            </div>
          )}

          {/* Buttons */}
          <div className="flex space-x-3 pt-6">
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 px-4 py-2 border border-white/20 text-gray-300 rounded-lg hover:bg-white/5 transition"
              disabled={isLoading}
            >
              Cancel
            </button>

            <button
              type="submit"
              disabled={selectedIngredients.length === 0 || isLoading}
              className="flex-1 px-4 py-2 bg-gradient-to-r from-purple-500 to-blue-500 text-white rounded-lg hover:from-purple-600 hover:to-blue-600 transition disabled:opacity-50 disabled:cursor-not-allowed"
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