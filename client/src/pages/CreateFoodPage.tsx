import React, { useCallback, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import type { IngredientSelection, NutritionTotals } from '../types/ingredient';

type CreateFoodFormValues = {
  name: string;
  type: string;
};

const emptyTotals: NutritionTotals = {
  calories: 0,
  protein: 0,
  carbs: 0,
  fat: 0,
};

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [selectedIngredients, setSelectedIngredients] = useState<IngredientSelection[]>([]);
  const [totals, setTotals] = useState<NutritionTotals>(emptyTotals);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateFoodFormValues>();

  const handleTotalsChange = useCallback((nextTotals: NutritionTotals) => {
    setTotals(nextTotals);
  }, []);

  const onSubmit = async (values: CreateFoodFormValues) => {
    if (selectedIngredients.length === 0) {
      setError('Please add at least one ingredient.');
      return;
    }

    try {
      setIsSubmitting(true);
      setError(null);
      setSuccess(null);

      await foodService.createFood({
        name: values.name,
        type: values.type,
        calories: Math.max(1, Math.round(totals.calories)),
      });

      setSuccess('Food created successfully.');
      setSelectedIngredients([]);
      setTotals(emptyTotals);
      reset();
    } catch (submitError) {
      const message = submitError instanceof Error ? submitError.message : 'Failed to create food';
      setError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-12 max-w-5xl space-y-8">
        <div className="flex items-center justify-between gap-4">
          <div>
            <h1 className="text-4xl font-bold text-white">Create Food</h1>
            <p className="mt-2 text-gray-400">Build a food item from ingredients and preview the calculated nutrition.</p>
          </div>
          <button
            type="button"
            onClick={() => navigate('/foods')}
            className="rounded-lg border border-white/20 px-4 py-2 text-gray-200"
          >
            Back to Foods
          </button>
        </div>

        {error && <div className="rounded-lg border border-red-500/40 bg-red-500/10 px-4 py-3 text-red-200">{error}</div>}
        {success && <div className="rounded-lg border border-green-500/40 bg-green-500/10 px-4 py-3 text-green-200">{success}</div>}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 rounded-2xl border border-white/10 bg-white/5 p-6">
            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">Food Name</label>
              <input
                {...register('name', { required: 'Food name is required' })}
                type="text"
                className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white"
                placeholder="e.g. Chicken Bowl"
              />
              {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
            </div>

            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">Food Type</label>
              <input
                {...register('type', { required: 'Food type is required' })}
                type="text"
                className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white"
                placeholder="e.g. Dinner"
              />
              {errors.type && <span className="text-red-400 text-xs mt-1 block">{errors.type.message}</span>}
            </div>
          </div>

          <FoodIngredientsPicker
            value={selectedIngredients}
            onChange={setSelectedIngredients}
            onTotalsChange={handleTotalsChange}
          />

          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 rounded-2xl border border-white/10 bg-white/5 p-6">
            {[
              { label: 'Calories', value: totals.calories },
              { label: 'Protein', value: totals.protein },
              { label: 'Carbs', value: totals.carbs },
              { label: 'Fat', value: totals.fat },
            ].map((item) => (
              <div key={item.label} className="rounded-xl border border-white/10 bg-slate-900/60 px-4 py-4">
                <div className="text-xs uppercase tracking-wide text-gray-400">{item.label}</div>
                <div className="mt-2 text-2xl font-semibold text-white">{item.value.toFixed(2)}</div>
              </div>
            ))}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-3 font-semibold text-white disabled:opacity-50"
          >
            {isSubmitting ? 'Creating...' : 'Create Food'}
          </button>
        </form>
      </div>
    </div>
  );
};