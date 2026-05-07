import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { ArrowLeft, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker, computePickerTotals } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientPickerLine } from '../types/ingredient';
import type { FoodCreateDto } from '../types';

type FormValues = {
  name: string;
  type: string;
  calories: number;
};

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [catalog, setCatalog] = useState<Ingredient[]>([]);
  const [pickerLines, setPickerLines] = useState<IngredientPickerLine[]>([]);
  const [catalogLoading, setCatalogLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: { name: '', type: '', calories: 0 },
  });

  const totals = useMemo(() => computePickerTotals(pickerLines), [pickerLines]);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        setCatalogLoading(true);
        const data = await ingredientService.getAll();
        if (!cancelled) setCatalog(data);
      } catch {
        if (!cancelled) setError('Could not load ingredients.');
      } finally {
        if (!cancelled) setCatalogLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  const onSubmit = async (data: FormValues) => {
    try {
      setSubmitting(true);
      setError(null);
      const payload: FoodCreateDto = {
        name: data.name.trim(),
        type: data.type.trim(),
        calories: Math.round(data.calories),
      };
      await foodService.createFood(payload);
      navigate('/foods');
    } catch (err: unknown) {
      const msg =
        typeof err === 'object' && err !== null && 'response' in err
          ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
          : undefined;
      setError(msg || 'Failed to create food.');
    } finally {
      setSubmitting(false);
    }
  };

  const applyCalculatedCalories = () => {
    setValue('calories', Math.round(totals.calories * 100) / 100);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-10 max-w-3xl">
        <button
          type="button"
          onClick={() => navigate('/foods')}
          className="flex items-center gap-2 text-gray-400 hover:text-white mb-8"
        >
          <ArrowLeft size={18} />
          Back to foods
        </button>

        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex gap-3">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <h1 className="text-3xl sm:text-4xl font-bold text-white mb-2">Create food</h1>
        <p className="text-gray-400 mb-10">Add a meal and estimate nutrition from ingredients.</p>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-10">
          <div className="rounded-xl border border-white/10 bg-white/5 p-6 space-y-5">
            <h2 className="text-lg font-semibold text-white">Basics</h2>
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-2">Name</label>
              <input
                {...register('name', { required: 'Name is required' })}
                className="w-full px-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
                placeholder="e.g. Chicken bowl"
              />
              {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name.message}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-2">Type</label>
              <input
                {...register('type', { required: 'Type is required' })}
                className="w-full px-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
                placeholder="e.g. Main"
              />
              {errors.type && <p className="text-red-400 text-xs mt-1">{errors.type.message}</p>}
            </div>
            <div>
              <div className="flex justify-between items-center mb-2">
                <label className="text-sm font-medium text-gray-300">Calories (stored on food)</label>
                <button
                  type="button"
                  onClick={applyCalculatedCalories}
                  className="text-xs text-purple-300 hover:text-purple-200"
                >
                  Use total from ingredients
                </button>
              </div>
              <input
                {...register('calories', {
                  required: true,
                  valueAsNumber: true,
                  min: { value: 1, message: 'Calories must be greater than 0' },
                })}
                type="number"
                className="w-full px-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
              />
              {errors.calories && <p className="text-red-400 text-xs mt-1">{errors.calories.message}</p>}
            </div>
          </div>

          <FoodIngredientsPicker
            catalog={catalog}
            lines={pickerLines}
            onChange={setPickerLines}
            disabled={submitting}
            isLoading={catalogLoading}
          />

          <div className="rounded-xl border border-emerald-500/30 bg-emerald-950/20 p-4 text-sm text-emerald-100">
            <p className="font-medium mb-1">Calculated nutrition (from picker)</p>
            <p className="text-gray-300">
              Calories <strong className="text-white">{Math.round(totals.calories * 100) / 100}</strong>
              {' · '}P {Math.round(totals.protein * 100) / 100}
              {' · '}C {Math.round(totals.carbs * 100) / 100}
              {' · '}F {Math.round(totals.fat * 100) / 100}
            </p>
            <p className="text-xs text-gray-500 mt-2">
              Ingredient links are not persisted by the API; this is for planning and copying calories into the food
              record.
            </p>
          </div>

          <button
            type="submit"
            disabled={submitting}
            className="w-full py-3 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 text-white font-semibold disabled:opacity-50"
          >
            {submitting ? 'Creating…' : 'Create food'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default CreateFoodPage;
