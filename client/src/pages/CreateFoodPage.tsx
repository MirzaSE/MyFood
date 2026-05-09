import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { ArrowLeft, Plus, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import type { FoodCreateDto, FoodIngredient } from '../types';

export function CreateFoodPage() {
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [ingredients, setIngredients] = useState<FoodIngredient[]>([]);
  const [calculatedNutrition, setCalculatedNutrition] = useState({
    calories: 0,
    protein: 0,
    carbs: 0,
    fat: 0,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<Omit<FoodCreateDto, 'ingredients'>>({
    defaultValues: {
      name: '',
      type: '',
      calories: 0,
    },
  });

  const handleIngredientsChange = (newIngredients: FoodIngredient[], nutrition: any) => {
    setIngredients(newIngredients);
    setCalculatedNutrition(nutrition);
  };

  const onSubmit = async (data: Omit<FoodCreateDto, 'ingredients'>) => {
    try {
      setIsSubmitting(true);
      setError(null);

      const foodData: FoodCreateDto = {
        ...data,
        ingredients: ingredients.length > 0 ? ingredients : undefined,
      };

      await foodService.createFood(foodData);
      navigate('/foods');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create food');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-8">
        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center space-x-4">
            <button
              onClick={() => navigate('/foods')}
              className="flex items-center space-x-2 text-gray-400 hover:text-white transition-colors"
            >
              <ArrowLeft size={20} />
              <span>Back to Foods</span>
            </button>
          </div>
          <h1 className="text-3xl font-bold text-white">Create New Food</h1>
        </div>

        {/* Error Banner */}
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Food Form */}
          <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl p-6">
            <h2 className="text-xl font-bold text-white mb-6">Food Details</h2>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
              <div>
                <label className="block text-sm font-semibold text-gray-300 mb-2">
                  Food Name
                </label>
                <input
                  {...register('name', { required: 'Name is required' })}
                  type="text"
                  className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
                  placeholder="e.g., Grilled Chicken Salad"
                  disabled={isSubmitting}
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
                  placeholder="e.g., Main Course, Salad"
                  disabled={isSubmitting}
                />
                {errors.type && <span className="text-red-400 text-xs mt-1 block">{errors.type.message}</span>}
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-300 mb-2">
                  Total Calories
                </label>
                <input
                  {...register('calories', {
                    required: 'Calories is required',
                    valueAsNumber: true,
                    min: { value: 0, message: 'Calories must be positive' },
                  })}
                  type="number"
                  className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
                  placeholder="e.g., 450"
                  disabled={isSubmitting}
                />
                {errors.calories && <span className="text-red-400 text-xs mt-1 block">{errors.calories.message}</span>}
              </div>

              <button
                type="submit"
                disabled={isSubmitting}
                className="w-full flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 disabled:opacity-50 font-semibold shadow-lg hover:shadow-purple-500/50"
              >
                {isSubmitting ? (
                  <>
                    <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                    <span>Creating...</span>
                  </>
                ) : (
                  <>
                    <Plus size={20} />
                    <span>Create Food</span>
                  </>
                )}
              </button>
            </form>
          </div>

          {/* Ingredients Section */}
          <div className="space-y-6">
            <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl p-6">
              <h2 className="text-xl font-bold text-white mb-6">Ingredients</h2>
              <FoodIngredientsPicker onIngredientsChange={handleIngredientsChange} />
            </div>

            {/* Nutrition Summary */}
            <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl p-6">
              <h2 className="text-xl font-bold text-white mb-6">Calculated Nutrition</h2>
              <div className="grid grid-cols-2 gap-4">
                <div className="bg-white/5 rounded-lg p-4">
                  <div className="text-2xl font-bold text-purple-400">{calculatedNutrition.calories.toFixed(1)}</div>
                  <div className="text-sm text-gray-400">Calories</div>
                </div>
                <div className="bg-white/5 rounded-lg p-4">
                  <div className="text-2xl font-bold text-blue-400">{calculatedNutrition.protein.toFixed(1)}g</div>
                  <div className="text-sm text-gray-400">Protein</div>
                </div>
                <div className="bg-white/5 rounded-lg p-4">
                  <div className="text-2xl font-bold text-green-400">{calculatedNutrition.carbs.toFixed(1)}g</div>
                  <div className="text-sm text-gray-400">Carbs</div>
                </div>
                <div className="bg-white/5 rounded-lg p-4">
                  <div className="text-2xl font-bold text-yellow-400">{calculatedNutrition.fat.toFixed(1)}g</div>
                  <div className="text-sm text-gray-400">Fat</div>
                </div>
              </div>
              <p className="text-xs text-gray-500 mt-4">
                Nutrition values are calculated based on selected ingredients and their quantities.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
