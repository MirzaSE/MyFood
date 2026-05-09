import React, { useState, useEffect } from 'react';
import { Plus, X, AlertCircle } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

export interface SelectedIngredient {
  ingredientId: number;
  quantity: number;
  name: string;
}

interface FoodIngredientsPickerProps {
  selectedIngredients: SelectedIngredient[];
  onIngredientsChange: (ingredients: SelectedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  selectedIngredients,
  onIngredientsChange,
}) => {
  const [availableIngredients, setAvailableIngredients] = useState<Ingredient[]>([]);
  const [isLoadingIngredients, setIsLoadingIngredients] = useState(true);
  const [selectedIngredientId, setSelectedIngredientId] = useState<string>('');
  const [quantity, setQuantity] = useState<string>('1');
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadIngredients();
  }, []);

  const loadIngredients = async () => {
    try {
      setIsLoadingIngredients(true);
      setError(null);
      const response = await ingredientService.getAllIngredients(1, 100);
      setAvailableIngredients(response.value);
    } catch (err: any) {
      setError('Failed to load ingredients');
    } finally {
      setIsLoadingIngredients(false);
    }
  };

  const handleAddIngredient = () => {
    if (!selectedIngredientId || !quantity) {
      setError('Please select an ingredient and enter a quantity');
      return;
    }

    const qty = parseFloat(quantity);
    if (qty <= 0) {
      setError('Quantity must be greater than 0');
      return;
    }

    const ingredient = availableIngredients.find(
      (ing) => ing.id.toString() === selectedIngredientId
    );

    if (!ingredient) {
      setError('Ingredient not found');
      return;
    }

    // Check if already selected
    if (
      selectedIngredients.some((ing) => ing.ingredientId === ingredient.id)
    ) {
      setError('Ingredient already added');
      return;
    }

    const newIngredient: SelectedIngredient = {
      ingredientId: ingredient.id,
      quantity: qty,
      name: ingredient.name,
    };

    onIngredientsChange([...selectedIngredients, newIngredient]);
    setSelectedIngredientId('');
    setQuantity('1');
    setError(null);
  };

  const handleRemoveIngredient = (ingredientId: number) => {
    onIngredientsChange(
      selectedIngredients.filter((ing) => ing.ingredientId !== ingredientId)
    );
  };

  const handleQuantityChange = (ingredientId: number, newQuantity: string) => {
    const qty = parseFloat(newQuantity);
    if (qty > 0) {
      onIngredientsChange(
        selectedIngredients.map((ing) =>
          ing.ingredientId === ingredientId
            ? { ...ing, quantity: qty }
            : ing
        )
      );
    }
  };

  return (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-2">
          Add Ingredients
        </label>

        {/* Error Message */}
        {error && (
          <div className="mb-3 p-3 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-2 backdrop-blur">
            <AlertCircle size={18} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200 text-sm">{error}</p>
          </div>
        )}

        {/* Input Controls */}
        <div className="flex gap-2 mb-4">
          <select
            value={selectedIngredientId}
            onChange={(e) => setSelectedIngredientId(e.target.value)}
            disabled={isLoadingIngredients}
            className="flex-1 px-3 py-2 bg-gray-700 border border-gray-600 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">
              {isLoadingIngredients ? 'Loading...' : 'Select ingredient'}
            </option>
            {availableIngredients.map((ing) => (
              <option key={ing.id} value={ing.id}>
                {ing.name}
              </option>
            ))}
          </select>

          <input
            type="number"
            step="0.1"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            placeholder="Qty"
            min="0.1"
            className="w-20 px-3 py-2 bg-gray-700 border border-gray-600 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
          />

          <button
            onClick={handleAddIngredient}
            disabled={isLoadingIngredients}
            className="flex items-center space-x-1 px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white rounded-lg transition"
          >
            <Plus size={18} />
            <span>Add</span>
          </button>
        </div>
      </div>

      {/* Selected Ingredients List */}
      {selectedIngredients.length > 0 && (
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-2">
            Selected Ingredients ({selectedIngredients.length})
          </label>
          <div className="space-y-2">
            {selectedIngredients.map((ing) => (
              <div
                key={ing.ingredientId}
                className="flex items-center justify-between bg-gray-700 p-3 rounded-lg"
              >
                <div className="flex-1">
                  <p className="text-white font-medium">{ing.name}</p>
                  <div className="flex items-center space-x-2 mt-1">
                    <input
                      type="number"
                      step="0.1"
                      value={ing.quantity}
                      onChange={(e) =>
                        handleQuantityChange(ing.ingredientId, e.target.value)
                      }
                      min="0.1"
                      className="w-20 px-2 py-1 bg-gray-600 border border-gray-500 rounded text-white text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                    <span className="text-gray-400 text-sm">units</span>
                  </div>
                </div>
                <button
                  onClick={() => handleRemoveIngredient(ing.ingredientId)}
                  className="p-2 text-red-400 hover:bg-red-500/20 rounded transition"
                  title="Remove"
                >
                  <X size={18} />
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
