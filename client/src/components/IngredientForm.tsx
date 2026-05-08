import React, { useState, useEffect } from 'react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => void;
  onCancel: () => void;
  isLoading: boolean;
}

interface FormState {
  name: string;
  unit: string;
  caloriesPerUnit: string;
  protein: string;
  carbs: string;
  fat: string;
  foodEntityId: string;
}

interface FormErrors {
  name?: string;
  unit?: string;
  caloriesPerUnit?: string;
  protein?: string;
  carbs?: string;
  fat?: string;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  isLoading,
}) => {
  const [form, setForm] = useState<FormState>({
    name: '',
    unit: '',
    caloriesPerUnit: '0',
    protein: '0',
    carbs: '0',
    fat: '0',
    foodEntityId: '0',
  });
  const [errors, setErrors] = useState<FormErrors>({});

  useEffect(() => {
    if (initialData) {
      setForm({
        name: initialData.name,
        unit: initialData.unit,
        caloriesPerUnit: String(initialData.caloriesPerUnit),
        protein: String(initialData.protein),
        carbs: String(initialData.carbs),
        fat: String(initialData.fat),
        foodEntityId: String(initialData.foodEntityId),
      });
    } else {
      setForm({
        name: '',
        unit: '',
        caloriesPerUnit: '0',
        protein: '0',
        carbs: '0',
        fat: '0',
        foodEntityId: '0',
      });
    }
    setErrors({});
  }, [initialData]);

  const validate = (): boolean => {
    const newErrors: FormErrors = {};

    if (!form.name.trim()) {
      newErrors.name = 'Name is required';
    }

    if (!form.unit.trim()) {
      newErrors.unit = 'Unit is required';
    }

    const calories = Number(form.caloriesPerUnit);
    if (isNaN(calories) || calories < 0) {
      newErrors.caloriesPerUnit = 'Calories per unit must be 0 or greater';
    }

    const protein = Number(form.protein);
    if (isNaN(protein) || protein < 0) {
      newErrors.protein = 'Protein must be 0 or greater';
    }

    const carbs = Number(form.carbs);
    if (isNaN(carbs) || carbs < 0) {
      newErrors.carbs = 'Carbs must be 0 or greater';
    }

    const fat = Number(form.fat);
    if (isNaN(fat) || fat < 0) {
      newErrors.fat = 'Fat must be 0 or greater';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
    if (errors[name as keyof FormErrors]) {
      setErrors(prev => ({ ...prev, [name]: undefined }));
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    const data: IngredientCreateDto = {
      name: form.name.trim(),
      unit: form.unit.trim(),
      caloriesPerUnit: Number(form.caloriesPerUnit),
      protein: Number(form.protein),
      carbs: Number(form.carbs),
      fat: Number(form.fat),
    };

    onSubmit(data);
  };

  const inputClass =
    'w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-base transition-all disabled:opacity-50';
  const labelClass = 'block text-sm font-semibold text-gray-300 mb-2';
  const errorClass = 'text-red-400 text-xs mt-1 block';

  return (
    <form onSubmit={handleSubmit} className="p-6 space-y-5">
      <div>
        <label className={labelClass}>Name</label>
        <input
          name="name"
          type="text"
          value={form.name}
          onChange={handleChange}
          className={inputClass}
          placeholder="e.g., Chicken Breast"
          disabled={isLoading}
        />
        {errors.name && <span className={errorClass}>{errors.name}</span>}
      </div>

      <div>
        <label className={labelClass}>Unit</label>
        <input
          name="unit"
          type="text"
          value={form.unit}
          onChange={handleChange}
          className={inputClass}
          placeholder="e.g., grams, ml, piece"
          disabled={isLoading}
        />
        {errors.unit && <span className={errorClass}>{errors.unit}</span>}
      </div>

      <div>
        <label className={labelClass}>Calories per Unit</label>
        <input
          name="caloriesPerUnit"
          type="number"
          min={0}
          value={form.caloriesPerUnit}
          onChange={handleChange}
          className={inputClass}
          placeholder="e.g., 165"
          disabled={isLoading}
        />
        {errors.caloriesPerUnit && <span className={errorClass}>{errors.caloriesPerUnit}</span>}
      </div>

      <div className="grid grid-cols-3 gap-3">
        <div>
          <label className={labelClass}>Protein (g)</label>
          <input
            name="protein"
            type="number"
            min={0}
            step="0.1"
            value={form.protein}
            onChange={handleChange}
            className={inputClass}
            placeholder="0"
            disabled={isLoading}
          />
          {errors.protein && <span className={errorClass}>{errors.protein}</span>}
        </div>

        <div>
          <label className={labelClass}>Carbs (g)</label>
          <input
            name="carbs"
            type="number"
            min={0}
            step="0.1"
            value={form.carbs}
            onChange={handleChange}
            className={inputClass}
            placeholder="0"
            disabled={isLoading}
          />
          {errors.carbs && <span className={errorClass}>{errors.carbs}</span>}
        </div>

        <div>
          <label className={labelClass}>Fat (g)</label>
          <input
            name="fat"
            type="number"
            min={0}
            step="0.1"
            value={form.fat}
            onChange={handleChange}
            className={inputClass}
            placeholder="0"
            disabled={isLoading}
          />
          {errors.fat && <span className={errorClass}>{errors.fat}</span>}
        </div>
      </div>

      <div className="flex space-x-3 pt-2">
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
        >
          {isLoading ? (
            <span className="flex items-center justify-center">
              <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
              Saving...
            </span>
          ) : (
            initialData ? 'Update Ingredient' : 'Create Ingredient'
          )}
        </button>
      </div>
    </form>
  );
};
