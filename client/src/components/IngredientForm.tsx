import React, { useState, useEffect } from 'react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface Props {
  onSubmit: (dto: IngredientCreateDto) => void;
  onCancel: () => void;
  existing?: Ingredient;
}

export const IngredientForm: React.FC<Props> = ({ onSubmit, onCancel, existing }) => {
  const [name, setName] = useState('');
  const [unit, setUnit] = useState('');
  const [caloriesPerUnit, setCaloriesPerUnit] = useState(0);
  const [protein, setProtein] = useState(0);
  const [carbs, setCarbs] = useState(0);
  const [fat, setFat] = useState(0);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (existing) {
      setName(existing.name);
      setUnit(existing.unit);
      setCaloriesPerUnit(existing.caloriesPerUnit);
      setProtein(existing.protein);
      setCarbs(existing.carbs);
      setFat(existing.fat);
    }
  }, [existing]);

  const validate = () => {
    const newErrors: Record<string, string> = {};
    if (!name.trim()) newErrors.name = 'Name is required';
    if (!unit.trim()) newErrors.unit = 'Unit is required';
    if (caloriesPerUnit < 0) newErrors.caloriesPerUnit = 'Must be >= 0';
    if (protein < 0) newErrors.protein = 'Must be >= 0';
    if (carbs < 0) newErrors.carbs = 'Must be >= 0';
    if (fat < 0) newErrors.fat = 'Must be >= 0';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    onSubmit({ name, unit, caloriesPerUnit, protein, carbs, fat });
  };

  const inputClass = "w-full bg-white/5 border border-white/20 rounded-lg px-3 py-2 text-white placeholder-gray-400 focus:outline-none focus:border-purple-500";

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label className="text-sm text-gray-300">Name *</label>
        <input className={inputClass} value={name} onChange={e => setName(e.target.value)} placeholder="e.g. Chicken" />
        {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name}</p>}
      </div>
      <div>
        <label className="text-sm text-gray-300">Unit *</label>
        <input className={inputClass} value={unit} onChange={e => setUnit(e.target.value)} placeholder="e.g. g, ml, piece" />
        {errors.unit && <p className="text-red-400 text-xs mt-1">{errors.unit}</p>}
      </div>
      <div className="grid grid-cols-2 gap-3">
        <div>
          <label className="text-sm text-gray-300">Calories/Unit</label>
          <input type="number" className={inputClass} value={caloriesPerUnit} onChange={e => setCaloriesPerUnit(Number(e.target.value))} />
          {errors.caloriesPerUnit && <p className="text-red-400 text-xs mt-1">{errors.caloriesPerUnit}</p>}
        </div>
        <div>
          <label className="text-sm text-gray-300">Protein (g)</label>
          <input type="number" className={inputClass} value={protein} onChange={e => setProtein(Number(e.target.value))} />
          {errors.protein && <p className="text-red-400 text-xs mt-1">{errors.protein}</p>}
        </div>
        <div>
          <label className="text-sm text-gray-300">Carbs (g)</label>
          <input type="number" className={inputClass} value={carbs} onChange={e => setCarbs(Number(e.target.value))} />
          {errors.carbs && <p className="text-red-400 text-xs mt-1">{errors.carbs}</p>}
        </div>
        <div>
          <label className="text-sm text-gray-300">Fat (g)</label>
          <input type="number" className={inputClass} value={fat} onChange={e => setFat(Number(e.target.value))} />
          {errors.fat && <p className="text-red-400 text-xs mt-1">{errors.fat}</p>}
        </div>
      </div>
      <div className="flex gap-3 pt-2">
        <button type="submit" className="flex-1 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg font-medium transition-colors">
          {existing ? 'Update' : 'Create'}
        </button>
        <button type="button" onClick={onCancel} className="flex-1 py-2 bg-white/10 hover:bg-white/20 text-white rounded-lg font-medium transition-colors">
          Cancel
        </button>
      </div>
    </form>
  );
};