import React, { useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientPickerItem, FoodCreateDto } from '../types';
import type { IngredientCreateDto } from '../types/ingredient';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [name, setName] = useState('');
  const [type, setType] = useState('');
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedItems, setSelectedItems] = useState<IngredientPickerItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadIngredients();
  }, []);

  const loadIngredients = async () => {
    try {
      const result = await ingredientService.getAll(1, 100);
      setIngredients(result.items);
    } catch (err) {
      console.error('Failed to load ingredients:', err);
    }
  };

  const nutrition = useMemo(() => {
    return selectedItems.reduce(
      (acc, item) => {
        const ratio = item.quantity / 100;
        return {
          calories: acc.calories + item.ingredient.caloriesPerUnit * ratio,
          protein: acc.protein + item.ingredient.protein * ratio,
          carbs: acc.carbs + item.ingredient.carbs * ratio,
          fat: acc.fat + item.ingredient.fat * ratio,
        };
      },
      { calories: 0, protein: 0, carbs: 0, fat: 0 }
    );
  }, [selectedItems]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim() || !type.trim()) {
      setError('Name and type are required');
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const foodData: FoodCreateDto = {
        name: name.trim(),
        type: type.trim(),
        calories: Math.round(nutrition.calories),
      };
      await foodService.createFood(foodData);
      navigate('/foods');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create food');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        <div className="max-w-2xl mx-auto">
          <h1 className="text-3xl font-bold text-white mb-8">Create New Food</h1>

          {error && (
            <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg text-red-200">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-2">Food Name</label>
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
                placeholder="e.g. Grilled Chicken Salad"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-300 mb-2">Food Type</label>
              <input
                type="text"
                value={type}
                onChange={(e) => setType(e.target.value)}
                className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
                placeholder="e.g. Salad, Main Course"
              />
            </div>

            <div className="bg-slate-800/50 rounded-xl border border-white/10 p-6">
              <FoodIngredientsPicker
                availableIngredients={ingredients}
                selectedItems={selectedItems}
                onItemsChange={setSelectedItems}
              />
            </div>

            {selectedItems.length > 0 && (
              <div className="bg-gradient-to-r from-purple-600/20 to-blue-600/20 rounded-xl border border-purple-500/30 p-6">
                <h3 className="text-lg font-semibold text-white mb-4">Calculated Nutrition</h3>
                <div className="grid grid-cols-4 gap-4 text-center">
                  <div>
                    <div className="text-2xl font-bold text-white">{Math.round(nutrition.calories)}</div>
                    <div className="text-sm text-gray-400">Calories</div>
                  </div>
                  <div>
                    <div className="text-2xl font-bold text-white">{Math.round(nutrition.protein * 10) / 10}g</div>
                    <div className="text-sm text-gray-400">Protein</div>
                  </div>
                  <div>
                    <div className="text-2xl font-bold text-white">{Math.round(nutrition.carbs * 10) / 10}g</div>
                    <div className="text-sm text-gray-400">Carbs</div>
                  </div>
                  <div>
                    <div className="text-2xl font-bold text-white">{Math.round(nutrition.fat * 10) / 10}g</div>
                    <div className="text-sm text-gray-400">Fat</div>
                  </div>
                </div>
              </div>
            )}

            <div className="flex gap-4">
              <button
                type="button"
                onClick={() => navigate('/foods')}
                className="flex-1 px-4 py-3 rounded-lg border border-white/10 text-gray-300 hover:bg-white/5 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isLoading}
                className="flex-1 px-4 py-3 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white font-semibold transition-all disabled:opacity-50"
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