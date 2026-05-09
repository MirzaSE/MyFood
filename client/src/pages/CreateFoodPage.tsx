import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { ArrowLeft } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import type { FoodCreateDto } from '../types';
import type { SelectedIngredient } from '../types/ingredient';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredient[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const { register, handleSubmit, formState: { errors } } = useForm<FoodCreateDto>();

  const totalCaloriesFromIngredients = selectedIngredients.reduce(
    (sum, s) => sum + s.ingredient.caloriesPerUnit * s.quantity, 0
  );

  const onSubmit = async (data: FoodCreateDto) => {
    setIsLoading(true);
    setError('');
    try {
      await foodService.createFood(data);
      navigate('/foods');
    } catch (err: any) {
      setError(err?.response?.data || 'Failed to create food.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <div className="max-w-3xl mx-auto px-4 py-8">
        <button
          onClick={() => navigate('/foods')}
          className="flex items-center gap-2 text-gray-500 hover:text-gray-700 mb-6 transition-colors"
        >
          <ArrowLeft size={16} /> Back to Foods
        </button>

        <h1 className="text-3xl font-bold text-gray-800 mb-8">Create New Food</h1>

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-3 mb-6 text-red-600 text-sm">{error}</div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 space-y-4">
            <h2 className="text-lg font-semibold text-gray-700">Basic Information</h2>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Name *</label>
              <input
                {...register('name', { required: 'Name is required' })}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="e.g. Grilled Chicken"
              />
              {errors.name && <p className="text-red-500 text-sm mt-1">{errors.name.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Type *</label>
              <input
                {...register('type', { required: 'Type is required' })}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="e.g. Protein, Carb, Vegetable"
              />
              {errors.type && <p className="text-red-500 text-sm mt-1">{errors.type.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Calories *</label>
              <input
                type="number"
                {...register('calories', { required: 'Calories is required', valueAsNumber: true, min: 0 })}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="e.g. 250"
              />
              {errors.calories && <p className="text-red-500 text-sm mt-1">{errors.calories.message}</p>}
              {selectedIngredients.length > 0 && (
                <p className="text-xs text-blue-500 mt-1">
                  Suggested from ingredients: {totalCaloriesFromIngredients.toFixed(0)} cal
                </p>
              )}
            </div>
          </div>

          <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 className="text-lg font-semibold text-gray-700 mb-4">Ingredients</h2>
            <FoodIngredientsPicker
              selected={selectedIngredients}
              onChange={setSelectedIngredients}
            />
          </div>

          <div className="flex gap-3">
            <button
              type="submit"
              disabled={isLoading}
              className="flex-1 bg-blue-600 text-white py-3 rounded-xl hover:bg-blue-700 disabled:opacity-50 font-semibold text-lg"
            >
              {isLoading ? 'Creating...' : 'Create Food'}
            </button>
            <button
              type="button"
              onClick={() => navigate('/foods')}
              className="flex-1 bg-gray-100 text-gray-700 py-3 rounded-xl hover:bg-gray-200 font-semibold text-lg"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
