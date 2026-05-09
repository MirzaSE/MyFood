import React, { useState, useEffect } from 'react';
import { Plus, X } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

interface SelectedIngredient {
  ingredient: Ingredient;
  quantity: number;
}

interface Props {
  onChange: (selected: SelectedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<Props> = ({ onChange }) => {
  const [available, setAvailable] = useState<Ingredient[]>([]);
  const [selected, setSelected] = useState<SelectedIngredient[]>([]);
  const [selectedId, setSelectedId] = useState('');

  useEffect(() => {
    ingredientService.getAll().then(setAvailable).catch(() => setAvailable([]));
  }, []);

  useEffect(() => {
    onChange(selected);
  }, [selected]);

  const totalCalories = selected.reduce(
    (sum, s) => sum + s.ingredient.caloriesPerUnit * s.quantity, 0
  );

  const handleAdd = () => {
    const id = parseInt(selectedId);
    if (!id) return;
    if (selected.find(s => s.ingredient.id === id)) return;
    const ingredient = available.find(a => a.id === id);
    if (!ingredient) return;
    setSelected([...selected, { ingredient, quantity: 1 }]);
    setSelectedId('');
  };

  const handleRemove = (id: number) => {
    setSelected(selected.filter(s => s.ingredient.id !== id));
  };

  const handleQuantityChange = (id: number, qty: number) => {
    setSelected(selected.map(s =>
      s.ingredient.id === id ? { ...s, quantity: qty } : s
    ));
  };

  return (
    <div className="space-y-3">
      <label className="block text-sm font-medium text-gray-300">Ingredients</label>

      {/* Picker row */}
      <div className="flex space-x-2">
        <select
          value={selectedId}
          onChange={e => setSelectedId(e.target.value)}
          className="flex-1 bg-white/10 border border-white/20 rounded-lg px-3 py-2 text-white text-sm"
        >
          <option value="">Select ingredient...</option>
          {available.map(a => (
            <option key={a.id} value={a.id} className="bg-slate-800">
              {a.name} ({a.unit})
            </option>
          ))}
        </select>
        <button
          type="button"
          onClick={handleAdd}
          className="px-3 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition"
        >
          <Plus size={16} />
        </button>
      </div>

      {/* Selected list */}
      {selected.length > 0 && (
        <div className="space-y-2">
          {selected.map(s => (
            <div key={s.ingredient.id} className="flex items-center space-x-3 bg-white/5 rounded-lg px-3 py-2">
              <span className="flex-1 text-white text-sm">{s.ingredient.name}</span>
              <span className="text-gray-400 text-sm">{s.ingredient.unit}</span>
              <input
                type="number"
                min="0.1"
                step="0.1"
                value={s.quantity}
                onChange={e => handleQuantityChange(s.ingredient.id, parseFloat(e.target.value) || 0)}
                className="w-20 bg-white/10 border border-white/20 rounded px-2 py-1 text-white text-sm text-center"
              />
              <button
                type="button"
                onClick={() => handleRemove(s.ingredient.id)}
                className="text-red-400 hover:text-red-300"
              >
                <X size={16} />
              </button>
            </div>
          ))}

          <div className="text-right text-sm text-purple-300 font-medium">
            Total: {totalCalories.toFixed(0)} kcal
          </div>
        </div>
      )}
    </div>
  );
};