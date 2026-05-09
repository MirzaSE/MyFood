import React, { useEffect, useMemo, useState } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, SelectedIngredient } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  selected: SelectedIngredient[];
  onChange: (selected: SelectedIngredient[]) => void;
  disabled?: boolean;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  selected,
  onChange,
  disabled = false,
}) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedId, setSelectedId] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        setIsLoading(true);
        setIngredients(await ingredientService.getAllIngredients(1, 50));
      } finally {
        setIsLoading(false);
      }
    };

    loadIngredients();
  }, []);

  const availableIngredients = useMemo(
    () => ingredients.filter((ingredient) => ingredient.quantity > 0 && !selected.some((item) => item.ingredient.id === ingredient.id)),
    [ingredients, selected],
  );

  const totals = selected.reduce(
    (total, item) => ({
      quantity: total.quantity + item.quantity,
      calories: total.calories + item.ingredient.caloriesPerUnit * item.quantity,
      protein: total.protein + item.ingredient.protein * item.quantity,
      carbs: total.carbs + item.ingredient.carbs * item.quantity,
      fat: total.fat + item.ingredient.fat * item.quantity,
    }),
    { quantity: 0, calories: 0, protein: 0, carbs: 0, fat: 0 },
  );

  const addIngredient = () => {
    const ingredient = ingredients.find((item) => item.id === Number(selectedId));
    if (!ingredient) return;

    onChange([...selected, { ingredient, quantity: 1 }]);
    setSelectedId('');
  };

  const updateQuantity = (id: number, quantity: number) => {
    onChange(
      selected.map((item) =>
        item.ingredient.id === id
          ? { ...item, quantity: Math.min(item.ingredient.quantity, Number.isFinite(quantity) && quantity > 0 ? quantity : 1) }
          : item,
      ),
    );
  };

  const removeIngredient = (id: number) => {
    onChange(selected.filter((item) => item.ingredient.id !== id));
  };

  return (
    <div className="space-y-3 rounded-lg border border-white/10 bg-white/[0.03] p-4">
      <div className="flex items-center justify-between gap-3">
        <label className="text-sm font-semibold text-gray-300">Ingredients</label>
        <span className="text-xs text-gray-400">Total quantity: {totals.quantity}</span>
      </div>

      <div className="flex gap-2">
        <select
          value={selectedId}
          onChange={(event) => setSelectedId(event.target.value)}
          disabled={disabled || isLoading}
          className="min-w-0 flex-1 px-3 py-2 bg-slate-950 border border-white/20 rounded-lg focus:outline-none focus:border-emerald-500 text-white disabled:opacity-50"
        >
          <option value="">{isLoading ? 'Loading ingredients...' : 'Select ingredient'}</option>
          {availableIngredients.map((ingredient) => (
            <option key={ingredient.id} value={ingredient.id}>
              {ingredient.name} ({ingredient.quantity} {ingredient.unit} available)
            </option>
          ))}
        </select>
        <button
          type="button"
          onClick={addIngredient}
          disabled={disabled || !selectedId}
          className="inline-flex items-center justify-center w-11 h-11 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg disabled:opacity-50"
          title="Add ingredient"
          aria-label="Add ingredient"
        >
          <Plus size={18} />
        </button>
      </div>

      {selected.length > 0 && (
        <div className="space-y-2">
          {selected.map((item) => (
            <div key={item.ingredient.id} className="flex items-center gap-2 rounded-lg bg-slate-950/70 p-2">
              <span className="min-w-0 flex-1 text-sm text-white truncate">{item.ingredient.name}</span>
              <span className="hidden sm:inline text-xs text-gray-400">
                {item.ingredient.caloriesPerUnit * item.quantity} kcal
              </span>
              <input
                type="number"
                min="1"
                max={item.ingredient.quantity}
                value={item.quantity}
                onChange={(event) => updateQuantity(item.ingredient.id, Number(event.target.value))}
                disabled={disabled}
                className="w-20 px-2 py-1 bg-white/10 border border-white/20 rounded text-white"
              />
              <span className="w-14 text-xs text-gray-400">
                / {item.ingredient.quantity}
              </span>
              <button
                type="button"
                onClick={() => removeIngredient(item.ingredient.id)}
                disabled={disabled}
                className="inline-flex items-center justify-center w-9 h-9 text-red-300 hover:text-red-200 disabled:opacity-50"
                title="Remove ingredient"
                aria-label="Remove ingredient"
              >
                <Trash2 size={16} />
              </button>
            </div>
          ))}
        </div>
      )}

      {selected.length > 0 && (
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-2 pt-2">
          {[
            ['Calories', `${Math.round(totals.calories)} kcal`],
            ['Protein', `${totals.protein.toFixed(1)}g`],
            ['Carbs', `${totals.carbs.toFixed(1)}g`],
            ['Fat', `${totals.fat.toFixed(1)}g`],
          ].map(([label, value]) => (
            <div key={label} className="rounded-lg border border-white/10 bg-slate-950/60 p-2">
              <p className="text-xs text-gray-400">{label}</p>
              <p className="text-sm font-semibold text-white">{value}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
