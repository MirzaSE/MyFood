import React, { useState, useEffect } from 'react';
import { Search, Plus, X } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, SelectedIngredient } from '../types/ingredient';

interface Props {
  selected: SelectedIngredient[];
  onChange: (selected: SelectedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<Props> = ({ selected, onChange }) => {
  const [allIngredients, setAllIngredients] = useState<Ingredient[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    ingredientService.getAll(1, 100).then(({ data }) => {
      setAllIngredients(data);
      setIsLoading(false);
    }).catch(() => setIsLoading(false));
  }, []);

  const filtered = allIngredients.filter(ing =>
    ing.name.toLowerCase().includes(searchTerm.toLowerCase()) &&
    !selected.some(s => s.ingredient.id === ing.id)
  );

  const totalCalories = selected.reduce(
    (sum, s) => sum + s.ingredient.caloriesPerUnit * s.quantity, 0
  );

  const addIngredient = (ing: Ingredient) => {
    onChange([...selected, { ingredient: ing, quantity: 1 }]);
    setSearchTerm('');
  };

  const removeIngredient = (id: number) => {
    onChange(selected.filter(s => s.ingredient.id !== id));
  };

  const updateQuantity = (id: number, qty: number) => {
    onChange(selected.map(s => s.ingredient.id === id ? { ...s, quantity: Math.max(1, qty) } : s));
  };

  return (
    <div className="space-y-4">
      <div className="relative">
        <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
        <input
          value={searchTerm}
          onChange={e => setSearchTerm(e.target.value)}
          placeholder="Search ingredients to add..."
          className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {searchTerm && !isLoading && filtered.length > 0 && (
        <div className="border border-gray-200 rounded-lg divide-y divide-gray-100 max-h-48 overflow-y-auto shadow-sm">
          {filtered.map(ing => (
            <button
              key={ing.id}
              type="button"
              onClick={() => addIngredient(ing)}
              className="w-full flex items-center justify-between px-4 py-2.5 hover:bg-blue-50 text-left transition-colors"
            >
              <span className="font-medium text-gray-800 text-sm">{ing.name}</span>
              <div className="flex items-center gap-3 text-xs text-gray-500">
                <span>{ing.caloriesPerUnit} cal/{ing.unit}</span>
                <Plus size={14} className="text-blue-500" />
              </div>
            </button>
          ))}
        </div>
      )}

      {selected.length > 0 && (
        <div className="space-y-2">
          <p className="text-sm font-medium text-gray-700">Selected ingredients:</p>
          {selected.map(s => (
            <div key={s.ingredient.id} className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg">
              <span className="flex-1 text-sm font-medium text-gray-800">{s.ingredient.name}</span>
              <span className="text-xs text-gray-500">{s.ingredient.unit}</span>
              <div className="flex items-center gap-1">
                <label className="text-xs text-gray-500">Qty:</label>
                <input
                  type="number"
                  min="1"
                  value={s.quantity}
                  onChange={e => updateQuantity(s.ingredient.id, parseInt(e.target.value) || 1)}
                  className="w-16 border border-gray-300 rounded px-2 py-1 text-sm text-center focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
              </div>
              <span className="text-xs text-green-600 font-medium w-20 text-right">
                {(s.ingredient.caloriesPerUnit * s.quantity).toFixed(1)} cal
              </span>
              <button
                type="button"
                onClick={() => removeIngredient(s.ingredient.id)}
                className="text-red-400 hover:text-red-600 transition-colors"
              >
                <X size={16} />
              </button>
            </div>
          ))}

          <div className="flex justify-end pt-2 border-t border-gray-200">
            <span className="text-sm font-semibold text-gray-700">
              Total: <span className="text-blue-600">{totalCalories.toFixed(1)} cal</span>
            </span>
          </div>
        </div>
      )}

      {selected.length === 0 && !searchTerm && (
        <p className="text-sm text-gray-400 text-center py-4">
          Search and add ingredients above.
        </p>
      )}
    </div>
  );
};
