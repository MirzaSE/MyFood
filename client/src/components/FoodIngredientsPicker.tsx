import React, { useState } from 'react';
import { Plus, X } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface SelectedIngredient {
  ingredient: Ingredient;
  quantity: number;
}

interface FoodIngredientsPickerProps {
  availableIngredients: Ingredient[];
  selectedIngredients: SelectedIngredient[];
  onAdd: (ingredient: Ingredient, quantity: number) => void;
  onRemove: (ingredientId: number) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  availableIngredients,
  selectedIngredients,
  onAdd,
  onRemove,
}) => {
  const [selectedId, setSelectedId] = useState<string>('');
  const [quantity, setQuantity] = useState<string>('1');

  const selectedIds = new Set(selectedIngredients.map(si => si.ingredient.id));
  const unselectedIngredients = availableIngredients.filter(i => !selectedIds.has(i.id));

  const totalCalories = selectedIngredients.reduce(
    (sum, si) => sum + si.ingredient.caloriesPerUnit * si.quantity,
    0
  );

  const handleAdd = () => {
    const id = Number(selectedId);
    const qty = Number(quantity);

    if (!id || qty <= 0) return;

    const ingredient = availableIngredients.find(i => i.id === id);
    if (!ingredient) return;

    onAdd(ingredient, qty);
    setSelectedId('');
    setQuantity('1');
  };

  return (
    <div className="space-y-4">
      <h3 className="text-sm font-semibold text-gray-300 uppercase tracking-wider">Ingredients</h3>

      {/* Picker row */}
      <div className="flex items-center space-x-2">
        <select
          value={selectedId}
          onChange={e => setSelectedId(e.target.value)}
          className="flex-1 px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white transition-all"
        >
          <option value="" className="bg-slate-800">Select an ingredient...</option>
          {unselectedIngredients.map(ingredient => (
            <option key={ingredient.id} value={ingredient.id} className="bg-slate-800">
              {ingredient.name} ({ingredient.unit}) — {ingredient.caloriesPerUnit} cal
            </option>
          ))}
        </select>

        <input
          type="number"
          min={1}
          value={quantity}
          onChange={e => setQuantity(e.target.value)}
          className="w-20 px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white transition-all text-center"
          placeholder="Qty"
        />

        <button
          type="button"
          onClick={handleAdd}
          disabled={!selectedId || Number(quantity) <= 0}
          className="p-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Plus size={18} />
        </button>
      </div>

      {/* Selected ingredients list */}
      {selectedIngredients.length > 0 && (
        <div className="space-y-2">
          {selectedIngredients.map(({ ingredient, quantity: qty }) => (
            <div
              key={ingredient.id}
              className="flex items-center justify-between bg-white/5 border border-white/10 rounded-lg px-3 py-2"
            >
              <div className="flex items-center space-x-3">
                <span className="text-white font-medium">{ingredient.name}</span>
                <span className="text-gray-400 text-sm">
                  {qty} {ingredient.unit}
                </span>
                <span className="text-purple-400 text-sm">
                  {ingredient.caloriesPerUnit * qty} cal
                </span>
              </div>
              <button
                type="button"
                onClick={() => onRemove(ingredient.id)}
                className="p-1 text-red-400 hover:text-red-300 hover:bg-red-500/20 rounded transition-all duration-200"
              >
                <X size={14} />
              </button>
            </div>
          ))}

          {/* Total calories */}
          <div className="flex justify-end pt-2 border-t border-white/10">
            <span className="text-sm font-semibold text-gray-300">
              Total:{' '}
              <span className="text-white">{totalCalories} calories</span>
            </span>
          </div>
        </div>
      )}

      {selectedIngredients.length === 0 && (
        <p className="text-gray-500 text-sm text-center py-4">
          No ingredients selected. Add ingredients above.
        </p>
      )}
    </div>
  );
};
