import React from 'react';
import type { Ingredient } from '../../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: {
    name: string;
    unit: string;
    caloriesPerUnit: number;
    protein: number;
    carbs: number;
    fat: number;
  }) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isLoading = false,
}) => {
  const [name, setName] = React.useState(initialData?.name ?? '');
  const [unit, setUnit] = React.useState(initialData?.unit ?? '');
  const [caloriesPerUnit, setCaloriesPerUnit] = React.useState(initialData?.caloriesPerUnit?.toString() ?? '0');
  const [protein, setProtein] = React.useState(initialData?.protein?.toString() ?? '0');
  const [carbs, setCarbs] = React.useState(initialData?.carbs?.toString() ?? '0');
  const [fat, setFat] = React.useState(initialData?.fat?.toString() ?? '0');
  const [errors, setErrors] = React.useState<Record<string, string>>({});

  React.useEffect(() => {
    if (initialData) {
      setName(initialData.name);
      setUnit(initialData.unit);
      setCaloriesPerUnit(initialData.caloriesPerUnit.toString());
      setProtein(initialData.protein.toString());
      setCarbs(initialData.carbs.toString());
      setFat(initialData.fat.toString());
    }
  }, [initialData]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!name.trim()) newErrors.name = 'Name is required';
    if (!unit.trim()) newErrors.unit = 'Unit is required';

    const cal = parseFloat(caloriesPerUnit);
    const prot = parseFloat(protein);
    const carb = parseFloat(carbs);
    const fatVal = parseFloat(fat);

    if (isNaN(cal) || cal < 0) newErrors.caloriesPerUnit = 'Must be >= 0';
    if (isNaN(prot) || prot < 0) newErrors.protein = 'Must be >= 0';
    if (isNaN(carb) || carb < 0) newErrors.carbs = 'Must be >= 0';
    if (isNaN(fatVal) || fatVal < 0) newErrors.fat = 'Must be >= 0';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    onSubmit({
      name: name.trim(),
      unit: unit.trim(),
      caloriesPerUnit: parseFloat(caloriesPerUnit),
      protein: parseFloat(protein),
      carbs: parseFloat(carbs),
      fat: parseFloat(fat),
    });
  };

  const inputClass = (field: string) =>
    `w-full px-4 py-2.5 bg-white/5 border ${
      errors[field] ? 'border-red-500/70' : 'border-white/10 focus:border-purple-500/50'
    } rounded-lg text-white placeholder-gray-500 focus:outline-none focus:ring-2 ${
      errors[field] ? 'focus:ring-red-500/30' : 'focus:ring-purple-500/30'
    } transition-all`;

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {/* Name */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1.5">Name *</label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="e.g. Salt"
          className={inputClass('name')}
          disabled={isLoading}
        />
        {errors.name && <p className="mt-1 text-sm text-red-400">{errors.name}</p>}
      </div>

      {/* Unit */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1.5">Unit *</label>
        <input
          type="text"
          value={unit}
          onChange={(e) => setUnit(e.target.value)}
          placeholder="e.g. tsp, g, cup"
          className={inputClass('unit')}
          disabled={isLoading}
        />
        {errors.unit && <p className="mt-1 text-sm text-red-400">{errors.unit}</p>}
      </div>

      {/* Numeric fields grid */}
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1.5">Calories/Unit</label>
          <input
            type="number"
            step="0.1"
            min="0"
            value={caloriesPerUnit}
            onChange={(e) => setCaloriesPerUnit(e.target.value)}
            className={inputClass('caloriesPerUnit')}
            disabled={isLoading}
          />
          {errors.caloriesPerUnit && <p className="mt-1 text-sm text-red-400">{errors.caloriesPerUnit}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1.5">Protein</label>
          <input
            type="number"
            step="0.1"
            min="0"
            value={protein}
            onChange={(e) => setProtein(e.target.value)}
            className={inputClass('protein')}
            disabled={isLoading}
          />
          {errors.protein && <p className="mt-1 text-sm text-red-400">{errors.protein}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1.5">Carbs</label>
          <input
            type="number"
            step="0.1"
            min="0"
            value={carbs}
            onChange={(e) => setCarbs(e.target.value)}
            className={inputClass('carbs')}
            disabled={isLoading}
          />
          {errors.carbs && <p className="mt-1 text-sm text-red-400">{errors.carbs}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1.5">Fat</label>
          <input
            type="number"
            step="0.1"
            min="0"
            value={fat}
            onChange={(e) => setFat(e.target.value)}
            className={inputClass('fat')}
            disabled={isLoading}
          />
          {errors.fat && <p className="mt-1 text-sm text-red-400">{errors.fat}</p>}
        </div>
      </div>

      {/* Buttons */}
      <div className="flex gap-3 pt-2">
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white py-2.5 rounded-lg font-semibold transition-all disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isLoading ? 'Saving...' : initialData ? 'Update' : 'Create'}
        </button>
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="flex-1 bg-white/5 hover:bg-white/10 text-gray-300 py-2.5 rounded-lg font-medium transition-all border border-white/10 disabled:opacity-50"
        >
          Cancel
        </button>
      </div>
    </form>
  );
};
