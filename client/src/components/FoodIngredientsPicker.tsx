import React, { useEffect, useMemo, useState } from 'react';
import { Plus, Trash2, Search } from 'lucide-react';
import type { FoodIngredientPick, Ingredient } from '../types/ingredient';
import { ingredientService } from '../services/ingredientService';

interface FoodIngredientsPickerProps {
  value: FoodIngredientPick[];
  onChange: (next: FoodIngredientPick[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({ value, onChange }) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    const load = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const result = search.trim()
          ? await ingredientService.searchIngredients(search.trim(), 1, 50)
          : await ingredientService.getAllIngredients(1, 50);
        if (!cancelled) {
          setIngredients(result.items);
        }
      } catch (err: any) {
        if (!cancelled) {
          setError(err?.response?.data?.message ?? 'Failed to load ingredients');
        }
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };
    const handle = setTimeout(load, 250);
    return () => {
      cancelled = true;
      clearTimeout(handle);
    };
  }, [search]);

  const ingredientById = useMemo(() => {
    const map = new Map<number, Ingredient>();
    ingredients.forEach((i) => map.set(i.id, i));
    return map;
  }, [ingredients]);

  const totalCalories = useMemo(() => {
    return value.reduce((sum, pick) => {
      const ing = ingredientById.get(pick.ingredientId);
      if (!ing) return sum;
      return sum + ing.caloriesPerUnit * pick.quantity;
    }, 0);
  }, [value, ingredientById]);

  const addPick = (ingredient: Ingredient) => {
    if (value.find((p) => p.ingredientId === ingredient.id)) return;
    onChange([...value, { ingredientId: ingredient.id, quantity: 1 }]);
  };

  const updateQuantity = (id: number, quantity: number) => {
    onChange(value.map((p) => (p.ingredientId === id ? { ...p, quantity } : p)));
  };

  const remove = (id: number) => {
    onChange(value.filter((p) => p.ingredientId !== id));
  };

  return (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-semibold text-gray-300 mb-2">Add Ingredients</label>
        <div className="relative">
          <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search ingredients…"
            className="w-full pl-10 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 text-white placeholder-gray-400 transition-all"
          />
        </div>
        {error && <p className="text-red-400 text-xs mt-1">{error}</p>}
      </div>

      <div className="max-h-48 overflow-y-auto rounded-lg border border-white/10 bg-slate-900/40">
        {isLoading ? (
          <p className="px-4 py-6 text-center text-gray-400 text-sm">Loading…</p>
        ) : ingredients.length === 0 ? (
          <p className="px-4 py-6 text-center text-gray-400 text-sm">No ingredients available.</p>
        ) : (
          <ul className="divide-y divide-white/5">
            {ingredients.map((ingredient) => {
              const isPicked = value.some((p) => p.ingredientId === ingredient.id);
              return (
                <li key={ingredient.id} className="flex items-center justify-between px-4 py-2">
                  <div>
                    <p className="text-white text-sm font-medium">{ingredient.name}</p>
                    <p className="text-gray-400 text-xs">
                      {ingredient.caloriesPerUnit} kcal / {ingredient.unit}
                    </p>
                  </div>
                  <button
                    type="button"
                    onClick={() => addPick(ingredient)}
                    disabled={isPicked}
                    className="flex items-center space-x-1 text-xs px-3 py-1 bg-purple-500/20 hover:bg-purple-500/30 text-purple-300 rounded-lg border border-purple-500/30 disabled:opacity-40"
                  >
                    <Plus size={14} />
                    <span>{isPicked ? 'Added' : 'Add'}</span>
                  </button>
                </li>
              );
            })}
          </ul>
        )}
      </div>

      {value.length > 0 && (
        <div className="rounded-lg border border-white/10 bg-slate-900/40">
          <div className="px-4 py-2 border-b border-white/10 text-xs uppercase tracking-wider text-gray-400">
            Selected
          </div>
          <ul className="divide-y divide-white/5">
            {value.map((pick) => {
              const ingredient = ingredientById.get(pick.ingredientId);
              return (
                <li key={pick.ingredientId} className="flex items-center px-4 py-3 gap-3">
                  <div className="flex-1">
                    <p className="text-white text-sm font-medium">
                      {ingredient ? ingredient.name : `#${pick.ingredientId}`}
                    </p>
                    <p className="text-gray-400 text-xs">
                      {ingredient
                        ? `${(ingredient.caloriesPerUnit * pick.quantity).toFixed(1)} kcal`
                        : ''}
                    </p>
                  </div>
                  <input
                    type="number"
                    step="0.1"
                    min={0}
                    value={pick.quantity}
                    onChange={(e) => updateQuantity(pick.ingredientId, Number(e.target.value) || 0)}
                    className="w-20 px-2 py-1 bg-white/10 border border-white/20 rounded text-white text-sm"
                  />
                  <span className="text-gray-400 text-xs w-10">{ingredient?.unit ?? ''}</span>
                  <button
                    type="button"
                    onClick={() => remove(pick.ingredientId)}
                    className="p-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 rounded-lg border border-red-500/30"
                    title="Remove"
                  >
                    <Trash2 size={14} />
                  </button>
                </li>
              );
            })}
          </ul>
          <div className="px-4 py-3 border-t border-white/10 flex justify-between items-center bg-purple-500/10">
            <span className="text-gray-300 text-sm font-medium">Total calories</span>
            <span className="text-white font-bold">{totalCalories.toFixed(1)} kcal</span>
          </div>
        </div>
      )}
    </div>
  );
};
