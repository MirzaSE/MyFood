import React from 'react';
import { Search, Check } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface FoodIngredientsPickerProps {
  ingredients: Ingredient[];
  selectedIngredientIds: number[];
  onChange: (ids: number[]) => void;
  disabled?: boolean;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({ ingredients, selectedIngredientIds, onChange, disabled = false }) => {
  const [query, setQuery] = React.useState('');
  const filtered = ingredients.filter((ingredient) => ingredient.name.toLowerCase().includes(query.toLowerCase()));

  const toggle = (id: number) => {
    if (disabled) return;
    onChange(selectedIngredientIds.includes(id) ? selectedIngredientIds.filter((currentId) => currentId !== id) : [...selectedIngredientIds, id]);
  };

  return (
    <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
      <div className="mb-3 flex items-center justify-between gap-3">
        <div>
          <p className="text-sm font-semibold text-white">Ingredients</p>
          <p className="text-xs text-gray-400">Pick ingredients to attach to this food.</p>
        </div>
        <div className="text-xs text-cyan-200">{selectedIngredientIds.length} selected</div>
      </div>
      <div className="mb-3 flex items-center gap-2 rounded-lg border border-white/10 bg-slate-950/40 px-3 py-2">
        <Search size={16} className="text-gray-400" />
        <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search ingredients" disabled={disabled} className="w-full bg-transparent text-sm text-white placeholder-gray-500 focus:outline-none" />
      </div>
      <div className="max-h-56 overflow-auto pr-1">
        {filtered.length === 0 ? (
          <p className="py-6 text-center text-sm text-gray-400">No ingredients match.</p>
        ) : (
          <div className="grid gap-2">
            {filtered.map((ingredient) => {
              const isSelected = selectedIngredientIds.includes(ingredient.id);
              return (
                <button key={ingredient.id} type="button" onClick={() => toggle(ingredient.id)} disabled={disabled} className={`flex items-center justify-between rounded-xl border px-3 py-3 text-left transition ${isSelected ? 'border-cyan-400/50 bg-cyan-400/10 text-white' : 'border-white/10 bg-white/5 text-gray-200 hover:bg-white/10'}`}>
                  <span>
                    <span className="block font-medium">{ingredient.name}</span>
                    <span className="block text-xs text-gray-400">{ingredient.unit || 'unit not set'} · {ingredient.caloriesPerUnit ?? 0} kcal</span>
                  </span>
                  {isSelected && <Check size={16} className="text-cyan-300" />}
                </button>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
};
