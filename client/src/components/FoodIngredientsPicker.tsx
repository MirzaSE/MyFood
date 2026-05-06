import React from 'react';
import type { Ingredient, FoodIngredientSelection } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  ingredients: Ingredient[];
  selected: FoodIngredientSelection[];
  onChange: (selected: FoodIngredientSelection[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({ ingredients, selected, onChange }) => {
  const addIngredient = (ingredientId: number) => {
    if (selected.some((item) => item.ingredientId === ingredientId)) {
      return;
    }

    onChange([...selected, { ingredientId, quantity: 1 }]);
  };

  const updateQuantity = (ingredientId: number, quantity: number) => {
    onChange(selected.map((item) => item.ingredientId === ingredientId ? { ...item, quantity } : item));
  };

  const removeIngredient = (ingredientId: number) => {
    onChange(selected.filter((item) => item.ingredientId !== ingredientId));
  };

  const totalCalories = selected.reduce((acc, item) => {
    const ingredient = ingredients.find((x) => x.id === item.ingredientId);
    return acc + ((ingredient?.caloriesPerUnit ?? 0) * item.quantity);
  }, 0);

  return (
    <div className="space-y-3 p-4 rounded-lg border border-slate-700 bg-slate-900/60">
      <h3 className="text-lg text-white font-semibold">Ingredients</h3>
      <select className="w-full p-2 rounded bg-slate-800 text-white border border-slate-600" onChange={(e) => addIngredient(Number(e.target.value))} defaultValue="">
        <option value="" disabled>Select ingredient</option>
        {ingredients.map((i) => <option key={i.id} value={i.id}>{i.name} ({i.unit})</option>)}
      </select>
      {selected.map((item) => {
        const ingredient = ingredients.find((x) => x.id === item.ingredientId);
        if (!ingredient) return null;

        return (
          <div key={item.ingredientId} className="flex items-center gap-2">
            <span className="text-gray-200 w-44">{ingredient.name}</span>
            <input type="number" min={0.01} step={0.01} value={item.quantity} onChange={(e) => updateQuantity(item.ingredientId, Number(e.target.value))} className="w-24 p-2 rounded bg-slate-800 border border-slate-600 text-white" />
            <span className="text-gray-400">{ingredient.unit}</span>
            <button type="button" className="px-2 py-1 rounded bg-red-700 text-white" onClick={() => removeIngredient(item.ingredientId)}>Remove</button>
          </div>
        );
      })}
      <p className="text-green-300 font-medium">Total calories: {totalCalories.toFixed(2)}</p>
    </div>
  );
};
