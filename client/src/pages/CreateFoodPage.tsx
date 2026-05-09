import React, { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { AlertCircle, ChefHat } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { FoodCreateDto } from '../types';
import type { FoodIngredientLine } from '../types/ingredient';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FoodCreateDto>({
    defaultValues: { name: '', type: '', calories: 0 },
  });

  const [lines, setLines] = useState<FoodIngredientLine[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const totals = useMemo(() => {
    const acc = { calories: 0, protein: 0, carbs: 0, fat: 0 };
    for (const l of lines) {
      const q = Number.isFinite(l.quantity) ? l.quantity : 0;
      acc.calories += l.ingredient.caloriesPerUnit * q;
      acc.protein += l.ingredient.protein * q;
      acc.carbs += l.ingredient.carbs * q;
      acc.fat += l.ingredient.fat * q;
    }
    return acc;
  }, [lines]);

  const onSubmit = async (formData: FoodCreateDto) => {
    try {
      setError(null);
      setIsSubmitting(true);

      // Use the user-provided calorie count if non-zero, otherwise compute from picker.
      const calories =
        Number(formData.calories) > 0
          ? Number(formData.calories)
          : Math.round(totals.calories);

      const food = await foodService.createFood({
        name: formData.name,
        type: formData.type,
        calories,
      });

      // Best-effort link of selected ingredients to the new food. Each picker line
      // becomes an UPDATE on the existing ingredient (sets foodEntityId). If any
      // single link fails, we surface a soft error but the food itself is saved.
      const linkErrors: string[] = [];
      for (const line of lines) {
        try {
          await ingredientService.update(line.ingredient.id, {
            foodEntityId: food.id,
          });
        } catch {
          linkErrors.push(line.ingredient.name);
        }
      }
      if (linkErrors.length > 0) {
        setError(`Food saved, but failed to link: ${linkErrors.join(', ')}`);
        return;
      }

      navigate('/foods');
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create food');
    } finally {
      setIsSubmitting(false);
    }
  };

  const labelClass = 'block text-sm font-semibold text-gray-300 mb-2';
  const inputClass =
    'w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none ' +
    'focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white ' +
    'placeholder-gray-400 text-sm transition-all';

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-10">
        <div className="flex items-center space-x-3 mb-8">
          <div className="p-2 bg-gradient-to-br from-purple-500 to-blue-500 rounded-lg">
            <ChefHat size={22} className="text-white" />
          </div>
          <h1 className="text-3xl sm:text-4xl font-bold text-white">Compose a new food</h1>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="grid lg:grid-cols-2 gap-8">
          <section className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 rounded-2xl p-6 space-y-5">
            <h2 className="text-lg font-bold text-white">Food details</h2>

            <div>
              <label className={labelClass}>Name</label>
              <input
                {...register('name', { required: 'Name is required' })}
                placeholder="e.g., Tomato Soup"
                className={inputClass}
              />
              {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
            </div>

            <div>
              <label className={labelClass}>Type</label>
              <input
                {...register('type', { required: 'Type is required' })}
                placeholder="e.g., Soup, Main, Side"
                className={inputClass}
              />
              {errors.type && <span className="text-red-400 text-xs mt-1 block">{errors.type.message}</span>}
            </div>

            <div>
              <label className={labelClass}>Calories (override)</label>
              <input
                {...register('calories', { valueAsNumber: true, min: { value: 0, message: 'Must be ≥ 0' } })}
                type="number"
                step="1"
                placeholder="Leave 0 to use computed total"
                className={inputClass}
              />
              {errors.calories && <span className="text-red-400 text-xs mt-1 block">{errors.calories.message}</span>}
              <p className="text-xs text-gray-500 mt-1">
                Set to 0 to use the picker's computed value: {totals.calories.toFixed(1)} kcal.
              </p>
            </div>

            <div className="flex space-x-3 pt-3">
              <button
                type="button"
                onClick={() => navigate('/foods')}
                className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 font-medium"
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
          </section>

          <section className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 rounded-2xl p-6">
            <h2 className="text-lg font-bold text-white mb-4">Ingredients</h2>
            <FoodIngredientsPicker selectedLines={lines} onChange={setLines} />
          </section>
        </form>
      </div>
    </div>
  );
};
