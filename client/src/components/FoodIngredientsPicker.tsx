import React, { useMemo, useState } from 'react';
import { Plus, X } from 'lucide-react';
import type { FoodIngredientCreateDto, Ingredient } from '../types';

export interface SelectedFoodIngredient extends FoodIngredientCreateDto {
  ingredient?: Ingredient;
}

interface FoodIngredientsPickerProps {
  ingredients: Ingredient[];
  value: SelectedFoodIngredient[];
  onChange: (value: SelectedFoodIngredient[]) => void;
  disabled?: boolean;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  ingredients,
  value,
  onChange,
  disabled = false,
}) => {
  const [selectedIngredientId, setSelectedIngredientId] = useState('');
  const [quantity, setQuantity] = useState('1');

  const enriched = useMemo(
    () =>
      value.map((entry) => ({
        ...entry,
        ingredient: ingredients.find((item) => item.id === entry.ingredientId),
      })),
    [ingredients, value]
  );

  const totalCalories = useMemo(
    () =>
      enriched.reduce((sum, entry) => {
        const cal = Number(entry.ingredient?.caloriesPerUnit ?? 0);
        return sum + cal * entry.quantity;
      }, 0),
    [enriched]
  );

  const handleAdd = () => {
    const id = Number(selectedIngredientId);
    const q = Number(quantity);
    if (!id || !Number.isFinite(q) || q <= 0) return;

    const ingredient = ingredients.find((item) => item.id === id);
    if (!ingredient) return;

    const existing = value.find((entry) => entry.ingredientId === id);
    if (existing) {
      onChange(
        value.map((entry) =>
          entry.ingredientId === id
            ? { ...entry, quantity: entry.quantity + q, ingredient }
            : entry
        )
      );
    } else {
      onChange([...value, { ingredientId: id, quantity: q, ingredient }]);
    }

    setSelectedIngredientId('');
    setQuantity('1');
  };

  const handleRemove = (id: number) => {
    onChange(value.filter((entry) => entry.ingredientId !== id));
  };

  return (
    <div className="space-y-4 rounded-xl border border-white/10 bg-white/5 p-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold text-white">Ingredients</h3>
          <p className="text-sm text-gray-400">Pick ingredients and quantities to compute total calories.</p>
        </div>
        <div className="text-right text-sm text-gray-300">
          <div>Total calories</div>
          <div className="text-lg font-semibold text-white">{totalCalories.toFixed(2)} kcal</div>
        </div>
      </div>

      <div className="grid gap-3 md:grid-cols-[minmax(0,1fr)_140px_auto]">
        <select
          value={selectedIngredientId}
          onChange={(e) => setSelectedIngredientId(e.target.value)}
          disabled={disabled || ingredients.length === 0}
          className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white outline-none focus:border-purple-500"
        >
          <option value="">
            {ingredients.length === 0 ? 'No ingredients available' : 'Select ingredient'}
          </option>
          {ingredients.map((ingredient) => (
            <option key={ingredient.id} value={ingredient.id}>
              {ingredient.name} ({ingredient.unit})
            </option>
          ))}
        </select>

        <input
          type="number"
          min="0.01"
          step="0.01"
          value={quantity}
          onChange={(e) => setQuantity(e.target.value)}
          disabled={disabled}
          className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white outline-none focus:border-purple-500"
          placeholder="Quantity"
        />

        <button
          type="button"
          onClick={handleAdd}
          disabled={disabled}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 px-4 py-3 font-medium text-white disabled:opacity-50"
        >
          <Plus size={18} /> Add
        </button>
      </div>

      {enriched.length > 0 ? (
        <div className="space-y-2">
          {enriched.map((entry) => (
            <div
              key={entry.ingredientId}
              className="flex flex-col gap-3 rounded-lg border border-white/10 bg-slate-950/40 px-4 py-3 md:flex-row md:items-center md:justify-between"
            >
              <div>
                <div className="text-white font-medium">{entry.ingredient?.name ?? `#${entry.ingredientId}`}</div>
                <div className="text-sm text-gray-400">
                  {entry.quantity} {entry.ingredient?.unit} ·{' '}
                  {(Number(entry.ingredient?.caloriesPerUnit ?? 0) * entry.quantity).toFixed(2)} kcal
                </div>
              </div>
              <button
                type="button"
                onClick={() => handleRemove(entry.ingredientId)}
                disabled={disabled}
                className="inline-flex items-center gap-2 self-start rounded-lg border border-red-500/40 px-3 py-2 text-sm text-red-200 hover:bg-red-500/10 disabled:opacity-50"
              >
                <X size={16} /> Remove
              </button>
            </div>
          ))}
        </div>
      ) : (
        <div className="rounded-lg border border-dashed border-white/10 px-4 py-5 text-sm text-gray-400">
          No ingredients selected yet.
        </div>
      )}
    </div>
  );
};
