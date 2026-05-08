import { useEffect, useState } from 'react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  initialData?: Ingredient | null;
  isLoading?: boolean;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
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
  isLoading = false,
  onSubmit,
  onCancel,
}) => {
  const [formData, setFormData] = useState<IngredientCreateDto>({
    name: '',
    unit: '',
    caloriesPerUnit: 0,
    protein: 0,
    carbs: 0,
    fat: 0,
  });

  const [errors, setErrors] = useState<FormErrors>({});

  useEffect(() => {
    if (initialData) {
      setFormData({
        name: initialData.name,
        unit: initialData.unit,
        caloriesPerUnit: initialData.caloriesPerUnit,
        protein: initialData.protein,
        carbs: initialData.carbs,
        fat: initialData.fat,
        quantity: initialData.quantity,
        foodEntityId: initialData.foodEntityId,
      });
    } else {
      setFormData({
        name: '',
        unit: '',
        caloriesPerUnit: 0,
        protein: 0,
        carbs: 0,
        fat: 0,
      });
    }

    setErrors({});
  }, [initialData]);

  const validate = (): boolean => {
    const nextErrors: FormErrors = {};

    if (!formData.name.trim()) {
      nextErrors.name = 'Name is required.';
    }

    if (!formData.unit.trim()) {
      nextErrors.unit = 'Unit is required.';
    }

    if (formData.caloriesPerUnit <= 0) {
      nextErrors.caloriesPerUnit = 'Calories per unit must be greater than 0.';
    }

    if (formData.protein < 0) {
      nextErrors.protein = 'Protein cannot be negative.';
    }

    if (formData.carbs < 0) {
      nextErrors.carbs = 'Carbs cannot be negative.';
    }

    if (formData.fat < 0) {
      nextErrors.fat = 'Fat cannot be negative.';
    }

    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  };

  const handleTextChange = (
    field: 'name' | 'unit',
    value: string
  ) => {
    setFormData((current) => ({
      ...current,
      [field]: value,
    }));
  };

  const handleNumberChange = (
    field: 'caloriesPerUnit' | 'protein' | 'carbs' | 'fat',
    value: string
  ) => {
    setFormData((current) => ({
      ...current,
      [field]: Number(value),
    }));
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!validate()) {
      return;
    }

    await onSubmit({
      ...formData,
      name: formData.name.trim(),
      unit: formData.unit.trim(),
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700">
          Ingredient name
        </label>
        <input
          value={formData.name}
          onChange={(event) => handleTextChange('name', event.target.value)}
          className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
          placeholder="Example: Chicken breast"
        />
        {errors.name && <p className="mt-1 text-sm text-red-600">{errors.name}</p>}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700">
          Unit
        </label>
        <input
          value={formData.unit}
          onChange={(event) => handleTextChange('unit', event.target.value)}
          className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
          placeholder="g, ml, piece"
        />
        {errors.unit && <p className="mt-1 text-sm text-red-600">{errors.unit}</p>}
      </div>

      <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
        <div>
          <label className="block text-sm font-medium text-gray-700">
            Calories per unit
          </label>
          <input
            type="number"
            step="0.01"
            value={formData.caloriesPerUnit}
            onChange={(event) =>
              handleNumberChange('caloriesPerUnit', event.target.value)
            }
            className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
          />
          {errors.caloriesPerUnit && (
            <p className="mt-1 text-sm text-red-600">{errors.caloriesPerUnit}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">
            Protein
          </label>
          <input
            type="number"
            step="0.01"
            value={formData.protein}
            onChange={(event) => handleNumberChange('protein', event.target.value)}
            className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
          />
          {errors.protein && (
            <p className="mt-1 text-sm text-red-600">{errors.protein}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">
            Carbs
          </label>
          <input
            type="number"
            step="0.01"
            value={formData.carbs}
            onChange={(event) => handleNumberChange('carbs', event.target.value)}
            className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
          />
          {errors.carbs && (
            <p className="mt-1 text-sm text-red-600">{errors.carbs}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">
            Fat
          </label>
          <input
            type="number"
            step="0.01"
            value={formData.fat}
            onChange={(event) => handleNumberChange('fat', event.target.value)}
            className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2"
          />
          {errors.fat && (
            <p className="mt-1 text-sm text-red-600">{errors.fat}</p>
          )}
        </div>
      </div>

      <div className="flex justify-end gap-3 pt-4">
        <button
          type="button"
          onClick={onCancel}
          className="rounded-lg border border-gray-300 px-4 py-2 text-gray-700"
          disabled={isLoading}
        >
          Cancel
        </button>
        <button
          type="submit"
          className="rounded-lg bg-blue-600 px-4 py-2 text-white disabled:opacity-60"
          disabled={isLoading}
        >
          {isLoading ? 'Saving...' : initialData ? 'Update Ingredient' : 'Create Ingredient'}
        </button>
      </div>
    </form>
  );
};