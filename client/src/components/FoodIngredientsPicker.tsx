import React, { useMemo, useState } from 'react';
import { Plus, X } from 'lucide-react';
import type { FoodIngredientSelection, Ingredient } from '../types';

interface FoodIngredientsPickerProps {
  ingredients: Ingredient[];
  selectedIngredients: FoodIngredientSelection[];
  onChange: (items: FoodIngredientSelection[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  ingredients,
  selectedIngredients,
  onChange,
}) => {
  const [selectedId, setSelectedId] = useState('');

  const totalCalories = useMemo(() => {
    return selectedIngredients.reduce(
      (total, item) => total + item.quantity * item.ingredient.caloriesPerUnit,
      0
    );
  }, [selectedIngredients]);

  const totalProtein = useMemo(() => {
    return selectedIngredients.reduce(
      (total, item) => total + item.quantity * item.ingredient.protein,
      0
    );
  }, [selectedIngredients]);

  const totalCarbs = useMemo(() => {
    return selectedIngredients.reduce(
      (total, item) => total + item.quantity * item.ingredient.carbs,
      0
    );
  }, [selectedIngredients]);

  const totalFat = useMemo(() => {
    return selectedIngredients.reduce(
      (total, item) => total + item.quantity * item.ingredient.fat,
      0
    );
  }, [selectedIngredients]);

  const availableIngredients = ingredients.filter(
    (ingredient) =>
      !selectedIngredients.some((item) => item.ingredient.id === ingredient.id)
  );

  const handleAdd = () => {
    const ingredient = ingredients.find((item) => item.id === Number(selectedId));

    if (!ingredient) return;

    onChange([...selectedIngredients, { ingredient, quantity: 1 }]);
    setSelectedId('');
  };

  const handleQuantityChange = (ingredientId: number, quantity: number) => {
    onChange(
      selectedIngredients.map((item) =>
        item.ingredient.id === ingredientId
          ? { ...item, quantity: Math.max(0, quantity) }
          : item
      )
    );
  };

  const handleRemove = (ingredientId: number) => {
    onChange(
      selectedIngredients.filter((item) => item.ingredient.id !== ingredientId)
    );
  };

  return (
    <div className="mt-8 p-5 bg-white/5 border border-white/10 rounded-xl">
      <h3 className="text-lg font-bold text-white mb-4">Food Ingredients</h3>

      <div className="flex flex-col sm:flex-row gap-3 mb-5">
        <select
          value={selectedId}
          onChange={(event) => setSelectedId(event.target.value)}
          className="flex-1 px-4 py-3 bg-slate-900 border border-white/20 rounded-lg text-white focus:outline-none focus:border-purple-500"
        >
          <option value="">Select ingredient</option>
          {availableIngredients.map((ingredient) => (
            <option key={ingredient.id} value={ingredient.id}>
              {ingredient.name} ({ingredient.unit})
            </option>
          ))}
        </select>

        <button
          type="button"
          onClick={handleAdd}
          disabled={!selectedId}
          className="px-4 py-3 bg-purple-600 hover:bg-purple-700 text-white rounded-lg flex items-center justify-center gap-2 disabled:opacity-50"
        >
          <Plus size={18} />
          Add
        </button>
      </div>

      {selectedIngredients.length === 0 ? (
        <p className="text-gray-400 text-sm">No ingredients selected yet.</p>
      ) : (
        <div className="space-y-3">
          {selectedIngredients.map((item) => (
            <div
              key={item.ingredient.id}
              className="flex flex-col sm:flex-row sm:items-center gap-3 p-3 bg-slate-900/70 border border-white/10 rounded-lg"
            >
              <div className="flex-1">
                <p className="text-white font-semibold">{item.ingredient.name}</p>
                <p className="text-gray-400 text-sm">
                  {item.ingredient.caloriesPerUnit} kcal per {item.ingredient.unit}
                </p>
              </div>

              <input
                type="number"
                min="0"
                step="0.01"
                value={item.quantity}
                onChange={(event) =>
                  handleQuantityChange(
                    item.ingredient.id,
                    Number(event.target.value)
                  )
                }
                className="w-full sm:w-28 px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
              />

              <button
                type="button"
                onClick={() => handleRemove(item.ingredient.id)}
                className="px-3 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 rounded-lg"
              >
                <X size={18} />
              </button>
            </div>
          ))}
        </div>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-4 gap-3 mt-5">
        <div className="p-3 bg-white/10 rounded-lg">
          <p className="text-gray-400 text-xs">Total Calories</p>
          <p className="text-white font-bold">{totalCalories.toFixed(2)} kcal</p>
        </div>

        <div className="p-3 bg-white/10 rounded-lg">
          <p className="text-gray-400 text-xs">Protein</p>
          <p className="text-white font-bold">{totalProtein.toFixed(2)} g</p>
        </div>

        <div className="p-3 bg-white/10 rounded-lg">
          <p className="text-gray-400 text-xs">Carbs</p>
          <p className="text-white font-bold">{totalCarbs.toFixed(2)} g</p>
        </div>

        <div className="p-3 bg-white/10 rounded-lg">
          <p className="text-gray-400 text-xs">Fat</p>
          <p className="text-white font-bold">{totalFat.toFixed(2)} g</p>
        </div>
      </div>
    </div>
  );
};