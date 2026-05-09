import React, { useState, useEffect } from 'react';
import { Plus, X, Leaf } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

interface SelectedIngredient {
  ingredientId: number;
  name: string;
  quantity: string;
}

interface FoodIngredientsPickerProps {
  selectedIngredients: SelectedIngredient[];
  onChange: (ingredients: SelectedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  selectedIngredients,
  onChange,
}) => {
  const [allIngredients, setAllIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedId, setSelectedId] = useState<string>('');

  useEffect(() => {
    loadIngredients();
  }, []);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      const data = await ingredientService.getAllIngredients();
      setAllIngredients(data);
    } catch (err) {
      console.error('Failed to load ingredients', err);
    } finally {
      setIsLoading(false);
    }
  };

  const availableIngredients = allIngredients.filter(
    (ing) => !selectedIngredients.some((s) => s.ingredientId === ing.id)
  );

  const handleAdd = () => {
    const id = parseInt(selectedId);
    const ingredient = allIngredients.find((i) => i.id === id);
    if (!ingredient) return;

    onChange([
      ...selectedIngredients,
      { ingredientId: ingredient.id, name: ingredient.name, quantity: '1' },
    ]);
    setSelectedId('');
  };

  const handleRemove = (ingredientId: number) => {
    onChange(selectedIngredients.filter((s) => s.ingredientId !== ingredientId));
  };

  const handleQuantityChange = (ingredientId: number, quantity: string) => {
    onChange(
      selectedIngredients.map((s) =>
        s.ingredientId === ingredientId ? { ...s, quantity } : s
      )
    );
  };

  if (isLoading) {
    return <p className="text-gray-400 text-sm">Loading ingredients...</p>;
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center space-x-2">
        <Leaf size={18} className="text-purple-400" />
        <h3 className="text-sm font-semibold text-gray-300">Ingredients</h3>
      </div>

      {/* Add ingredient selector */}
      <div className="flex space-x-2">
        <select
          value={selectedId}
          onChange={(e) => setSelectedId(e.target.value)}
          className="flex-1 px-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 transition-all"
        >
          <option value="" className="bg-slate-800">Select ingredient...</option>
          {availableIngredients.map((ing) => (
            <option key={ing.id} value={ing.id} className="bg-slate-800">
              {ing.name}
            </option>
          ))}
        </select>
        <button
          type="button"
          onClick={handleAdd}
          disabled={!selectedId}
          className="px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <Plus size={18} />
        </button>
      </div>

      {/* Selected ingredients list */}
      {selectedIngredients.length > 0 && (
        <div className="space-y-2">
          {selectedIngredients.map((item) => (
            <div
              key={item.ingredientId}
              className="flex items-center space-x-3 p-3 bg-white/5 border border-white/10 rounded-lg"
            >
              <span className="flex-1 text-white text-sm font-medium">{item.name}</span>
              <input
                type="text"
                value={item.quantity}
                onChange={(e) => handleQuantityChange(item.ingredientId, e.target.value)}
                className="w-24 px-3 py-1 bg-white/10 border border-white/20 rounded-lg text-white text-sm focus:outline-none focus:border-purple-500 transition-all"
                placeholder="Qty"
              />
              <button
                type="button"
                onClick={() => handleRemove(item.ingredientId)}
                className="p-1 text-red-400 hover:text-red-300 hover:bg-red-500/20 rounded-lg transition-all"
              >
                <X size={16} />
              </button>
            </div>
          ))}

          {/* Total summary */}
          <div className="p-3 bg-purple-500/10 border border-purple-500/20 rounded-lg">
            <p className="text-sm text-purple-300">
              Total ingredients: <span className="font-semibold text-white">{selectedIngredients.length}</span>
            </p>
          </div>
        </div>
      )}

      {selectedIngredients.length === 0 && (
        <p className="text-gray-500 text-sm italic">No ingredients added yet</p>
      )}
    </div>
  );
};
