import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { ArrowLeft, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import type { FoodCreateDto } from '../types';

interface SelectedIngredient {
  ingredientId: number;
  name: string;
  quantity: string;
}

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredient[]>([]);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FoodCreateDto>();

  const onSubmit = async (data: FoodCreateDto) => {
    try {
      setIsSubmitting(true);
      setError(null);
      await foodService.createFood(data);
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

      <div className="container mx-auto px-6 py-16 max-w-2xl">
        <button
          onClick={() => navigate('/foods')}
          className="flex items-center space-x-2 text-gray-400 hover:text-white mb-8 transition-colors"
        >
          <ArrowLeft size={18} />
          <span>Back to Foods</span>
        </button>

        <h1 className="text-4xl font-bold text-white mb-8">Create New Food</h1>

        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 rounded-xl p-6 space-y-5">
            <h2 className="text-lg font-semibold text-white">Food Details</h2>

            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">Name</label>
              <input
                {...register('name', { required: 'Name is required' })}
                type="text"
                className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
                placeholder="e.g., Grilled Chicken"
                disabled={isSubmitting}
              />
              {errors.name && (
                <span className="text-red-400 text-xs mt-1 block">{errors.name.message}</span>
              )}
            </div>

            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">Type</label>
              <input
                {...register('type', { required: 'Type is required' })}
                type="text"
                className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
                placeholder="e.g., Protein"
                disabled={isSubmitting}
              />
              {errors.type && (
                <span className="text-red-400 text-xs mt-1 block">{errors.type.message}</span>
              )}
            </div>

            <div>
              <label className="block text-sm font-semibold text-gray-300 mb-2">Calories</label>
              <input
                {...register('calories', {
                  required: 'Calories is required',
                  valueAsNumber: true,
                  min: { value: 0, message: 'Calories must be positive' },
                })}
                type="number"
                className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
                placeholder="e.g., 250"
                disabled={isSubmitting}
              />
              {errors.calories && (
                <span className="text-red-400 text-xs mt-1 block">{errors.calories.message}</span>
              )}
            </div>
          </div>

          {/* Ingredients Picker Section */}
          <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 rounded-xl p-6">
            <FoodIngredientsPicker
              selectedIngredients={selectedIngredients}
              onChange={setSelectedIngredients}
            />
          </div>

          {/* Actions */}
          <div className="flex space-x-3">
            <button
              type="button"
              onClick={() => navigate('/foods')}
              className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 font-medium"
              disabled={isSubmitting}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
              disabled={isSubmitting}
            >
              {isSubmitting ? (
                <span className="flex items-center justify-center">
                  <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
                  Creating...
                </span>
              ) : (
                'Create Food'
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
