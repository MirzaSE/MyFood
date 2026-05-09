import { useState, useEffect } from 'react';
import type { Ingredient } from '../types/ingredient';
import type { FoodIngredient } from '../types';
import { ingredientService } from '../services/ingredientService';

interface FoodIngredientsPickerProps {
  onIngredientsChange: (ingredients: FoodIngredient[], nutrition: {
    calories: number;
    protein: number;
    carbs: number;
    fat: number;
  }) => void;
}

export function FoodIngredientsPicker({ onIngredientsChange }: FoodIngredientsPickerProps) {
  const [availableIngredients, setAvailableIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedIngredients, setSelectedIngredients] = useState<(FoodIngredient & { ingredient: Ingredient })[]>([]);
  const [newIngredientId, setNewIngredientId] = useState('');
  const [newQuantity, setNewQuantity] = useState('1');

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        const data = await ingredientService.getAll(1, 100);
        setAvailableIngredients(data);
      } catch (error) {
        console.error('Failed to load ingredients:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadIngredients();
  }, []);

  const calculateNutrition = (ingredients: (FoodIngredient & { ingredient: Ingredient })[]) => {
    const nutrition = ingredients.reduce(
      (acc, fi) => ({
        calories: acc.calories + fi.ingredient.caloriesPerUnit * fi.quantity,
        protein: acc.protein + fi.ingredient.protein * fi.quantity,
        carbs: acc.carbs + fi.ingredient.carbs * fi.quantity,
        fat: acc.fat + fi.ingredient.fat * fi.quantity,
      }),
      { calories: 0, protein: 0, carbs: 0, fat: 0 }
    );

    const foodIngredients: FoodIngredient[] = ingredients.map(fi => ({
      ingredientId: fi.ingredientId,
      quantity: fi.quantity,
    }));

    onIngredientsChange(foodIngredients, nutrition);
  };

  const handleAddIngredient = () => {
    if (!newIngredientId) return;

    const ingredient = availableIngredients.find(i => i.id === parseInt(newIngredientId));
    if (!ingredient) return;

    const quantity = parseFloat(newQuantity) || 1;

    const exists = selectedIngredients.find(fi => fi.ingredientId === ingredient.id);
    if (exists) {
      const updated = selectedIngredients.map(fi =>
        fi.ingredientId === ingredient.id
          ? { ...fi, quantity: fi.quantity + quantity }
          : fi
      );
      setSelectedIngredients(updated);
      calculateNutrition(updated);
    } else {
      const newSelected = [
        ...selectedIngredients,
        {
          ingredientId: ingredient.id,
          quantity,
          ingredient,
        },
      ];
      setSelectedIngredients(newSelected);
      calculateNutrition(newSelected);
    }

    setNewIngredientId('');
    setNewQuantity('1');
  };

  const handleRemoveIngredient = (ingredientId: number) => {
    const updated = selectedIngredients.filter(fi => fi.ingredientId !== ingredientId);
    setSelectedIngredients(updated);
    calculateNutrition(updated);
  };

  const handleQuantityChange = (ingredientId: number, quantity: number) => {
    const updated = selectedIngredients.map(fi =>
      fi.ingredientId === ingredientId
        ? { ...fi, quantity }
        : fi
    );
    setSelectedIngredients(updated);
    calculateNutrition(updated);
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <div className="w-6 h-6 border-2 border-purple-500 border-t-transparent rounded-full animate-spin"></div>
        <span className="ml-2 text-gray-300">Loading ingredients...</span>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col space-y-3">
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
          <select
            value={newIngredientId}
            onChange={e => setNewIngredientId(e.target.value)}
            className="px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400"
          >
            <option value="" className="bg-slate-800">Select ingredient...</option>
            {availableIngredients.map(ing => (
              <option key={ing.id} value={ing.id} className="bg-slate-800">
                {ing.name} ({ing.caloriesPerUnit} cal/{ing.unit})
              </option>
            ))}
          </select>
          <input
            type="number"
            min="0.01"
            step="0.01"
            value={newQuantity}
            onChange={e => setNewQuantity(e.target.value)}
            placeholder="Quantity"
            className="px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400"
          />
          <button
            onClick={handleAddIngredient}
            disabled={!newIngredientId}
            className="px-4 py-2 bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
          >
            Add
          </button>
        </div>
      </div>

      {selectedIngredients.length > 0 && (
        <div className="space-y-3">
          <h4 className="font-semibold text-white">Selected Ingredients:</h4>
          <div className="space-y-2 max-h-60 overflow-y-auto">
            {selectedIngredients.map(fi => (
              <div
                key={fi.ingredientId}
                className="flex items-center justify-between bg-white/5 p-3 rounded-lg border border-white/10"
              >
                <div className="flex-1">
                  <div className="font-medium text-white">{fi.ingredient.name}</div>
                  <div className="text-sm text-gray-400">
                    {fi.quantity} {fi.ingredient.unit} × {fi.ingredient.caloriesPerUnit} cal/{fi.ingredient.unit}
                  </div>
                </div>
                <div className="flex items-center space-x-2">
                  <input
                    type="number"
                    min="0.01"
                    step="0.01"
                    value={fi.quantity}
                    onChange={e => handleQuantityChange(fi.ingredientId, parseFloat(e.target.value) || 0)}
                    className="w-20 px-2 py-1 bg-white/10 border border-white/20 rounded text-white text-sm focus:outline-none focus:border-purple-500"
                  />
                  <button
                    onClick={() => handleRemoveIngredient(fi.ingredientId)}
                    className="text-red-400 hover:text-red-300 transition-colors"
                  >
                    Remove
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {selectedIngredients.length === 0 && (
        <div className="text-center py-8 text-gray-400">
          No ingredients selected. Add some ingredients to see nutrition information.
        </div>
      )}
    </div>
  );
}
