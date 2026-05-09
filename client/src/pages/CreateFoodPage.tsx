import React, { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { ArrowLeft, AlertCircle, CheckCircle2 } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import type { FoodCreateDto } from '../types';
import type { PickedIngredient } from '../types/ingredient';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<FoodCreateDto>({
    defaultValues: { name: '', type: '', calories: 0 },
  });

  const [picked, setPicked] = useState<PickedIngredient[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const computedCalories = useMemo(
    () =>
      Math.round(
        picked.reduce((sum, p) => sum + p.quantity * p.ingredient.caloriesPerUnit, 0)
      ),
    [picked]
  );

  const declaredCalories = Number(watch('calories')) || 0;

  const onSubmit = async (data: FoodCreateDto) => {
    setIsSubmitting(true);
    setError(null);
    setSuccess(null);
    try {
      const payload: FoodCreateDto = {
        ...data,
        // If user didn't manually enter calories, use computed total
        calories: declaredCalories || computedCalories,
      };
      const created = await foodService.createFood(payload);
      setSuccess(`Created "${created.name}".`);
      setTimeout(() => navigate('/foods'), 700);
    } catch (err: unknown) {
      const e = err as { response?: { data?: { message?: string } } };
      setError(e.response?.data?.message ?? 'Failed to create food.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const useComputed = () => setValue('calories', computedCalories);

  const fieldClass =
    'w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all';

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-12 max-w-3xl">
        <button
          onClick={() => navigate('/foods')}
          className="flex items-center gap-2 text-gray-300 hover:text-white mb-6 text-sm"
        >
          <ArrowLeft size={16} /> Back to foods
        </button>

        <h1 className="text-4xl font-bold text-white mb-2">Create Food</h1>
        <p className="text-gray-400 mb-8">
          Pick ingredients to compose the food and see calculated nutrition.
        </p>

        {error && (
          <div className="mb-4 p-3 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start gap-2">
            <AlertCircle size={18} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200 text-sm">{error}</p>
          </div>
        )}
        {success && (
          <div className="mb-4 p-3 bg-emerald-500/20 border border-emerald-500/50 rounded-lg flex items-start gap-2">
            <CheckCircle2 size={18} className="text-emerald-400 flex-shrink-0 mt-0.5" />
            <p className="text-emerald-200 text-sm">{success}</p>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
            <input
              {...register('name', { required: 'Name is required' })}
              className={fieldClass}
              placeholder="e.g., Margherita Pizza"
              disabled={isSubmitting}
            />
            {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Type</label>
            <input
              {...register('type', { required: 'Type is required' })}
              className={fieldClass}
              placeholder="e.g., Main, Starter, Dessert"
              disabled={isSubmitting}
            />
            {errors.type && <p className="text-red-400 text-xs mt-1">{errors.type.message}</p>}
          </div>

          <div>
            <div className="flex justify-between items-end mb-2">
              <label className="text-sm font-semibold text-gray-300">Calories</label>
              {computedCalories > 0 && (
                <button
                  type="button"
                  onClick={useComputed}
                  className="text-xs text-purple-300 hover:text-purple-200"
                >
                  Use computed ({computedCalories})
                </button>
              )}
            </div>
            <input
              {...register('calories', {
                valueAsNumber: true,
                min: { value: 0, message: 'Must be positive' },
              })}
              type="number"
              className={fieldClass}
              placeholder={computedCalories > 0 ? `Auto: ${computedCalories}` : 'e.g., 800'}
              disabled={isSubmitting}
            />
            {errors.calories && (
              <p className="text-red-400 text-xs mt-1">{errors.calories.message}</p>
            )}
          </div>

          {/* Ingredients picker — calculated nutrition lives inside this component */}
          <FoodIngredientsPicker picked={picked} onChange={setPicked} />

          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={() => navigate('/foods')}
              disabled={isSubmitting}
              className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
            >
              {isSubmitting ? 'Saving…' : 'Create Food'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
