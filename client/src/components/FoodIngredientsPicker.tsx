import React from 'react';
import { Plus, Trash2 } from 'lucide-react';
import type { FoodIngredientInput, Ingredient, NutritionTotals } from '../types';

interface FoodIngredientsPickerProps {
  availableIngredients: Ingredient[];
  selectedIngredients: FoodIngredientInput[];
  onChange: (items: FoodIngredientInput[]) => void;
  disabled?: boolean;
}

export const calculateNutritionTotals = (
  availableIngredients: Ingredient[],
  selectedIngredients: FoodIngredientInput[]
): NutritionTotals => {
  return selectedIngredients.reduce<NutritionTotals>(
    (totals, selection) => {
      const ingredient = availableIngredients.find((item) => item.id === selection.ingredientId);
      if (!ingredient) {
        return totals;
      }

      return {
        calories: totals.calories + ingredient.caloriesPerUnit * selection.quantity,
        protein: totals.protein + ingredient.protein * selection.quantity,
        carbs: totals.carbs + ingredient.carbs * selection.quantity,
        fat: totals.fat + ingredient.fat * selection.quantity,
      };
    },
    { calories: 0, protein: 0, carbs: 0, fat: 0 }
  );
};

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  availableIngredients,
  selectedIngredients,
  onChange,
  disabled = false,
}) => {
  const [nextIngredientId, setNextIngredientId] = React.useState<number | ''>('');

  const addIngredient = () => {
    if (!nextIngredientId) {
      return;
    }

    if (selectedIngredients.some((item) => item.ingredientId === nextIngredientId)) {
      return;
    }

    onChange([...selectedIngredients, { ingredientId: nextIngredientId, quantity: 1 }]);
    setNextIngredientId('');
  };

  const updateQuantity = (ingredientId: number, quantity: number) => {
    onChange(
      selectedIngredients.map((item) =>
        item.ingredientId === ingredientId ? { ...item, quantity: quantity > 0 ? quantity : 0 } : item
      )
    );
  };

  const removeIngredient = (ingredientId: number) => {
    onChange(selectedIngredients.filter((item) => item.ingredientId !== ingredientId));
  };

  const totals = calculateNutritionTotals(availableIngredients, selectedIngredients);

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 rounded-xl border border-white/10 bg-white/5 p-4 md:flex-row">
        <select
          value={nextIngredientId}
          onChange={(event) => setNextIngredientId(event.target.value ? Number(event.target.value) : '')}
          className="flex-1 rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white"
          disabled={disabled}
        >
          <option value="">Select an ingredient</option>
          {availableIngredients.map((ingredient) => (
            <option key={ingredient.id} value={ingredient.id}>
              {ingredient.name} ({ingredient.unit})
            </option>
          ))}
        </select>
        <button
          type="button"
          onClick={addIngredient}
          disabled={disabled || !nextIngredientId}
          className="flex items-center justify-center gap-2 rounded-lg bg-amber-500 px-4 py-3 font-semibold text-slate-900 transition hover:bg-amber-400 disabled:opacity-50"
        >
          <Plus size={18} />
          Add Ingredient
        </button>
      </div>

      <div className="space-y-3">
        {selectedIngredients.length === 0 ? (
          <div className="rounded-xl border border-dashed border-white/15 bg-slate-900/40 p-5 text-sm text-gray-400">
            No ingredients selected yet.
          </div>
        ) : (
          selectedIngredients.map((selection) => {
            const ingredient = availableIngredients.find((item) => item.id === selection.ingredientId);
            if (!ingredient) {
              return null;
            }

            return (
              <div key={selection.ingredientId} className="grid gap-3 rounded-xl border border-white/10 bg-slate-900/60 p-4 md:grid-cols-[1fr_120px_90px]">
                <div>
                  <p className="font-semibold text-white">{ingredient.name}</p>
                  <p className="text-sm text-gray-400">
                    {ingredient.caloriesPerUnit} kcal per {ingredient.unit}
                  </p>
                </div>
                <input
                  type="number"
                  min={0.01}
                  step="0.01"
                  value={selection.quantity}
                  onChange={(event) => updateQuantity(selection.ingredientId, Number(event.target.value))}
                  className="rounded-lg border border-white/20 bg-slate-950 px-3 py-2 text-white"
                  disabled={disabled}
                />
                <button
                  type="button"
                  onClick={() => removeIngredient(selection.ingredientId)}
                  className="flex items-center justify-center gap-2 rounded-lg border border-red-500/30 bg-red-500/10 px-3 py-2 text-red-200 transition hover:bg-red-500/20"
                  disabled={disabled}
                >
                  <Trash2 size={16} />
                  Remove
                </button>
              </div>
            );
          })
        )}
      </div>

      <div className="grid gap-3 rounded-2xl border border-amber-500/30 bg-amber-500/10 p-5 text-sm text-amber-50 md:grid-cols-4">
        <div>
          <p className="text-amber-200">Calories</p>
          <p className="text-xl font-bold">{totals.calories.toFixed(0)}</p>
        </div>
        <div>
          <p className="text-amber-200">Protein</p>
          <p className="text-xl font-bold">{totals.protein.toFixed(1)} g</p>
        </div>
        <div>
          <p className="text-amber-200">Carbs</p>
          <p className="text-xl font-bold">{totals.carbs.toFixed(1)} g</p>
        </div>
        <div>
          <p className="text-amber-200">Fat</p>
          <p className="text-xl font-bold">{totals.fat.toFixed(1)} g</p>
        </div>
      </div>
    </div>
  );
};
