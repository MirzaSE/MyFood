import React, { useState, useEffect } from 'react';
import type { Ingredient } from '../types/ingredient';
import { ingredientService } from '..//services/ingredientService';
import { Plus, Trash2 } from 'lucide-react';

interface SelectedIngredient {
  ingredient: Ingredient;
  quantity: number;
}

interface Props {
  onChange?: (selected: SelectedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<Props> = ({ onChange }) => {
  const [available, setAvailable] = useState<Ingredient[]>([]);
  const [selected, setSelected] = useState<SelectedIngredient[]>([]);
  const [selectedId, setSelectedId] = useState<number | ''>('');

  useEffect(() => {
    ingredientService.getAll(1, 100).then(setAvailable).catch(() => {});
  }, []);

  useEffect(() => {
    onChange?.(selected);
  }, [selected]);

  const totalCalories = selected.reduce(
    (sum, s) => sum + s.ingredient.caloriesPerUnit * s.quantity, 0
  );

  const handleAdd = () => {
    if (!selectedId) return;
    const ingredient = available.find(i => i.id === Number(selectedId));
    if (!ingredient) return;
    if (selected.find(s => s.ingredient.id === ingredient.id)) return;
    setSelected(prev => [...prev, { ingredient, quantity: 1 }]);
    setSelectedId('');
  };

  const handleRemove = (id: number) => {
    setSelected(prev => prev.filter(s => s.ingredient.id !== id));
  };

  const handleQtyChange = (id: number, qty: number) => {
    setSelected(prev => prev.map(s => s.ingredient.id === id ? { ...s, quantity: qty } : s));
  };

  return (
    <div className="space-y-3">
      <h3 className="text-sm font-semibold text-gray-300">Ingredients</h3>

      {/* Picker row */}
      <div className="flex gap-2">
        <select
          value={selectedId}
          onChange={e => setSelectedId(e.target.value === '' ? '' : Number(e.target.value))}
          className="flex-1 bg-white/5 border border-white/20 rounded-lg px-3 py-2 text-white focus:outline-none focus:border-purple-500 text-sm"
        >
          <option value="">Select ingredient...</option>
          {available.map(i => (
            <option key={i.id} value={i.id} className="bg-slate-800">{i.name} ({i.unit})</option>
          ))}
        </select>
        <button
          type="button"
          onClick={handleAdd}
          className="px-3 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-colors"
        >
          <Plus size={16} />
        </button>
      </div>

      {/* Selected list */}
      {selected.length > 0 && (
        <div className="space-y-2">
          {selected.map(s => (
            <div key={s.ingredient.id} className="flex items-center gap-3 bg-white/5 rounded-lg px-3 py-2">
              <span className="flex-1 text-white text-sm">{s.ingredient.name}</span>
              <span className="text-gray-400 text-xs">{s.ingredient.unit}</span>
              <input
                type="number"
                min={1}
                value={s.quantity}
                onChange={e => handleQtyChange(s.ingredient.id, Number(e.target.value))}
                className="w-16 bg-white/10 border border-white/20 rounded px-2 py-1 text-white text-sm focus:outline-none"
              />
              <button
                type="button"
                onClick={() => handleRemove(s.ingredient.id)}
                className="text-red-400 hover:text-red-300 transition-colors"
              >
                <Trash2 size={14} />
              </button>
            </div>
          ))}

          {/* Total calories */}
          <div className="flex justify-between items-center px-3 py-2 bg-purple-500/10 border border-purple-500/20 rounded-lg">
            <span className="text-sm text-gray-300">Total Calories</span>
            <span className="text-white font-semibold">{totalCalories.toFixed(1)} kcal</span>
          </div>
        </div>
      )}
    </div>
  );
};