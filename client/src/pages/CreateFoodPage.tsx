import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { FoodCreateDto, Ingredient, SelectedIngredientInput } from '../types';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredientInput[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<FoodCreateDto>({ name: '', type: '', calories: 0 });

  useEffect(() => {
    void ingredientService.getAllIngredients('', 1, 50).then(setIngredients).catch(() => setIngredients([]));
  }, []);

  const calculatedNutrition = useMemo(() => {
    return selectedIngredients.reduce((sum, selected) => {
      const ingredient = ingredients.find((x) => x.id === selected.ingredientId);
      if (!ingredient) return sum;
      // Fallback model: ingredient.quantity is treated as calories/unit for preview.
      return sum + ingredient.quantity * selected.quantity;
    }, 0);
  }, [ingredients, selectedIngredients]);

  const finalCalories = (Number(formData.calories) || 0) + calculatedNutrition;

  const submit = async (event: React.FormEvent) => {
    event.preventDefault();
    try {
      setIsSubmitting(true);
      setError(null);
      const payload: FoodCreateDto = {
        ...formData,
        calories: finalCalories,
      };
      await foodService.createFood(payload);
      navigate('/foods');
    } catch {
      setError('Failed to create food.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-10 max-w-3xl">
        <h1 className="text-3xl font-bold text-white mb-2">Create Food</h1>
        <p className="text-gray-400 mb-6">Add ingredients and let nutrition totals auto-calculate.</p>

        {error && <p className="mb-4 text-red-300">{error}</p>}

        <form onSubmit={submit} className="space-y-5">
          <div>
            <label className="block text-sm text-gray-300 mb-1">Name</label>
            <input
              value={formData.name}
              onChange={(e) => setFormData((prev) => ({ ...prev, name: e.target.value }))}
              required
              className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            />
          </div>
          <div>
            <label className="block text-sm text-gray-300 mb-1">Type</label>
            <input
              value={formData.type}
              onChange={(e) => setFormData((prev) => ({ ...prev, type: e.target.value }))}
              required
              className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            />
          </div>
          <div>
            <label className="block text-sm text-gray-300 mb-1">Base calories (food only)</label>
            <input
              type="number"
              min={1}
              value={formData.calories || ''}
              onChange={(e) => setFormData((prev) => ({ ...prev, calories: Number(e.target.value) }))}
              required={selectedIngredients.length === 0}
              className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
            />
          </div>

          <div>
            <h2 className="text-white font-medium mb-2">Ingredients Picker</h2>
            <FoodIngredientsPicker
              ingredients={ingredients}
              selected={selectedIngredients}
              onChange={setSelectedIngredients}
            />
            <p className="text-sm text-gray-300 mt-2">Ingredients calories: {calculatedNutrition}</p>
            <p className="text-sm text-emerald-300 mt-1">Final calories to save: {finalCalories}</p>
          </div>

          <div className="flex gap-3">
            <button
              type="button"
              onClick={() => navigate('/foods')}
              className="px-4 py-2 rounded-lg border border-white/20 text-gray-200"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-4 py-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 text-white"
            >
              {isSubmitting ? 'Creating...' : 'Create Food'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
