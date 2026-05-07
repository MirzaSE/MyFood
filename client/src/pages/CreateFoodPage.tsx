import React from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, AlertCircle } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { FoodCreateDto, Ingredient, SelectedIngredient } from '../types';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [availableIngredients, setAvailableIngredients] = React.useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = React.useState<SelectedIngredient[]>([]);
  const [isLoading, setIsLoading] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);
  const [success, setSuccess] = React.useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm<FoodCreateDto>({
    defaultValues: {
      name: '',
      type: '',
      calories: 1,
    },
  });

  React.useEffect(() => {
    const loadIngredients = async () => {
      const response = await ingredientService.getAllIngredients({ page: 1, pageCount: 50, query: '' });
      setAvailableIngredients(response.items);
    };
    loadIngredients().catch(() => {
      setAvailableIngredients([]);
    });
  }, []);

  const nutritionSummary = React.useMemo(
    () =>
      selectedIngredients.reduce(
        (totals, item) => {
          const amount = item.quantity;
          return {
            calories: totals.calories + item.ingredient.caloriesPerUnit * amount,
            protein: totals.protein + item.ingredient.protein * amount,
            carbs: totals.carbs + item.ingredient.carbs * amount,
            fat: totals.fat + item.ingredient.fat * amount,
          };
        },
        { calories: 0, protein: 0, carbs: 0, fat: 0 }
      ),
    [selectedIngredients]
  );
  const caloriesInput = watch('calories');
  const baseFoodCalories = Number.isFinite(caloriesInput) && caloriesInput > 0 ? caloriesInput : 0;
  const totalCalories = baseFoodCalories + nutritionSummary.calories;

  const onSubmit = async (data: FoodCreateDto) => {
    try {
      setIsLoading(true);
      setError(null);
      setSuccess(null);
      const createdFood = await foodService.createFood(data);
      await Promise.all(
        selectedIngredients.map((item) =>
          ingredientService.createIngredient(
            {
              name: item.ingredient.name,
              unit: item.ingredient.unit,
              caloriesPerUnit: Math.max(1, Math.floor(item.quantity)),
              protein: item.ingredient.protein,
              carbs: item.ingredient.carbs,
              fat: item.ingredient.fat,
            },
            createdFood.id
          )
        )
      );
      setSuccess('Food created successfully.');
      window.setTimeout(
        () =>
          navigate('/foods', {
            state: { createdFood, createdAt: Date.now() },
          }),
        500
      );
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create food.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-12 flex justify-center">
        <div className="w-full max-w-3xl">
          <button
            onClick={() => navigate('/foods')}
            className="mb-6 inline-flex items-center gap-2 text-gray-300 hover:text-white"
          >
            <ArrowLeft size={16} />
            Back to foods
          </button>

          <h1 className="text-4xl font-bold text-white mb-2">Create Food</h1>
          <p className="text-gray-400 mb-8">Create a food item and compose it with ingredients in one flow.</p>

          {error && (
            <div className="mb-4 p-4 bg-red-500/20 border border-red-500/40 rounded-lg text-red-200 flex items-start gap-2">
              <AlertCircle size={18} className="mt-0.5" />
              <span>{error}</span>
            </div>
          )}
          {success && (
            <div className="mb-4 p-4 bg-green-500/20 border border-green-500/40 rounded-lg text-green-200">{success}</div>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div className="bg-slate-800/70 border border-white/10 rounded-xl p-5 space-y-4">
              <h2 className="text-xl font-semibold text-white">Food Details</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
                  <input
                    {...register('name', {
                      required: 'Name is required',
                      setValueAs: (v) => (typeof v === 'string' ? v.trim() : v),
                      validate: (v) => (v?.length ? true : 'Name is required'),
                    })}
                    className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white"
                    placeholder="Food name"
                  />
                  {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>}
                </div>
                <div>
                  <label className="block text-sm font-semibold text-gray-300 mb-2">Type</label>
                  <input
                    {...register('type', {
                      required: 'Type is required',
                      setValueAs: (v) => (typeof v === 'string' ? v.trim() : v),
                      validate: (v) => (v?.length ? true : 'Type is required'),
                    })}
                    className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white"
                    placeholder="Protein, Dessert..."
                  />
                  {errors.type && <span className="text-red-400 text-xs mt-1 block">{errors.type.message}</span>}
                </div>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-300 mb-2">Calories</label>
                <input
                  {...register('calories', {
                    valueAsNumber: true,
                    validate: (v) => (Number.isFinite(v) && v > 0 ? true : 'Calories must be greater than 0'),
                  })}
                  type="number"
                  className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white"
                />
                {errors.calories && <span className="text-red-400 text-xs mt-1 block">{errors.calories.message}</span>}
              </div>
            </div>

            <FoodIngredientsPicker
              availableIngredients={availableIngredients}
              selectedIngredients={selectedIngredients}
              onChange={setSelectedIngredients}
            />

            <div className="bg-slate-800/60 border border-white/10 rounded-xl p-5">
              <h3 className="text-lg font-semibold text-white mb-4">Calculated Nutrition</h3>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                <NutritionTile label="Calories" value={`${totalCalories.toFixed(1)} kcal`} />
                <NutritionTile label="Protein" value={`${nutritionSummary.protein.toFixed(1)} g`} />
                <NutritionTile label="Carbs" value={`${nutritionSummary.carbs.toFixed(1)} g`} />
                <NutritionTile label="Fat" value={`${nutritionSummary.fat.toFixed(1)} g`} />
              </div>
              <p className="text-xs text-gray-400 mt-3">
                Food calories: {baseFoodCalories.toFixed(1)} kcal + Ingredients: {nutritionSummary.calories.toFixed(1)} kcal
              </p>
              {selectedIngredients.length === 0 && (
                <p className="text-xs text-gray-400 mt-3">
                  Add ingredients to see estimated totals.
                </p>
              )}
            </div>

            <div className="flex justify-end gap-3">
              <button
                type="button"
                onClick={() => navigate('/foods')}
                className="px-5 py-3 rounded-lg border border-white/20 text-gray-300 hover:text-white"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isLoading}
                className="px-6 py-3 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 text-white disabled:opacity-60"
              >
                {isLoading ? 'Creating...' : 'Create Food'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

const NutritionTile: React.FC<{ label: string; value: string }> = ({ label, value }) => (
  <div className="rounded-lg border border-white/10 bg-slate-900/70 px-4 py-3">
    <p className="text-xs uppercase tracking-wide text-gray-400">{label}</p>
    <p className="text-white font-semibold mt-1">{value}</p>
  </div>
);
