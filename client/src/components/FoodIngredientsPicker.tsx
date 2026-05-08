import React from 'react';
import type { Ingredient, SelectedIngredientInput } from '../types';

interface FoodIngredientsPickerProps {
  ingredients: Ingredient[];
  selected: SelectedIngredientInput[];
  onChange: (next: SelectedIngredientInput[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  ingredients,
  selected,
  onChange,
}) => {
  const addIngredient = (ingredientId: number) => {
    if (!ingredientId || selected.some((x) => x.ingredientId === ingredientId)) return;
    onChange([...selected, { ingredientId, quantity: 1 }]);
  };

  const updateQuantity = (ingredientId: number, quantity: number) => {
    onChange(
      selected.map((x) =>
        x.ingredientId === ingredientId ? { ...x, quantity: Math.max(1, quantity) } : x
      )
    );
  };

  const remove = (ingredientId: number) => {
    onChange(selected.filter((x) => x.ingredientId !== ingredientId));
  };

  const totalCalories = selected.reduce((sum, item) => {
    const ingredient = ingredients.find((x) => x.id === item.ingredientId);
    if (!ingredient) return sum;
    // In current backend model, ingredient.quantity is used as calories-per-unit fallback.
    return sum + ingredient.quantity * item.quantity;
  }, 0);

  return (
    <div className="space-y-3 p-3 rounded-lg border border-white/10 bg-white/5">
      <div className="flex items-center gap-2">
        <select
          defaultValue=""
          onChange={(e) => addIngredient(Number(e.target.value))}
          className="flex-1 px-3 py-2 rounded bg-white/10 border border-white/20 text-white"
        >
          <option value="" className="bg-slate-900">
            Add ingredient...
          </option>
          {ingredients.map((ingredient) => (
            <option key={ingredient.id} value={ingredient.id} className="bg-slate-900">
              {ingredient.name}
            </option>
          ))}
        </select>
      </div>

      {selected.map((item) => {
        const ingredient = ingredients.find((x) => x.id === item.ingredientId);
        if (!ingredient) return null;
        return (
          <div key={item.ingredientId} className="flex items-center gap-2">
            <span className="flex-1 text-sm text-gray-200">{ingredient.name}</span>
            <input
              type="number"
              min={1}
              value={item.quantity}
              onChange={(e) => updateQuantity(item.ingredientId, Number(e.target.value))}
              className="w-20 px-2 py-1 rounded bg-white/10 border border-white/20 text-white"
            />
            <button
              type="button"
              className="px-2 py-1 text-xs rounded border border-red-400/40 text-red-300"
              onClick={() => remove(item.ingredientId)}
            >
              Remove
            </button>
          </div>
        );
      })}

      <div className="text-sm text-gray-300">Calculated total calories: {totalCalories}</div>
    </div>
  );
};
