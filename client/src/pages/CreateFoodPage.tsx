import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker, calculateNutritionTotals } from '../components/FoodIngredientsPicker';
import { extractApiErrorMessage } from '../services/api';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { FoodCreateDto, FoodIngredientInput, Ingredient } from '../types';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = useState<FoodIngredientInput[]>([]);
  const [name, setName] = useState('');
  const [type, setType] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    void ingredientService.getAllIngredients({ page: 1, pageCount: 100 })
      .then((result) => setIngredients(result.items))
      .catch((err) => setError(extractApiErrorMessage(err)));
  }, []);

  const nutritionTotals = calculateNutritionTotals(ingredients, selectedIngredients);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    const payload: FoodCreateDto = {
      name,
      type,
      calories: Math.round(nutritionTotals.calories),
      ingredients: selectedIngredients,
    };

    try {
      setIsLoading(true);
      setError(null);
      await foodService.createFood(payload);
      navigate('/foods');
    } catch (err: any) {
      setError(extractApiErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-[linear-gradient(180deg,#111827_0%,#0f172a_52%,#1f2937_100%)]">
      <Navbar />
      <div className="container mx-auto max-w-5xl px-6 py-12">
        <button onClick={() => navigate('/foods')} className="mb-6 inline-flex items-center gap-2 text-sm text-amber-200 transition hover:text-amber-100">
          <ArrowLeft size={16} />
          Back to foods
        </button>

        <div className="rounded-[2rem] border border-white/10 bg-white/5 p-8 shadow-2xl backdrop-blur">
          <p className="mb-3 text-sm uppercase tracking-[0.35em] text-amber-300">Create Food</p>
          <h1 className="text-4xl font-black text-white">Build a food from real ingredients</h1>
          <p className="mt-3 text-gray-400">Pick ingredients, set quantities, and let the nutrition totals update automatically.</p>

          {error && <div className="mt-6 rounded-xl border border-red-500/30 bg-red-500/10 p-4 text-red-100">{error}</div>}

          <form onSubmit={handleSubmit} className="mt-8 space-y-8">
            <div className="grid gap-6 md:grid-cols-2">
              <div>
                <label className="mb-2 block text-sm font-semibold text-gray-200">Food Name</label>
                <input
                  value={name}
                  onChange={(event) => setName(event.target.value)}
                  required
                  className="w-full rounded-xl border border-white/10 bg-slate-950 px-4 py-3 text-white"
                />
              </div>
              <div>
                <label className="mb-2 block text-sm font-semibold text-gray-200">Type</label>
                <input
                  value={type}
                  onChange={(event) => setType(event.target.value)}
                  required
                  className="w-full rounded-xl border border-white/10 bg-slate-950 px-4 py-3 text-white"
                />
              </div>
            </div>

            <section className="space-y-4">
              <div>
                <h2 className="text-2xl font-bold text-white">Ingredients</h2>
                <p className="text-sm text-gray-400">Choose ingredients and adjust quantities to compute calories and macros.</p>
              </div>
              <FoodIngredientsPicker
                availableIngredients={ingredients}
                selectedIngredients={selectedIngredients}
                onChange={setSelectedIngredients}
                disabled={isLoading}
              />
            </section>

            <div className="flex flex-col gap-3 sm:flex-row">
              <button
                type="button"
                onClick={() => navigate('/foods')}
                className="rounded-xl border border-white/15 px-5 py-3 text-gray-200 transition hover:bg-white/10"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isLoading}
                className="rounded-xl bg-amber-500 px-5 py-3 font-semibold text-slate-900 transition hover:bg-amber-400 disabled:opacity-60"
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
