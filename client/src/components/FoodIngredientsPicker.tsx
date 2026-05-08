import { useEffect, useMemo, useState } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, SelectedFoodIngredient } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  selectedIngredients: SelectedFoodIngredient[];
  onChange: (ingredients: SelectedFoodIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  selectedIngredients,
  onChange,
}) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredientId, setSelectedIngredientId] = useState('');
  const [quantity, setQuantity] = useState(1);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        setIsLoading(true);
        const data = await ingredientService.getIngredients(1, 100);
        setIngredients(data);
      } finally {
        setIsLoading(false);
      }
    };

    loadIngredients();
  }, []);

  const totalCalories = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.caloriesPerUnit,
      0
    );
  }, [selectedIngredients]);

  const totalProtein = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.protein,
      0
    );
  }, [selectedIngredients]);

  const totalCarbs = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.carbs,
      0
    );
  }, [selectedIngredients]);

  const totalFat = useMemo(() => {
    return selectedIngredients.reduce(
      (sum, item) => sum + item.quantity * item.fat,
      0
    );
  }, [selectedIngredients]);

  const addIngredient = () => {
    const ingredient = ingredients.find(
      (item) => item.id === Number(selectedIngredientId)
    );

    if (!ingredient || quantity <= 0) {
      return;
    }

    const nextSelected: SelectedFoodIngredient = {
      ingredientId: ingredient.id,
      name: ingredient.name,
      unit: ingredient.unit,
      quantity,
      caloriesPerUnit: ingredient.caloriesPerUnit,
      protein: ingredient.protein,
      carbs: ingredient.carbs,
      fat: ingredient.fat,
    };

    onChange([...selectedIngredients, nextSelected]);
    setSelectedIngredientId('');
    setQuantity(1);
  };

  const removeIngredient = (index: number) => {
    onChange(selectedIngredients.filter((_, itemIndex) => itemIndex !== index));
  };

  const updateQuantity = (index: number, nextQuantity: number) => {
    if (nextQuantity <= 0) {
      return;
    }

    onChange(
      selectedIngredients.map((item, itemIndex) =>
        itemIndex === index ? { ...item, quantity: nextQuantity } : item
      )
    );
  };

  return (
    <div className="rounded-xl border border-gray-200 bg-white p-4">
      <h3 className="mb-3 text-lg font-semibold text-gray-900">
        Ingredients
      </h3>

      {isLoading ? (
        <p className="text-sm text-gray-500">Loading ingredients...</p>
      ) : (
        <div className="mb-4 grid grid-cols-1 gap-3 md:grid-cols-3">
          <select
            value={selectedIngredientId}
            onChange={(event) => setSelectedIngredientId(event.target.value)}
            className="rounded-lg border border-gray-300 px-3 py-2 md:col-span-2"
          >
            <option value="">Select ingredient</option>
            {ingredients.map((ingredient) => (
              <option key={ingredient.id} value={ingredient.id}>
                {ingredient.name} ({ingredient.unit})
              </option>
            ))}
          </select>

          <div className="flex gap-2">
            <input
              type="number"
              min="0.01"
              step="0.01"
              value={quantity}
              onChange={(event) => setQuantity(Number(event.target.value))}
              className="w-full rounded-lg border border-gray-300 px-3 py-2"
              placeholder="Qty"
            />
            <button
              type="button"
              onClick={addIngredient}
              className="rounded-lg bg-blue-600 px-3 py-2 text-white"
            >
              <Plus size={18} />
            </button>
          </div>
        </div>
      )}

      {selectedIngredients.length === 0 ? (
        <p className="text-sm text-gray-500">
          No ingredients selected yet.
        </p>
      ) : (
        <div className="space-y-3">
          {selectedIngredients.map((item, index) => (
            <div
              key={`${item.ingredientId}-${index}`}
              className="flex items-center justify-between rounded-lg border border-gray-200 p-3"
            >
              <div>
                <p className="font-medium text-gray-900">{item.name}</p>
                <p className="text-sm text-gray-500">
                  {Math.round(item.quantity * item.caloriesPerUnit * 100) / 100} calories
                </p>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="number"
                  min="0.01"
                  step="0.01"
                  value={item.quantity}
                  onChange={(event) =>
                    updateQuantity(index, Number(event.target.value))
                  }
                  className="w-24 rounded-lg border border-gray-300 px-2 py-1"
                />
                <span className="text-sm text-gray-500">{item.unit}</span>
                <button
                  type="button"
                  onClick={() => removeIngredient(index)}
                  className="rounded-lg p-2 text-red-600 hover:bg-red-50"
                >
                  <Trash2 size={18} />
                </button>
              </div>
            </div>
          ))}

          <div className="rounded-lg bg-gray-50 p-3 text-sm">
            <p><strong>Total calories:</strong> {Math.round(totalCalories * 100) / 100}</p>
            <p><strong>Total protein:</strong> {Math.round(totalProtein * 100) / 100}</p>
            <p><strong>Total carbs:</strong> {Math.round(totalCarbs * 100) / 100}</p>
            <p><strong>Total fat:</strong> {Math.round(totalFat * 100) / 100}</p>
          </div>
        </div>
      )}
    </div>
  );
};