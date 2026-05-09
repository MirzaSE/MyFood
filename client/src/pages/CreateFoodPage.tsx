import React, { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { AlertCircle, ChefHat } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { FoodCreateDto } from '../types';
import type { FoodIngredientPick, Ingredient } from '../types/ingredient';

interface CreateFoodFormValues {
  name: string;
  type: string;
  calories: number;
}

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [picks, setPicks] = useState<FoodIngredientPick[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [ingredientCache, setIngredientCache] = useState<Map<number, Ingredient>>(new Map());

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<CreateFoodFormValues>({
    defaultValues: { name: '', type: '', calories: 0 },
  });

  // Try to keep an Ingredient cache by querying when picks change.
  React.useEffect(() => {
    let cancelled = false;
    const missing = picks
      .map((p) => p.ingredientId)
      .filter((id) => !ingredientCache.has(id));
    if (missing.length === 0) return;
    Promise.all(missing.map((id) => ingredientService.getIngredientById(id).catch(() => null)))
      .then((results) => {
        if (cancelled) return;
        setIngredientCache((prev) => {
          const next = new Map(prev);
          results.forEach((ing) => {
            if (ing) next.set(ing.id, ing);
          });
          return next;
        });
      });
    return () => {
      cancelled = true;
    };
  }, [picks, ingredientCache]);

  const computedNutrition = useMemo(() => {
    let calories = 0;
    let protein = 0;
    let carbs = 0;
    let fat = 0;
    picks.forEach((pick) => {
      const ing = ingredientCache.get(pick.ingredientId);
      if (!ing) return;
      calories += ing.caloriesPerUnit * pick.quantity;
      protein += ing.protein * pick.quantity;
      carbs += ing.carbs * pick.quantity;
      fat += ing.fat * pick.quantity;
    });
    return { calories, protein, carbs, fat };
  }, [picks, ingredientCache]);

  // Auto-fill calories field with computed total when changes
  React.useEffect(() => {
    if (picks.length > 0) {
      setValue('calories', Math.round(computedNutrition.calories));
    }
  }, [computedNutrition.calories, picks.length, setValue]);

  const onSubmit = async (values: CreateFoodFormValues) => {
    try {
      setIsSubmitting(true);
      setError(null);
      const payload: FoodCreateDto = {
        name: values.name,
        type: values.type,
        calories: Number(values.calories) || 0,
      };
      await foodService.createFood(payload);
      navigate('/foods');
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'Failed to create food');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-12 max-w-3xl">
        <div className="mb-8 flex items-center space-x-3">
          <ChefHat size={32} className="text-purple-400" />
          <h1 className="text-3xl sm:text-4xl font-bold text-white">Create Food</h1>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 rounded-2xl p-6 space-y-5"
        >
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Food name</label>
            <input
              {...register('name', { required: 'Name is required' })}
              type="text"
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500"
              placeholder="e.g., Mediterranean Salad"
            />
            {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">Type</label>
              <input
                {...register('type', { required: 'Type is required' })}
                type="text"
                className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500"
                placeholder="e.g., Main"
              />
              {errors.type && <span className="text-red-400 text-xs mt-1 block">{errors.type.message}</span>}
            </div>
            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">Calories</label>
              <input
                {...register('calories', { required: 'Calories is required', valueAsNumber: true, min: 0 })}
                type="number"
                className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500"
              />
              {picks.length > 0 && (
                <p className="text-xs text-purple-300 mt-1">Auto-calculated from ingredients.</p>
              )}
            </div>
          </div>

          <div className="border-t border-white/10 pt-5">
            <h2 className="text-lg font-semibold text-white mb-4">Ingredients</h2>
            <FoodIngredientsPicker value={picks} onChange={setPicks} />
          </div>

          {picks.length > 0 && (
            <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
              <NutritionStat label="Calories" value={`${computedNutrition.calories.toFixed(0)} kcal`} />
              <NutritionStat label="Protein" value={`${computedNutrition.protein.toFixed(1)} g`} />
              <NutritionStat label="Carbs" value={`${computedNutrition.carbs.toFixed(1)} g`} />
              <NutritionStat label="Fat" value={`${computedNutrition.fat.toFixed(1)} g`} />
            </div>
          )}

          <div className="flex space-x-3 pt-4">
            <button
              type="button"
              onClick={() => navigate('/foods')}
              className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all font-medium"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all font-medium shadow-lg hover:shadow-purple-500/50 disabled:opacity-50"
            >
              {isSubmitting ? 'Creating…' : 'Create Food'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

const NutritionStat: React.FC<{ label: string; value: string }> = ({ label, value }) => (
  <div className="bg-white/5 border border-white/10 rounded-lg p-3 text-center">
    <p className="text-gray-400 text-xs uppercase tracking-wide">{label}</p>
    <p className="text-white text-lg font-bold mt-1">{value}</p>
  </div>
);
