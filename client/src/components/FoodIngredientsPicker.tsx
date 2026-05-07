import React, { useMemo, useState } from 'react';
import { Plus, X } from 'lucide-react';
import type { Ingredient, IngredientPickerLine } from '../types/ingredient';

export interface FoodIngredientsTotals {
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
}

// eslint-disable-next-line react-refresh/only-export-components
export function computePickerTotals(lines: IngredientPickerLine[]): FoodIngredientsTotals {
  return lines.reduce(
    (acc, line) => {
      const q = line.quantity;
      if (q <= 0) return acc;
      return {
        calories: acc.calories + q * line.ingredient.caloriesPerUnit,
        protein: acc.protein + q * line.ingredient.protein,
        carbs: acc.carbs + q * line.ingredient.carbs,
        fat: acc.fat + q * line.ingredient.fat,
      };
    },
    { calories: 0, protein: 0, carbs: 0, fat: 0 },
  );
}

interface FoodIngredientsPickerProps {
  catalog: Ingredient[];
  lines: IngredientPickerLine[];
  onChange: (lines: IngredientPickerLine[]) => void;
  disabled?: boolean;
  isLoading?: boolean;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  catalog,
  lines,
  onChange,
  disabled = false,
  isLoading = false,
}) => {
  const [addId, setAddId] = useState<string>('');

  const idsInUse = useMemo(() => new Set(lines.map((l) => l.ingredient.id)), [lines]);

  const available = useMemo(
    () => catalog.filter((i) => !idsInUse.has(i.id)),
    [catalog, idsInUse],
  );

  const totals = useMemo(() => computePickerTotals(lines), [lines]);

  const addLine = () => {
    const id = Number(addId);
    if (!id) return;
    const ing = catalog.find((i) => i.id === id);
    if (!ing || idsInUse.has(id)) return;
    onChange([...lines, { ingredient: ing, quantity: 1 }]);
    setAddId('');
  };

  const updateQuantity = (index: number, quantity: number) => {
    const q = Number.isFinite(quantity) && quantity > 0 ? quantity : 0.0001;
    const next = [...lines];
    next[index] = { ...next[index], quantity: q };
    onChange(next);
  };

  const removeLine = (index: number) => {
    onChange(lines.filter((_, i) => i !== index));
  };

  return (
    <div className="rounded-xl border border-white/10 bg-white/5 p-6 space-y-6">
      <div>
        <h3 className="text-lg font-semibold text-white mb-1">Ingredients</h3>
        <p className="text-sm text-gray-400">Add ingredients and quantities to estimate nutrition.</p>
      </div>

      <div className="flex flex-col sm:flex-row gap-3">
        <select
          value={addId}
          onChange={(e) => setAddId(e.target.value)}
          disabled={disabled || isLoading || available.length === 0}
          className="flex-1 min-w-0 px-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white disabled:opacity-50"
        >
          <option value="">
            {isLoading ? 'Loading catalog…' : available.length === 0 ? 'No more ingredients to add' : 'Select ingredient…'}
          </option>
          {available.map((i) => (
            <option key={i.id} value={i.id}>
              {i.name} ({i.unit})
            </option>
          ))}
        </select>
        <button
          type="button"
          onClick={addLine}
          disabled={disabled || !addId || isLoading}
          className="inline-flex items-center justify-center gap-2 px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg font-medium disabled:opacity-50"
        >
          <Plus size={18} />
          Add
        </button>
      </div>

      {lines.length === 0 ? (
        <p className="text-sm text-gray-500 py-4 text-center border border-dashed border-white/10 rounded-lg">
          No ingredients added yet.
        </p>
      ) : (
        <ul className="space-y-3">
          {lines.map((line, index) => (
            <li
              key={`${line.ingredient.id}-${index}`}
              className="flex flex-wrap items-center gap-3 p-3 rounded-lg bg-white/5 border border-white/10"
            >
              <span className="flex-1 min-w-[140px] font-medium text-white">{line.ingredient.name}</span>
              <label className="flex items-center gap-2 text-sm text-gray-300">
                <span>Qty ({line.ingredient.unit})</span>
                <input
                  type="number"
                  min={0.0001}
                  step="any"
                  value={line.quantity}
                  onChange={(e) => updateQuantity(index, Number(e.target.value))}
                  disabled={disabled}
                  className="w-28 px-2 py-1 bg-white/10 border border-white/20 rounded text-white"
                />
              </label>
              <span className="text-sm text-gray-400">
                {Math.round(line.quantity * line.ingredient.caloriesPerUnit * 100) / 100} kcal
              </span>
              <button
                type="button"
                onClick={() => removeLine(index)}
                disabled={disabled}
                className="p-2 rounded-lg bg-red-500/20 text-red-300 hover:bg-red-500/30 disabled:opacity-50"
                title="Remove"
              >
                <X size={18} />
              </button>
            </li>
          ))}
        </ul>
      )}

      <div className="rounded-lg bg-gradient-to-r from-purple-900/40 to-blue-900/40 border border-white/10 p-4">
        <p className="text-xs uppercase tracking-wide text-gray-400 mb-2">Totals from selection</p>
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 text-sm">
          <div>
            <span className="text-gray-400 block">Calories</span>
            <span className="text-xl font-bold text-white">
              {Math.round(totals.calories * 100) / 100}
            </span>
          </div>
          <div>
            <span className="text-gray-400 block">Protein</span>
            <span className="text-lg text-emerald-300">{Math.round(totals.protein * 100) / 100}</span>
          </div>
          <div>
            <span className="text-gray-400 block">Carbs</span>
            <span className="text-lg text-amber-300">{Math.round(totals.carbs * 100) / 100}</span>
          </div>
          <div>
            <span className="text-gray-400 block">Fat</span>
            <span className="text-lg text-rose-300">{Math.round(totals.fat * 100) / 100}</span>
          </div>
        </div>
      </div>
    </div>
  );
};
