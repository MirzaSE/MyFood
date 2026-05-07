import React, { useEffect, useMemo, useState } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import { getApiErrorMessage } from '../services/api';
import { ingredientService } from '../services/ingredientService';
import type { FoodIngredientInput, Ingredient } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  disabled?: boolean;
  value: FoodIngredientInput[];
  onChange: (ingredients: FoodIngredientInput[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({ disabled = false, value, onChange }) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedId, setSelectedId] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        setIsLoading(true);
        setError(null);
        setIngredients(await ingredientService.getAllIngredients());
      } catch (loadError: unknown) {
        setError(getApiErrorMessage(loadError, 'Failed to load ingredients'));
      } finally {
        setIsLoading(false);
      }
    };

    void loadIngredients();
  }, []);

  const totalCalories = useMemo(
    () => value.reduce((total, ingredient) => total + (ingredient.quantity || 0) * (ingredient.caloriesPerUnit || 0), 0),
    [value]
  );

  const availableIngredients = ingredients.filter((ingredient) => !value.some((selected) => selected.id === ingredient.id));

  const handleAdd = () => {
    const ingredient = ingredients.find((item) => item.id === Number(selectedId));
    if (!ingredient) return;

    onChange([
      ...value,
      {
        id: ingredient.id,
        name: ingredient.name,
        quantity: 1,
        unit: ingredient.unit,
        caloriesPerUnit: ingredient.caloriesPerUnit,
      },
    ]);
    setSelectedId('');
  };

  const updateQuantity = (index: number, quantity: number) => {
    onChange(value.map((ingredient, ingredientIndex) => (
      ingredientIndex === index ? { ...ingredient, quantity } : ingredient
    )));
  };

  const removeIngredient = (index: number) => {
    onChange(value.filter((_, ingredientIndex) => ingredientIndex !== index));
  };

  return (
    <div className="space-y-3">
      <div className="flex items-center justify-between gap-3">
        <div>
          <p className="text-sm font-semibold text-gray-300">Ingredients</p>
          <p className="text-xs text-gray-500">Total: {totalCalories.toFixed(2)} kcal</p>
        </div>
      </div>

      {error && <div className="rounded-lg border border-red-400/40 bg-red-500/15 px-3 py-2 text-sm text-red-100">{error}</div>}

      <div className="flex flex-col sm:flex-row gap-2">
        <select
          className="flex-1 px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-emerald-400 disabled:opacity-50"
          disabled={disabled || isLoading || availableIngredients.length === 0}
          onChange={(event) => setSelectedId(event.target.value)}
          value={selectedId}
        >
          <option className="bg-slate-900" value="">{isLoading ? 'Loading...' : 'Select ingredient'}</option>
          {availableIngredients.map((ingredient) => (
            <option className="bg-slate-900" key={ingredient.id} value={ingredient.id}>
              {ingredient.name} ({ingredient.caloriesPerUnit} kcal/{ingredient.unit})
            </option>
          ))}
        </select>
        <button
          className="inline-flex items-center justify-center gap-2 px-4 py-3 rounded-lg bg-emerald-500/20 text-emerald-100 border border-emerald-400/40 hover:bg-emerald-500/30 disabled:opacity-50"
          disabled={disabled || !selectedId}
          onClick={handleAdd}
          type="button"
        >
          <Plus size={16} />
          Add
        </button>
      </div>

      <div className="space-y-2">
        {value.length === 0 ? (
          <div className="rounded-lg border border-dashed border-white/15 px-4 py-5 text-sm text-gray-500">No ingredients selected.</div>
        ) : value.map((ingredient, index) => (
          <div className="grid grid-cols-1 sm:grid-cols-[1fr_140px_auto] gap-2 items-center rounded-lg bg-white/5 border border-white/10 p-3" key={`${ingredient.id ?? ingredient.name}-${index}`}>
            <div>
              <p className="font-semibold text-white">{ingredient.name}</p>
              <p className="text-xs text-gray-400">{ingredient.caloriesPerUnit ?? 0} kcal/{ingredient.unit || 'unit'}</p>
            </div>
            <input
              className="px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white focus:outline-none focus:border-emerald-400"
              disabled={disabled}
              min="1"
              onChange={(event) => updateQuantity(index, Number(event.target.value))}
              type="number"
              value={ingredient.quantity}
            />
            <button
              className="px-3 py-2 rounded-lg border border-red-500/40 text-red-200 hover:bg-red-500/20 disabled:opacity-50"
              disabled={disabled}
              onClick={() => removeIngredient(index)}
              type="button"
            >
              <Trash2 size={16} />
            </button>
          </div>
        ))}
      </div>
    </div>
  );
};
