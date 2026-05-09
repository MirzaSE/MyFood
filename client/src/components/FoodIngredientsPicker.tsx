import React, { useEffect, useMemo, useState } from 'react';
import { Plus, X, Search } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { FoodIngredientLine, Ingredient } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  selectedLines: FoodIngredientLine[];
  onChange: (lines: FoodIngredientLine[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  selectedLines,
  onChange,
}) => {
  const [available, setAvailable] = useState<Ingredient[]>([]);
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let alive = true;
    const load = async () => {
      try {
        setIsLoading(true);
        setError(null);
        if (search.trim()) {
          const results = await ingredientService.search(search.trim());
          if (alive) setAvailable(results);
        } else {
          const { items } = await ingredientService.getAll(1, 25);
          if (alive) setAvailable(items);
        }
      } catch (err: any) {
        if (alive) setError(err?.response?.data?.message || 'Failed to load ingredients');
      } finally {
        if (alive) setIsLoading(false);
      }
    };

    const t = setTimeout(load, search ? 250 : 0);
    return () => {
      alive = false;
      clearTimeout(t);
    };
  }, [search]);

  const selectedIds = useMemo(
    () => new Set(selectedLines.map((l) => l.ingredient.id)),
    [selectedLines]
  );

  const addIngredient = (ing: Ingredient) => {
    if (selectedIds.has(ing.id)) return;
    onChange([...selectedLines, { ingredient: ing, quantity: 1 }]);
  };

  const removeLine = (id: number) => {
    onChange(selectedLines.filter((l) => l.ingredient.id !== id));
  };

  const setQuantity = (id: number, quantity: number) => {
    onChange(
      selectedLines.map((l) =>
        l.ingredient.id === id ? { ...l, quantity: Math.max(0, quantity) } : l
      )
    );
  };

  const totals = useMemo(() => {
    const acc = { calories: 0, protein: 0, carbs: 0, fat: 0 };
    for (const line of selectedLines) {
      const q = Number.isFinite(line.quantity) ? line.quantity : 0;
      acc.calories += line.ingredient.caloriesPerUnit * q;
      acc.protein += line.ingredient.protein * q;
      acc.carbs += line.ingredient.carbs * q;
      acc.fat += line.ingredient.fat * q;
    }
    return acc;
  }, [selectedLines]);

  return (
    <div className="space-y-5">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">
          Add ingredients
        </label>
        <div className="relative">
          <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search by name…"
            className="w-full pl-10 pr-4 py-2.5 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-sm"
          />
        </div>

        {error && (
          <p className="mt-2 text-xs text-red-300">{error}</p>
        )}

        <div className="mt-3 max-h-44 overflow-y-auto rounded-lg border border-white/10 divide-y divide-white/5 bg-slate-900/40">
          {isLoading ? (
            <p className="px-4 py-3 text-sm text-gray-400">Loading…</p>
          ) : available.length === 0 ? (
            <p className="px-4 py-3 text-sm text-gray-400">No ingredients available.</p>
          ) : (
            available.map((ing) => {
              const already = selectedIds.has(ing.id);
              return (
                <div
                  key={ing.id}
                  className="flex items-center justify-between px-4 py-2 hover:bg-white/5 transition-colors"
                >
                  <div>
                    <p className="text-sm text-white font-medium">{ing.name}</p>
                    <p className="text-xs text-gray-400">
                      {ing.caloriesPerUnit.toFixed(2)} kcal / {ing.unit}
                    </p>
                  </div>
                  <button
                    type="button"
                    onClick={() => addIngredient(ing)}
                    disabled={already}
                    className="flex items-center space-x-1 px-2.5 py-1 bg-purple-500/20 hover:bg-purple-500/40 text-purple-200 rounded-md border border-purple-500/30 text-xs disabled:opacity-40"
                  >
                    <Plus size={14} />
                    <span>{already ? 'Added' : 'Add'}</span>
                  </button>
                </div>
              );
            })
          )}
        </div>
      </div>

      {selectedLines.length > 0 && (
        <div>
          <label className="block text-sm font-semibold text-gray-300 mb-2">
            Selected ({selectedLines.length})
          </label>
          <div className="space-y-2">
            {selectedLines.map((line) => (
              <div
                key={line.ingredient.id}
                className="flex items-center space-x-3 px-3 py-2 bg-white/5 border border-white/10 rounded-lg"
              >
                <div className="flex-1 min-w-0">
                  <p className="text-sm text-white font-medium truncate">{line.ingredient.name}</p>
                  <p className="text-xs text-gray-400">
                    {line.ingredient.caloriesPerUnit.toFixed(2)} kcal / {line.ingredient.unit}
                  </p>
                </div>
                <div className="flex items-center space-x-2">
                  <input
                    type="number"
                    min="0"
                    step="0.1"
                    value={line.quantity}
                    onChange={(e) => setQuantity(line.ingredient.id, Number(e.target.value))}
                    className="w-20 px-2 py-1 bg-slate-800 border border-white/20 rounded-md text-white text-sm text-right focus:outline-none focus:border-purple-500"
                  />
                  <span className="text-xs text-gray-400 w-8">{line.ingredient.unit}</span>
                  <button
                    type="button"
                    onClick={() => removeLine(line.ingredient.id)}
                    className="p-1.5 text-red-300 hover:text-red-200 hover:bg-red-500/20 rounded-md transition-colors"
                    title="Remove"
                  >
                    <X size={16} />
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 pt-3 border-t border-white/10">
        <div className="text-center">
          <p className="text-xs text-gray-400 uppercase tracking-wide">Calories</p>
          <p className="text-lg font-bold text-white">{totals.calories.toFixed(1)}</p>
        </div>
        <div className="text-center">
          <p className="text-xs text-gray-400 uppercase tracking-wide">Protein</p>
          <p className="text-lg font-bold text-white">{totals.protein.toFixed(1)} g</p>
        </div>
        <div className="text-center">
          <p className="text-xs text-gray-400 uppercase tracking-wide">Carbs</p>
          <p className="text-lg font-bold text-white">{totals.carbs.toFixed(1)} g</p>
        </div>
        <div className="text-center">
          <p className="text-xs text-gray-400 uppercase tracking-wide">Fat</p>
          <p className="text-lg font-bold text-white">{totals.fat.toFixed(1)} g</p>
        </div>
      </div>
    </div>
  );
};
