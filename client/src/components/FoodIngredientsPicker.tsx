import React, { useEffect, useMemo, useState } from 'react';
import { Plus, X, Search } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, PickedIngredient } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  picked: PickedIngredient[];
  onChange: (picked: PickedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  picked,
  onChange,
}) => {
  const [available, setAvailable] = useState<Ingredient[]>([]);
  const [searchInput, setSearchInput] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    (async () => {
      setIsLoading(true);
      setError(null);
      try {
        const term = searchInput.trim();
        const items = term
          ? await ingredientService.search(term)
          : (await ingredientService.getAll(1, 50)).items;
        if (active) setAvailable(items);
      } catch (err: unknown) {
        const e = err as { response?: { data?: { message?: string } } };
        if (active) setError(e.response?.data?.message ?? 'Failed to load ingredients.');
      } finally {
        if (active) setIsLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, [searchInput]);

  const pickedIds = useMemo(() => new Set(picked.map((p) => p.ingredient.id)), [picked]);

  const addIngredient = (ingredient: Ingredient) => {
    if (pickedIds.has(ingredient.id)) return;
    onChange([...picked, { ingredient, quantity: 1 }]);
  };

  const removeIngredient = (id: number) => {
    onChange(picked.filter((p) => p.ingredient.id !== id));
  };

  const updateQuantity = (id: number, quantity: number) => {
    if (quantity < 0 || isNaN(quantity)) return;
    onChange(picked.map((p) => (p.ingredient.id === id ? { ...p, quantity } : p)));
  };

  const totals = useMemo(() => {
    let calories = 0;
    let protein = 0;
    let carbs = 0;
    let fat = 0;
    for (const { ingredient, quantity } of picked) {
      calories += quantity * ingredient.caloriesPerUnit;
      protein += quantity * (ingredient.protein ?? 0);
      carbs += quantity * (ingredient.carbs ?? 0);
      fat += quantity * (ingredient.fat ?? 0);
    }
    return {
      calories: +calories.toFixed(2),
      protein: +protein.toFixed(2),
      carbs: +carbs.toFixed(2),
      fat: +fat.toFixed(2),
    };
  }, [picked]);

  return (
    <div className="space-y-4 border border-white/10 rounded-xl p-4 bg-white/5">
      <h3 className="text-base font-semibold text-white">Ingredients</h3>

      {/* Search & catalog */}
      <div>
        <div className="relative mb-3">
          <Search
            size={16}
            className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none"
          />
          <input
            type="text"
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            placeholder="Search to add an ingredient…"
            className="w-full pl-9 pr-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 text-white placeholder-gray-400 text-sm"
          />
        </div>

        {error && <p className="text-red-300 text-xs mb-2">{error}</p>}

        <div className="max-h-40 overflow-y-auto space-y-1 pr-1">
          {isLoading ? (
            <p className="text-gray-400 text-sm py-2">Loading…</p>
          ) : available.length === 0 ? (
            <p className="text-gray-400 text-sm py-2">No matching ingredients.</p>
          ) : (
            available.map((ing) => {
              const isPicked = pickedIds.has(ing.id);
              return (
                <button
                  key={ing.id}
                  type="button"
                  onClick={() => addIngredient(ing)}
                  disabled={isPicked}
                  className="w-full flex items-center justify-between px-3 py-2 bg-white/5 hover:bg-white/10 disabled:bg-emerald-500/10 disabled:hover:bg-emerald-500/10 rounded-lg border border-white/10 transition-all text-left disabled:cursor-not-allowed"
                >
                  <div>
                    <span className="text-white text-sm font-medium">{ing.name}</span>
                    <span className="text-gray-400 text-xs ml-2">
                      {ing.caloriesPerUnit} kcal / {ing.unit ?? 'unit'}
                    </span>
                  </div>
                  {isPicked ? (
                    <span className="text-emerald-300 text-xs">Added</span>
                  ) : (
                    <Plus size={16} className="text-purple-300" />
                  )}
                </button>
              );
            })
          )}
        </div>
      </div>

      {/* Picked list */}
      <div>
        <p className="text-sm text-gray-300 mb-2">
          {picked.length} ingredient{picked.length === 1 ? '' : 's'} selected
        </p>
        {picked.length === 0 ? (
          <p className="text-gray-500 text-sm italic">No ingredients picked yet.</p>
        ) : (
          <div className="space-y-2">
            {picked.map(({ ingredient, quantity }) => (
              <div
                key={ingredient.id}
                className="flex items-center gap-2 px-3 py-2 bg-white/5 rounded-lg border border-white/10"
              >
                <div className="flex-1 min-w-0">
                  <p className="text-white text-sm font-medium truncate">{ingredient.name}</p>
                  <p className="text-gray-400 text-xs">
                    {ingredient.caloriesPerUnit} kcal × qty
                  </p>
                </div>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  value={quantity}
                  onChange={(e) => updateQuantity(ingredient.id, Number(e.target.value))}
                  className="w-20 px-2 py-1 bg-white/10 border border-white/20 rounded text-white text-sm focus:outline-none focus:border-purple-500"
                />
                <span className="text-gray-400 text-xs w-8">{ingredient.unit ?? ''}</span>
                <button
                  type="button"
                  onClick={() => removeIngredient(ingredient.id)}
                  className="p-1.5 text-red-300 hover:text-red-200 hover:bg-red-500/20 rounded transition-all"
                  aria-label="Remove"
                >
                  <X size={16} />
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Totals */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-2 pt-3 border-t border-white/10">
        <div className="text-center">
          <p className="text-xs text-gray-400">Calories</p>
          <p className="text-lg font-bold text-white">{totals.calories}</p>
        </div>
        <div className="text-center">
          <p className="text-xs text-gray-400">Protein</p>
          <p className="text-lg font-bold text-white">{totals.protein}g</p>
        </div>
        <div className="text-center">
          <p className="text-xs text-gray-400">Carbs</p>
          <p className="text-lg font-bold text-white">{totals.carbs}g</p>
        </div>
        <div className="text-center">
          <p className="text-xs text-gray-400">Fat</p>
          <p className="text-lg font-bold text-white">{totals.fat}g</p>
        </div>
      </div>
    </div>
  );
};
