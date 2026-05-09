import React from 'react';
import { Plus, X, Flame } from 'lucide-react';
import type { Ingredient, SelectedIngredient } from '../../types/ingredient';

interface FoodIngredientsPickerProps {
  availableIngredients: Ingredient[];
  selected: SelectedIngredient[];
  onChange: (selected: SelectedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  availableIngredients,
  selected,
  onChange,
}) => {
  const [addingId, setAddingId] = React.useState<number | null>(null);
  const [newQuantity, setNewQuantity] = React.useState('1');

  const alreadySelected = new Set(selected.map((s) => s.ingredientId));

  const handleAdd = () => {
    if (!addingId) return;
    const qty = parseFloat(newQuantity);
    if (isNaN(qty) || qty <= 0) return;

    onChange([...selected, { ingredientId: addingId, quantity: qty }]);
    setAddingId(null);
    setNewQuantity('1');
  };

  const handleRemove = (ingredientId: number) => {
    onChange(selected.filter((s) => s.ingredientId !== ingredientId));
  };

  const handleQuantityChange = (ingredientId: number, quantity: number) => {
    onChange(
      selected.map((s) =>
        s.ingredientId === ingredientId ? { ...s, quantity } : s
      )
    );
  };

  const getIngredient = (id: number) => availableIngredients.find((i) => i.id === id);

  const totalCalories = selected.reduce((sum, s) => {
    const ing = getIngredient(s.ingredientId);
    return sum + (ing ? ing.caloriesPerUnit * s.quantity : 0);
  }, 0);
  const totalProtein = selected.reduce((sum, s) => {
    const ing = getIngredient(s.ingredientId);
    return sum + (ing ? ing.protein * s.quantity : 0);
  }, 0);
  const totalCarbs = selected.reduce((sum, s) => {
    const ing = getIngredient(s.ingredientId);
    return sum + (ing ? ing.carbs * s.quantity : 0);
  }, 0);
  const totalFat = selected.reduce((sum, s) => {
    const ing = getIngredient(s.ingredientId);
    return sum + (ing ? ing.fat * s.quantity : 0);
  }, 0);

  const unselectedIngredients = availableIngredients.filter(
    (i) => !alreadySelected.has(i.id)
  );

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold text-white">Ingredients</h3>

      {/* Add ingredient */}
      {unselectedIngredients.length > 0 && (
        <div className="flex gap-2 items-end">
          <div className="flex-1">
            <label className="block text-sm text-gray-400 mb-1">Add ingredient</label>
            <select
              value={addingId ?? ''}
              onChange={(e) => setAddingId(e.target.value ? Number(e.target.value) : null)}
              className="w-full px-3 py-2 bg-white/5 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500/50"
            >
              <option value="">Select...</option>
              {unselectedIngredients.map((i) => (
                <option key={i.id} value={i.id}>
                  {i.name} ({i.unit})
                </option>
              ))}
            </select>
          </div>
          <div className="w-24">
            <label className="block text-sm text-gray-400 mb-1">Qty</label>
            <input
              type="number"
              min="0.1"
              step="0.1"
              value={newQuantity}
              onChange={(e) => setNewQuantity(e.target.value)}
              className="w-full px-3 py-2 bg-white/5 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500/50"
            />
          </div>
          <button
            onClick={handleAdd}
            disabled={!addingId}
            className="px-4 py-2 bg-purple-600/30 hover:bg-purple-600/50 text-purple-300 rounded-lg transition-all border border-purple-500/30 disabled:opacity-30"
          >
            <Plus size={18} />
          </button>
        </div>
      )}

      {/* Selected ingredients list */}
      {selected.length > 0 ? (
        <div className="space-y-2">
          {selected.map((sel) => {
            const ing = getIngredient(sel.ingredientId);
            if (!ing) return null;
            return (
              <div
                key={sel.ingredientId}
                className="flex items-center gap-3 bg-white/5 border border-white/10 rounded-lg px-4 py-2"
              >
                <span className="flex-1 text-white font-medium">{ing.name}</span>
                <span className="text-xs text-gray-400">({ing.unit})</span>
                <input
                  type="number"
                  min="0.1"
                  step="0.1"
                  value={sel.quantity}
                  onChange={(e) =>
                    handleQuantityChange(sel.ingredientId, parseFloat(e.target.value) || 0)
                  }
                  className="w-20 px-2 py-1 bg-white/5 border border-white/10 rounded text-white text-sm text-center focus:outline-none focus:border-purple-500/50"
                />
                <span className="text-sm text-gray-400">
                  {(ing.caloriesPerUnit * sel.quantity).toFixed(0)} kcal
                </span>
                <button
                  onClick={() => handleRemove(sel.ingredientId)}
                  className="p-1 hover:bg-red-500/20 text-red-400 rounded transition-colors"
                >
                  <X size={16} />
                </button>
              </div>
            );
          })}

          {/* Total nutrition */}
          <div className="flex items-center gap-2 pt-2 mt-2 border-t border-white/10">
            <Flame size={18} className="text-orange-400" />
            <span className="text-sm text-gray-300">
              Total: <strong className="text-white">{totalCalories.toFixed(0)} kcal</strong> |{' '}
              P: {totalProtein.toFixed(1)}g | C: {totalCarbs.toFixed(1)}g | F: {totalFat.toFixed(1)}g
            </span>
          </div>
        </div>
      ) : (
        <p className="text-gray-500 text-sm">No ingredients added yet.</p>
      )}
    </div>
  );
};
