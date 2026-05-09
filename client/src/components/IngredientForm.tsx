import { useState } from 'react';
import type { IngredientCreateDto, IngredientUpdateDto, Ingredient } from '../types/ingredient';

interface IngredientFormProps {
  ingredient?: Ingredient;
  onSubmit: (data: IngredientCreateDto | IngredientUpdateDto) => Promise<void>;
  onCancel: () => void;
  isLoading: boolean;
}

interface FormErrors {
  name?: string;
  unit?: string;
  caloriesPerUnit?: string;
  protein?: string;
  carbs?: string;
  fat?: string;
}

export function IngredientForm({ ingredient, onSubmit, onCancel, isLoading }: IngredientFormProps) {
  const [formData, setFormData] = useState({
    name: ingredient?.name || '',
    unit: ingredient?.unit || '',
    caloriesPerUnit: ingredient?.caloriesPerUnit || 0,
    protein: ingredient?.protein || 0,
    carbs: ingredient?.carbs || 0,
    fat: ingredient?.fat || 0,
  });

  const [errors, setErrors] = useState<FormErrors>({});

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Name is required';
    }

    if (!formData.unit.trim()) {
      newErrors.unit = 'Unit is required';
    }

    if (formData.caloriesPerUnit <= 0) {
      newErrors.caloriesPerUnit = 'Calories per unit must be greater than 0';
    }

    if (formData.protein <= 0) {
      newErrors.protein = 'Protein must be greater than 0';
    }

    if (formData.carbs <= 0) {
      newErrors.carbs = 'Carbs must be greater than 0';
    }

    if (formData.fat <= 0) {
      newErrors.fat = 'Fat must be greater than 0';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name.startsWith('calories') || name === 'protein' || name === 'carbs' || name === 'fat'
        ? parseFloat(value) || 0
        : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      await onSubmit(formData);
    } catch (error) {
      console.error('Error submitting form:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700">Name</label>
        <input
          type="text"
          name="name"
          value={formData.name}
          onChange={handleChange}
          className={`mt-1 block w-full rounded-md border px-3 py-2 ${
            errors.name ? 'border-red-500' : 'border-gray-300'
          }`}
          disabled={isLoading}
        />
        {errors.name && <p className="text-red-500 text-sm mt-1">{errors.name}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700">Unit</label>
        <input
          type="text"
          name="unit"
          value={formData.unit}
          onChange={handleChange}
          placeholder="e.g., g, ml, oz"
          className={`mt-1 block w-full rounded-md border px-3 py-2 ${
            errors.unit ? 'border-red-500' : 'border-gray-300'
          }`}
          disabled={isLoading}
        />
        {errors.unit && <p className="text-red-500 text-sm mt-1">{errors.unit}</p>}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700">Calories per Unit</label>
          <input
            type="number"
            name="caloriesPerUnit"
            value={formData.caloriesPerUnit}
            onChange={handleChange}
            step="0.01"
            min="0"
            className={`mt-1 block w-full rounded-md border px-3 py-2 ${
              errors.caloriesPerUnit ? 'border-red-500' : 'border-gray-300'
            }`}
            disabled={isLoading}
          />
          {errors.caloriesPerUnit && <p className="text-red-500 text-sm mt-1">{errors.caloriesPerUnit}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Protein (g)</label>
          <input
            type="number"
            name="protein"
            value={formData.protein}
            onChange={handleChange}
            step="0.01"
            min="0"
            className={`mt-1 block w-full rounded-md border px-3 py-2 ${
              errors.protein ? 'border-red-500' : 'border-gray-300'
            }`}
            disabled={isLoading}
          />
          {errors.protein && <p className="text-red-500 text-sm mt-1">{errors.protein}</p>}
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700">Carbs (g)</label>
          <input
            type="number"
            name="carbs"
            value={formData.carbs}
            onChange={handleChange}
            step="0.01"
            min="0"
            className={`mt-1 block w-full rounded-md border px-3 py-2 ${
              errors.carbs ? 'border-red-500' : 'border-gray-300'
            }`}
            disabled={isLoading}
          />
          {errors.carbs && <p className="text-red-500 text-sm mt-1">{errors.carbs}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Fat (g)</label>
          <input
            type="number"
            name="fat"
            value={formData.fat}
            onChange={handleChange}
            step="0.01"
            min="0"
            className={`mt-1 block w-full rounded-md border px-3 py-2 ${
              errors.fat ? 'border-red-500' : 'border-gray-300'
            }`}
            disabled={isLoading}
          />
          {errors.fat && <p className="text-red-500 text-sm mt-1">{errors.fat}</p>}
        </div>
      </div>

      <div className="flex gap-2 pt-4">
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 disabled:opacity-50"
        >
          {isLoading ? 'Saving...' : 'Save'}
        </button>
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="flex-1 bg-gray-300 text-gray-800 px-4 py-2 rounded-md hover:bg-gray-400 disabled:opacity-50"
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
