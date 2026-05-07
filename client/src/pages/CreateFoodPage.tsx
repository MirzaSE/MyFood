import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import type { SelectedFoodIngredient } from '../types/ingredient';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [type, setType] = useState('');
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedFoodIngredient[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const totalCalories = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.caloriesPerUnit,
      0
    );
  }, [selectedIngredients]);

  const totalProtein = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.protein,
      0
    );
  }, [selectedIngredients]);

  const totalCarbs = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.carbs,
      0
    );
  }, [selectedIngredients]);

  const totalFat = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.fat,
      0
    );
  }, [selectedIngredients]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!name.trim()) {
      setError('Food name is required.');
      return;
    }

    if (!type.trim()) {
      setError('Food type is required.');
      return;
    }

    try {
      setIsSaving(true);
      setError(null);

      await foodService.createFood({
        name: name.trim(),
        type: type.trim(),
        calories: Math.round(totalCalories),
      });

      navigate('/foods');
    } catch {
      setError('Failed to create food.');
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <main className="mx-auto max-w-5xl p-6">
        <div className="mb-6">
          <h1 className="text-3xl font-bold text-gray-900">
            Create Food
          </h1>
          <p className="text-gray-600">
            Add ingredients and calculate nutrition before saving the food.
          </p>
        </div>

        {error && (
          <div className="mb-4 rounded-lg bg-red-50 p-3 text-red-700">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="rounded-xl border border-gray-200 bg-white p-4">
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Food name
                </label>
                <input
                  value={name}
                  onChange={(event) => setName(event.target.value)}
                  className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
                  placeholder="Example: Chicken rice bowl"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Food type
                </label>
                <input
                  value={type}
                  onChange={(event) => setType(event.target.value)}
                  className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
                  placeholder="Example: Meal"
                />
              </div>
            </div>
          </div>

          <FoodIngredientsPicker
            selectedIngredients={selectedIngredients}
            onChange={setSelectedIngredients}
          />

          <div className="rounded-xl border border-gray-200 bg-white p-4">
            <h3 className="mb-3 text-lg font-semibold text-gray-900">
              Calculated Nutrition
            </h3>
            <div className="grid grid-cols-2 gap-3 md:grid-cols-4">
              <div className="rounded-lg bg-gray-50 p-3">
                <p className="text-sm text-gray-500">Calories</p>
                <p className="text-xl font-bold">{Math.round(totalCalories)}</p>
              </div>
              <div className="rounded-lg bg-gray-50 p-3">
                <p className="text-sm text-gray-500">Protein</p>
                <p className="text-xl font-bold">{Math.round(totalProtein * 100) / 100}</p>
              </div>
              <div className="rounded-lg bg-gray-50 p-3">
                <p className="text-sm text-gray-500">Carbs</p>
                <p className="text-xl font-bold">{Math.round(totalCarbs * 100) / 100}</p>
              </div>
              <div className="rounded-lg bg-gray-50 p-3">
                <p className="text-sm text-gray-500">Fat</p>
                <p className="text-xl font-bold">{Math.round(totalFat * 100) / 100}</p>
              </div>
            </div>
          </div>

          <div className="flex justify-end gap-3">
            <button
              type="button"
              onClick={() => navigate('/foods')}
              className="rounded-lg border border-gray-300 px-4 py-2 text-gray-700"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSaving}
              className="rounded-lg bg-blue-600 px-4 py-2 text-white disabled:opacity-60"
            >
              {isSaving ? 'Saving...' : 'Create Food'}
            </button>
          </div>
        </form>
      </main>
    </div>
  );
};