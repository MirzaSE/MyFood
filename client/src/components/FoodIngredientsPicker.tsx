import React from 'react';
import type { Ingredient } from '../types/ingredient';

interface PickedIngredient { ingredientId: number; quantity: number; calories: number; }
interface Props { ingredients: Ingredient[]; value: PickedIngredient[]; onChange: (v: PickedIngredient[]) => void; }

export const FoodIngredientsPicker: React.FC<Props> = ({ ingredients, value, onChange }) => {
  const add = (ingredientId: number) => {
    const ingredient = ingredients.find(x => x.id === ingredientId);
    if (!ingredient || value.some(v => v.ingredientId === ingredientId)) return;
    onChange([...value, { ingredientId, quantity: 1, calories: ingredient.caloriesPerUnit }]);
  };

  const updateQty = (ingredientId: number, quantity: number) => {
    const ingredient = ingredients.find(x => x.id === ingredientId);
    if (!ingredient) return;
    onChange(value.map(v => v.ingredientId === ingredientId ? { ...v, quantity, calories: quantity * ingredient.caloriesPerUnit } : v));
  };

  const remove = (ingredientId: number) => onChange(value.filter(v => v.ingredientId !== ingredientId));
  const totalCalories = value.reduce((sum, v) => sum + v.calories, 0);

  return <div className="space-y-2"><select onChange={(e) => add(Number(e.target.value))} className="p-2 rounded bg-slate-800 text-white"><option value="">Add ingredient</option>{ingredients.map(i => <option key={i.id} value={i.id}>{i.name}</option>)}</select>{value.map(v => <div key={v.ingredientId} className="flex gap-2"><input type="number" value={v.quantity} onChange={(e) => updateQty(v.ingredientId, Number(e.target.value))} className="p-2 rounded bg-slate-800 text-white" /><button onClick={() => remove(v.ingredientId)} className="text-red-400">Remove</button></div>)}<p className="text-gray-300">Total calories: {totalCalories.toFixed(2)}</p></div>;
};
