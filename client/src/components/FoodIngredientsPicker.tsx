import React, { useState, useEffect } from 'react';
import { X, Search } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientPickerItem } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  selected: IngredientPickerItem[];
  onChange: (items: IngredientPickerItem[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  selected,
  onChange,
}) => {
  const [allIngredients, setAllIngredients] = useState<Ingredient[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [showDropdown, setShowDropdown] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    loadIngredients();
  }, []);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      const data = await ingredientService.getAllIngredients(1, 100);
      setAllIngredients(data);
    } catch {
      // silently fail
    } finally {
      setIsLoading(false);
    }
  };

  const filteredIngredients = allIngredients.filter(
    ing =>
      !selected.some(s => s.ingredient.id === ing.id) &&
      ing.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const addIngredient = (ingredient: Ingredient) => {
    onChange([...selected, { ingredient, quantity: 1 }]);
    setSearchQuery('');
    setShowDropdown(false);
  };

  const removeIngredient = (id: number) => {
    onChange(selected.filter(s => s.ingredient.id !== id));
  };

  const updateQuantity = (id: number, quantity: number) => {
    onChange(
      selected.map(s =>
        s.ingredient.id === id ? { ...s, quantity: Math.max(1, quantity) } : s
      )
    );
  };

  const totalCalories = selected.reduce(
    (sum, s) => sum + s.ingredient.caloriesPerUnit * s.quantity,
    0
  );

  return (
    <div className="space-y-3">
      <div className="relative">
        <div className="flex items-center px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus-within:border-purple-500 focus-within:ring-2 focus-within:ring-purple-500/30 transition-all">
          <Search size={16} className="text-gray-400 mr-2 flex-shrink-0" />
          <input
            type="text"
            value={searchQuery}
            onChange={e => {
              setSearchQuery(e.target.value);
              setShowDropdown(true);
            }}
            onFocus={() => setShowDropdown(true)}
            placeholder={isLoading ? 'Loading...' : 'Search and add ingredients...'}
            className="flex-1 bg-transparent text-white placeholder-gray-400 focus:outline-none text-sm"
            disabled={isLoading}
          />
        </div>

        {showDropdown && filteredIngredients.length > 0 && (
          <div className="absolute z-10 w-full mt-1 bg-slate-800 border border-white/20 rounded-lg shadow-xl max-h-48 overflow-y-auto">
            {filteredIngredients.map(ing => (
              <button
                key={ing.id}
                type="button"
                onClick={() => addIngredient(ing)}
                className="w-full flex items-center justify-between px-3 py-2 hover:bg-white/10 text-left transition-colors"
              >
                <span className="text-white text-sm">{ing.name}</span>
                <span className="text-gray-400 text-xs">
                  {ing.caloriesPerUnit} kcal/{ing.unit}
                </span>
              </button>
            ))}
          </div>
        )}

        {showDropdown && filteredIngredients.length === 0 && searchQuery && (
          <div
            className="absolute z-10 w-full mt-1 bg-slate-800 border border-white/20 rounded-lg px-3 py-2 text-gray-400 text-sm cursor-pointer"
            onClick={() => setShowDropdown(false)}
          >
            No ingredients found
          </div>
        )}
      </div>

      {selected.length > 0 && (
        <div className="space-y-2">
          {selected.map(item => (
            <div
              key={item.ingredient.id}
              className="flex items-center space-x-2 bg-white/5 border border-white/10 rounded-lg px-3 py-2"
            >
              <span className="flex-1 text-white text-sm font-medium">{item.ingredient.name}</span>
              <span className="text-gray-400 text-xs">{item.ingredient.unit}</span>
              <input
                type="number"
                min={1}
                value={item.quantity}
                onChange={e => updateQuantity(item.ingredient.id, parseInt(e.target.value) || 1)}
                className="w-16 px-2 py-1 bg-white/10 border border-white/20 rounded text-white text-sm text-center focus:outline-none focus:border-purple-500 transition-all"
              />
              <span className="text-gray-400 text-xs w-20 text-right">
                {(item.ingredient.caloriesPerUnit * item.quantity).toFixed(1)} kcal
              </span>
              <button
                type="button"
                onClick={() => removeIngredient(item.ingredient.id)}
                className="text-gray-400 hover:text-red-400 transition-colors p-1"
              >
                <X size={14} />
              </button>
            </div>
          ))}

          <div className="flex justify-end pt-1 border-t border-white/10">
            <span className="text-sm font-semibold text-purple-400">
              Total: {totalCalories.toFixed(1)} kcal
            </span>
          </div>
        </div>
      )}

      {selected.length === 0 && (
        <p className="text-gray-500 text-xs text-center py-2">No ingredients selected</p>
      )}
    </div>
  );
};
