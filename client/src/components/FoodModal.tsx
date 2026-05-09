import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { X } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Food, FoodCreateDto, FoodIngredientInput, Ingredient } from '../types';
import { FoodIngredientsPicker, calculateNutritionTotals } from './FoodIngredientsPicker';

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
  const [availableIngredients, setAvailableIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = useState<FoodIngredientInput[]>([]);
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FoodCreateDto>({
    defaultValues: {
      name: '',
      type: '',
      calories: 0,
      ingredients: [],
    },
  });

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    void ingredientService.getAllIngredients({ page: 1, pageCount: 100 })
      .then((result) => setAvailableIngredients(result.items))
      .catch(() => setAvailableIngredients([]));

    const modalIngredients = initialData?.ingredients?.map((item) => ({
      ingredientId: item.ingredientId,
      quantity: item.quantity,
    })) ?? [];

    setSelectedIngredients(modalIngredients);

    reset(initialData ? {
      name: initialData.name,
      type: initialData.type,
      calories: initialData.calories,
      ingredients: modalIngredients,
    } : {
      name: '',
      type: '',
      calories: 0,
      ingredients: [],
    });
  }, [initialData, isOpen, reset]);

  const handleClose = () => {
    reset();
    setSelectedIngredients([]);
    onClose();
  };

  const nutritionTotals = calculateNutritionTotals(availableIngredients, selectedIngredients);

  const onSubmitForm = async (data: FoodCreateDto) => {
    await onSubmit({
      ...data,
      calories: Math.round(nutritionTotals.calories || data.calories),
      ingredients: selectedIngredients,
    });
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4 backdrop-blur-sm">
      <div className="max-h-[92vh] w-full max-w-3xl overflow-y-auto rounded-2xl border border-white/20 bg-gradient-to-br from-slate-800 to-slate-950 shadow-2xl">
        <div className="flex items-center justify-between bg-gradient-to-r from-amber-500 to-orange-500 px-6 py-6">
          <h2 className="text-xl font-bold text-slate-950">
            {initialData ? 'Edit Food' : 'Add New Food'}
          </h2>
          <button
            onClick={handleClose}
            className="text-slate-900/80 hover:text-slate-950 transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmitForm)} className="space-y-8 p-8">
          <div>
            <label className="mb-2 block text-sm font-semibold text-gray-300">Food Name</label>
            <input
              {...register('name', { required: 'Name is required' })}
              type="text"
              className="w-full rounded-lg border border-white/20 bg-white/10 px-3 py-3 text-white"
              disabled={isLoading}
            />
            {errors.name && <span className="mt-1 block text-xs text-red-400">{errors.name.message}</span>}
          </div>

          <div>
            <label className="mb-2 block text-sm font-semibold text-gray-300">Food Type</label>
            <input
              {...register('type', { required: 'Type is required' })}
              type="text"
              className="w-full rounded-lg border border-white/20 bg-white/10 px-4 py-3 text-white"
              disabled={isLoading}
            />
            {errors.type && <span className="mt-1 block text-xs text-red-400">{errors.type.message}</span>}
          </div>

          <section className="space-y-4">
            <div>
              <h3 className="text-lg font-bold text-white">Ingredients</h3>
              <p className="text-sm text-gray-400">Pick ingredients and quantities to update the nutrition summary.</p>
            </div>
            <FoodIngredientsPicker
              availableIngredients={availableIngredients}
              selectedIngredients={selectedIngredients}
              onChange={setSelectedIngredients}
              disabled={isLoading}
            />
          </section>

          <div>
            <label className="mb-2 block text-sm font-semibold text-gray-300">Calculated Calories</label>
            <input
              {...register('calories', { valueAsNumber: true })}
              type="number"
              readOnly
              value={Math.round(nutritionTotals.calories)}
              className="w-full rounded-lg border border-amber-500/30 bg-amber-500/10 px-4 py-3 text-amber-50"
            />
          </div>

          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 rounded-lg border border-white/20 px-4 py-3 text-gray-300 transition"
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="flex-1 rounded-lg bg-amber-500 px-4 py-3 font-medium text-slate-900 transition disabled:opacity-50"
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
