import React, { useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { X, AlertCircle } from 'lucide-react';
import type { Food, FoodCreateDto, Ingredient } from '../types';
import { ingredientService } from '../services/ingredientService';
import { FoodIngredientsPicker } from './FoodIngredientsPicker';
import type { SelectedFoodIngredient } from './FoodIngredientsPicker';

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
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedFoodIngredient[]>([]);
  const [pickerError, setPickerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<FoodCreateDto>({
    defaultValues: initialData
      ? { name: initialData.name, type: initialData.type, calories: initialData.calories }
      : { name: '', type: '', calories: 0 },
  });

  useEffect(() => {
    if (!isOpen) {
      setSelectedIngredients([]);
      return;
    }

    reset(
      initialData
        ? { name: initialData.name, type: initialData.type, calories: initialData.calories }
        : { name: '', type: '', calories: 0 }
    );

    const loadIngredients = async () => {
      try {
        setPickerError(null);
        const result = await ingredientService.getAllIngredients({ page: 1, pageCount: 50 });
        setIngredients(result.items);
      } catch {
        setPickerError('Unable to load ingredients for the picker.');
      }
    };
    void loadIngredients();
  }, [isOpen, initialData, reset]);

  const calculatedCalories = useMemo(
    () =>
      selectedIngredients.reduce((sum, entry) => {
        const ing = ingredients.find((i) => i.id === entry.ingredientId);
        return sum + Number(ing?.caloriesPerUnit ?? 0) * entry.quantity;
      }, 0),
    [ingredients, selectedIngredients]
  );

  useEffect(() => {
    if (selectedIngredients.length > 0) {
      setValue('calories', Math.round(calculatedCalories));
    }
  }, [calculatedCalories, selectedIngredients.length, setValue]);

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
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full overflow-hidden">
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

        <form onSubmit={handleSubmit(onSubmitForm)} className="p-8 max-h-[80vh] overflow-y-auto space-y-6">
          {pickerError && (
            <div className="p-3 bg-yellow-500/20 border border-yellow-500/50 rounded-lg flex items-start space-x-2">
              <AlertCircle size={18} className="text-yellow-400 flex-shrink-0 mt-0.5" />
              <p className="text-yellow-200 text-sm">{pickerError}</p>
            </div>
          )}

          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Food Name</label>
            <input
              {...register('name', {
                required: 'Name is required',
                minLength: { value: 2, message: 'Name must be at least 2 characters' },
                maxLength: { value: 100, message: 'Name must be at most 100 characters' },
              })}
              type="text"
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., Grilled Chicken"
              disabled={isLoading}
            />
            {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Food Type</label>
            <input
              {...register('type', {
                required: 'Type is required',
                minLength: { value: 2, message: 'Type must be at least 2 characters' },
              })}
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
              {selectedIngredients.length > 0 && (
                <span className="ml-2 text-xs font-normal text-purple-300">(auto-filled from ingredients)</span>
              )}
            </label>
            <input
              {...register('calories', {
                required: 'Calories is required',
                valueAsNumber: true,
                min: { value: 0, message: 'Calories cannot be negative' },
                max: { value: 10000, message: 'Calories cannot exceed 10,000' },
              })}
              type="number"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., 250"
              disabled={isLoading}
            />
            {errors.calories && <span className="text-red-400 text-xs mt-1 block">{errors.calories.message}</span>}
          </div>

          <FoodIngredientsPicker
            ingredients={ingredients}
            value={selectedIngredients}
            onChange={setSelectedIngredients}
            disabled={isLoading}
          />

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
