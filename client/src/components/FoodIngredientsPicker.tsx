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

  const selectedIngredients = useMemo(
    () => value.map((entry) => ({ ...entry, ingredient: ingredients.find((item) => item.id === entry.ingredientId) })),
    [ingredients, value]
  );

  const totalCalories = useMemo(
    () =>
      selectedIngredients.reduce((sum, entry) => {
        const caloriesPerUnit = entry.ingredient?.caloriesPerUnit ?? 0;
        return sum + caloriesPerUnit * entry.quantity;
      }, 0),
    [selectedIngredients]
  );

  const handleAddIngredient = () => {
    const ingredientId = Number(selectedIngredientId);
    const parsedQuantity = Number(quantity);

    if (!ingredientId || parsedQuantity <= 0) {
      return;
    }

    const ingredient = ingredients.find((item) => item.id === ingredientId);
    if (!ingredient) {
      return;
    }

    const existing = value.find((entry) => entry.ingredientId === ingredientId);
    if (existing) {
      onChange(
        value.map((entry) =>
          entry.ingredientId === ingredientId
            ? { ...entry, quantity: entry.quantity + parsedQuantity, ingredient }
            : entry
        )
      );
    } else {
      onChange([...value, { ingredientId, quantity: parsedQuantity, ingredient }]);
    }

    setSelectedIngredientId('');
    setQuantity('1');
  };

  const handleRemoveIngredient = (ingredientId: number) => {
    onChange(value.filter((entry) => entry.ingredientId !== ingredientId));
  };

  return (
    <div className="space-y-4 rounded-xl border border-white/10 bg-white/5 p-4">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold text-white">Ingredients</h3>
          <p className="text-sm text-gray-400">Add ingredients and quantities for this food.</p>
        </div>
        <div className="text-right text-sm text-gray-300">
          <div>Total calories</div>
          <div className="text-lg font-semibold text-white">{totalCalories.toFixed(2)} kcal</div>
        </div>
      </div>

      <div className="grid gap-3 md:grid-cols-[minmax(0,1fr)_140px_auto]">
        <select
          value={selectedIngredientId}
          onChange={(event) => setSelectedIngredientId(event.target.value)}
          disabled={disabled}
          className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white outline-none"
        >
          <option value="">Select ingredient</option>
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
          onChange={(event) => setQuantity(event.target.value)}
          disabled={disabled}
          className="w-full rounded-lg border border-white/20 bg-slate-900 px-4 py-3 text-white outline-none"
          placeholder="Quantity"
        />

        <button
          type="button"
          onClick={handleAddIngredient}
          disabled={disabled}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-4 py-3 font-medium text-white disabled:opacity-50"
        >
          <Plus size={18} />
          Add
        </button>
      </div>

      {selectedIngredients.length > 0 ? (
        <div className="space-y-2">
          {selectedIngredients.map((entry) => (
            <div
              key={entry.ingredientId}
              className="flex flex-col gap-3 rounded-lg border border-white/10 bg-slate-950/40 px-4 py-3 md:flex-row md:items-center md:justify-between"
            >
              <div>
                <div className="text-white font-medium">{entry.ingredient?.name}</div>
                <div className="text-sm text-gray-400">
                  {entry.quantity} {entry.ingredient?.unit} · {((entry.ingredient?.caloriesPerUnit ?? 0) * entry.quantity).toFixed(2)} kcal
                </div>
              </div>
              <button
                type="button"
                onClick={() => handleRemoveIngredient(entry.ingredientId)}
                disabled={disabled}
                className="inline-flex items-center gap-2 self-start rounded-lg border border-red-500/40 px-3 py-2 text-sm text-red-200 hover:bg-red-500/10 disabled:opacity-50"
              >
                <X size={16} />
                Remove
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