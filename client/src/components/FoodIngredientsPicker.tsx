import React, { useEffect, useMemo, useState } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientSelection, NutritionTotals } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  value: IngredientSelection[];
  onChange: (value: IngredientSelection[]) => void;
  onTotalsChange: (totals: NutritionTotals) => void;
}

const emptyTotals: NutritionTotals = {
  calories: 0,
  protein: 0,
  carbs: 0,
  fat: 0,
};

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({ value, onChange, onTotalsChange }) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const allIngredients = await ingredientService.getAllIngredients();
        setIngredients(allIngredients);
      } catch {
        setError('Failed to load ingredients.');
      } finally {
        setIsLoading(false);
      }
    };

    void loadIngredients();
  }, []);

  const totals = useMemo(() => {
    return value.reduce<NutritionTotals>((acc, item) => {
      const ingredient = ingredients.find((entry) => entry.id === item.ingredientId);
      if (!ingredient) {
        return acc;
      }

      return {
        calories: acc.calories + ingredient.caloriesPerUnit * item.quantity,
        protein: acc.protein + ingredient.protein * item.quantity,
        carbs: acc.carbs + ingredient.carbs * item.quantity,
        fat: acc.fat + ingredient.fat * item.quantity,
      };
    }, emptyTotals);
  }, [ingredients, value]);

  useEffect(() => {
    onTotalsChange(totals);
  }, [onTotalsChange, totals]);

  const addSelection = () => {
    if (ingredients.length === 0) {
      return;
    }

    onChange([...value, { ingredientId: ingredients[0].id, quantity: 1 }]);
  };

  const updateSelection = (index: number, updates: Partial<IngredientSelection>) => {
    const next = value.map((item, itemIndex) => (itemIndex === index ? { ...item, ...updates } : item));
    onChange(next);
  };

  const removeSelection = (index: number) => {
    onChange(value.filter((_, itemIndex) => itemIndex !== index));
  };

  return (
    <div className="rounded-2xl border border-white/10 bg-white/5 p-6 space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-xl font-semibold text-white">Food Ingredients</h3>
          <p className="text-sm text-gray-400">Pick ingredients, add quantity, and calculate nutrition.</p>
        </div>
        <button
          type="button"
          onClick={addSelection}
          className="inline-flex items-center gap-2 rounded-lg bg-purple-600 px-4 py-2 text-sm font-medium text-white disabled:opacity-50"
          disabled={isLoading || ingredients.length === 0}
        >
          <Plus size={16} /> Add Ingredient
        </button>
      </div>

      {error && <div className="rounded-lg border border-red-500/40 bg-red-500/10 px-4 py-3 text-sm text-red-200">{error}</div>}

      {isLoading ? (
        <div className="text-sm text-gray-300">Loading ingredient options...</div>
      ) : value.length === 0 ? (
        <div className="rounded-lg border border-dashed border-white/20 px-4 py-6 text-sm text-gray-400">
          No ingredients selected yet.
        </div>
      ) : (
        <div className="space-y-3">
          {value.map((item, index) => (
            <div key={`${item.ingredientId}-${index}`} className="grid grid-cols-1 md:grid-cols-[1fr_120px_auto] gap-3 items-end">
              <div>
                <label className="block text-sm font-semibold text-gray-300 mb-2">Ingredient</label>
                <select
                  value={item.ingredientId}
                  onChange={(event) => updateSelection(index, { ingredientId: Number(event.target.value) })}
                  className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white"
                >
                  {ingredients.map((ingredient) => (
                    <option key={ingredient.id} value={ingredient.id}>
                      {ingredient.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-300 mb-2">Quantity</label>
                <input
                  type="number"
                  min="1"
                  step="0.25"
                  value={item.quantity}
                  onChange={(event) => updateSelection(index, { quantity: Number(event.target.value) || 1 })}
                  className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white"
                />
              </div>

              <button
                type="button"
                onClick={() => removeSelection(index)}
                className="inline-flex items-center justify-center gap-2 rounded-lg border border-red-500/40 px-4 py-3 text-red-200 hover:bg-red-500/10"
              >
                <Trash2 size={16} /> Remove
              </button>
            </div>
          ))}
        </div>
      )}

    </div>
  );
};