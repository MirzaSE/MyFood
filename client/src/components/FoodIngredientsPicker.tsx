import React, { useState, useMemo } from 'react';
import type { Ingredient, IngredientPickerItem } from '../types/ingredient';

interface Props {
  availableIngredients: Ingredient[];
  selectedItems: IngredientPickerItem[];
  onItemsChange: (items: IngredientPickerItem[]) => void;
}

export const FoodIngredientsPicker: React.FC<Props> = ({
  availableIngredients,
  selectedItems,
  onItemsChange,
}) => {
  const [search, setSearch] = useState('');
  const [showDropdown, setShowDropdown] = useState(false);

  const filteredIngredients = useMemo(() => {
    if (!search) return availableIngredients;
    return availableIngredients.filter((ing) =>
      ing.name.toLowerCase().includes(search.toLowerCase())
    );
  }, [availableIngredients, search]);

  const addIngredient = (ingredient: Ingredient) => {
    const exists = selectedItems.find((item) => item.ingredient.id === ingredient.id);
    if (exists) return;
    onItemsChange([...selectedItems, { ingredient, quantity: 100 }]);
    setSearch('');
    setShowDropdown(false);
  };

  const removeIngredient = (id: number) => {
    onItemsChange(selectedItems.filter((item) => item.ingredient.id !== id));
  };

  const updateQuantity = (id: number, quantity: number) => {
    onItemsChange(
      selectedItems.map((item) =>
        item.ingredient.id === id ? { ...item, quantity } : item
      )
    );
  };

  const totals = useMemo(() => {
    return selectedItems.reduce(
      (acc, item) => {
        const ratio = item.quantity / 100;
        return {
          calories: acc.calories + item.ingredient.caloriesPerUnit * ratio,
          protein: acc.protein + item.ingredient.protein * ratio,
          carbs: acc.carbs + item.ingredient.carbs * ratio,
          fat: acc.fat + item.ingredient.fat * ratio,
        };
      },
      { calories: 0, protein: 0, carbs: 0, fat: 0 }
    );
  }, [selectedItems]);

  return (
    <div className="space-y-4">
      <label className="block text-sm font-medium text-gray-300">Ingredients</label>
      
      <div className="relative">
        <input
          type="text"
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setShowDropdown(true);
          }}
          onFocus={() => setShowDropdown(true)}
          placeholder="Search and add ingredients..."
          className="w-full bg-slate-700/60 border border-white/10 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/60"
        />
        {showDropdown && filteredIngredients.length > 0 && (
          <div className="absolute z-10 w-full mt-1 bg-slate-800 border border-white/10 rounded-lg shadow-lg max-h-48 overflow-auto">
            {filteredIngredients.map((ing) => (
              <button
                key={ing.id}
                onClick={() => addIngredient(ing)}
                className="w-full px-4 py-2 text-left text-white hover:bg-purple-600/30 transition-colors"
              >
                {ing.name} ({ing.unit})
              </button>
            ))}
          </div>
        )}
      </div>

      {selectedItems.length > 0 && (
        <div className="space-y-2">
          {selectedItems.map((item) => (
            <div key={item.ingredient.id} className="flex items-center gap-3 bg-slate-700/30 p-3 rounded-lg">
              <span className="flex-1 text-white">{item.ingredient.name}</span>
              <input
                type="number"
                value={item.quantity}
                onChange={(e) => updateQuantity(item.ingredient.id, Number(e.target.value))}
                className="w-20 bg-slate-700/60 border border-white/10 rounded px-2 py-1 text-white text-sm text-center"
                min="1"
              />
              <span className="text-gray-400 text-sm">{item.ingredient.unit}</span>
              <button
                onClick={() => removeIngredient(item.ingredient.id)}
                className="text-red-400 hover:text-red-300 p-1"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          ))}
        </div>
      )}

      {selectedItems.length > 0 && (
        <div className="bg-slate-700/30 p-4 rounded-lg space-y-2">
          <div className="text-sm font-semibold text-gray-300">Total Nutrition</div>
          <div className="grid grid-cols-4 gap-2 text-sm">
            <div className="text-center">
              <div className="text-gray-400">Calories</div>
              <div className="text-white font-medium">{Math.round(totals.calories)}</div>
            </div>
            <div className="text-center">
              <div className="text-gray-400">Protein</div>
              <div className="text-white font-medium">{Math.round(totals.protein * 10) / 10}g</div>
            </div>
            <div className="text-center">
              <div className="text-gray-400">Carbs</div>
              <div className="text-white font-medium">{Math.round(totals.carbs * 10) / 10}g</div>
            </div>
            <div className="text-center">
              <div className="text-gray-400">Fat</div>
              <div className="text-white font-medium">{Math.round(totals.fat * 10) / 10}g</div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};