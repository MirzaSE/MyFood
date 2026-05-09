import React, { useEffect, useState } from 'react';
import { Plus, X } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

export interface PickedIngredient {
  ingredient: Ingredient;
  quantity: string;
}

interface Props {
  value: PickedIngredient[];
  onChange: (picked: PickedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<Props> = ({ value, onChange }) => {
  const [available, setAvailable] = useState<Ingredient[]>([]);
  const [selectedId, setSelectedId] = useState<number | ''>('');
  const [qty, setQty] = useState('');

  useEffect(() => {
    ingredientService.getAllIngredients(1, 50).then(({ items }) => setAvailable(items));
  }, []);

  const add = () => {
    if (!selectedId || !qty.trim()) return;
    const ingredient = available.find((i) => i.id === Number(selectedId));
    if (!ingredient) return;
    if (value.some((p) => p.ingredient.id === ingredient.id)) return;
    onChange([...value, { ingredient, quantity: qty.trim() }]);
    setSelectedId('');
    setQty('');
  };

  const remove = (id: number) => onChange(value.filter((p) => p.ingredient.id !== id));

  return (
    <div className="space-y-4">
      <h3 className="text-sm font-semibold text-gray-300 uppercase tracking-wider">Ingredients</h3>

      {/* Picker row */}
      <div className="flex gap-2">
        <select
          value={selectedId}
          onChange={(e) => setSelectedId(e.target.value === '' ? '' : Number(e.target.value))}
          className="flex-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white focus:outline-none focus:ring-2 focus:ring-purple-500"
        >
          <option value="">Select ingredient…</option>
          {available.map((ing) => (
            <option key={ing.id} value={ing.id} className="bg-slate-800">
              {ing.name}
            </option>
          ))}
        </select>
        <input
          value={qty}
          onChange={(e) => setQty(e.target.value)}
          placeholder="Qty (e.g. 200g)"
          className="w-36 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500"
        />
        <button
          type="button"
          onClick={add}
          disabled={!selectedId || !qty.trim()}
          className="p-2 rounded-lg bg-purple-600 hover:bg-purple-700 text-white transition disabled:opacity-40 disabled:cursor-not-allowed"
        >
          <Plus size={18} />
        </button>
      </div>

      {/* Picked list */}
      {value.length > 0 && (
        <ul className="space-y-2">
          {value.map(({ ingredient, quantity }) => (
            <li
              key={ingredient.id}
              className="flex items-center justify-between bg-white/5 border border-white/10 rounded-lg px-4 py-2"
            >
              <span className="text-white font-medium">{ingredient.name}</span>
              <div className="flex items-center gap-3">
                <span className="text-gray-400 text-sm">{quantity}</span>
                <button
                  type="button"
                  onClick={() => remove(ingredient.id)}
                  className="text-red-400 hover:text-red-300 transition"
                >
                  <X size={16} />
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};
