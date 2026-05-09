import React, { useState, useEffect } from 'react';
import { AlertCircle } from 'lucide-react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface IngredientFormProps {
  ingredient?: Ingredient | null;
  onSubmit: (data: IngredientCreateDto) => Promise<void>;
  onCancel: () => void;
  isSubmitting?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  ingredient,
  onSubmit,
  onCancel,
  isSubmitting = false,
}) => {
  const [name, setName] = useState('');
  const [quantity, setQuantity] = useState('1');
  const [foodEntityId, setFoodEntityId] = useState('1');
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (ingredient) {
      setName(ingredient.name);
      setQuantity(ingredient.quantity.toString());
      setFoodEntityId(ingredient.foodEntityId.toString());
    }
  }, [ingredient]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!name.trim()) {
      newErrors.name = 'Ingredient name is required';
    }

    if (!quantity || parseFloat(quantity) <= 0) {
      newErrors.quantity = 'Quantity must be greater than 0';
    }

    if (!foodEntityId || parseFloat(foodEntityId) <= 0) {
      newErrors.foodEntityId = 'Food entity ID must be greater than 0';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      await onSubmit({
        name: name.trim(),
        quantity: parseFloat(quantity),
        foodEntityId: parseInt(foodEntityId),
      });
    } catch (err) {
      // Error handling is done in parent component
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {/* Name Field */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-2">
          Ingredient Name
        </label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="e.g., Chicken Breast"
          className={`w-full px-4 py-2 bg-gray-700 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 ${
            errors.name ? 'border-red-500 focus:ring-red-500' : 'border-gray-600 focus:ring-blue-500'
          }`}
        />
        {errors.name && (
          <div className="mt-1 flex items-center space-x-2 text-red-400 text-sm">
            <AlertCircle size={16} />
            <span>{errors.name}</span>
          </div>
        )}
      </div>

      {/* Quantity Field */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-2">
          Quantity
        </label>
        <input
          type="number"
          step="0.1"
          value={quantity}
          onChange={(e) => setQuantity(e.target.value)}
          placeholder="Enter quantity"
          className={`w-full px-4 py-2 bg-gray-700 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 ${
            errors.quantity ? 'border-red-500 focus:ring-red-500' : 'border-gray-600 focus:ring-blue-500'
          }`}
        />
        {errors.quantity && (
          <div className="mt-1 flex items-center space-x-2 text-red-400 text-sm">
            <AlertCircle size={16} />
            <span>{errors.quantity}</span>
          </div>
        )}
      </div>

      {/* Food Entity ID Field */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-2">
          Food Entity ID
        </label>
        <input
          type="number"
          value={foodEntityId}
          onChange={(e) => setFoodEntityId(e.target.value)}
          placeholder="Food Entity ID"
          className={`w-full px-4 py-2 bg-gray-700 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 ${
            errors.foodEntityId ? 'border-red-500 focus:ring-red-500' : 'border-gray-600 focus:ring-blue-500'
          }`}
        />
        {errors.foodEntityId && (
          <div className="mt-1 flex items-center space-x-2 text-red-400 text-sm">
            <AlertCircle size={16} />
            <span>{errors.foodEntityId}</span>
          </div>
        )}
      </div>

      {/* Action Buttons */}
      <div className="flex space-x-3 pt-4">
        <button
          type="submit"
          disabled={isSubmitting}
          className="flex-1 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-medium py-2 px-4 rounded-lg transition"
        >
          {isSubmitting ? 'Saving...' : 'Save Ingredient'}
        </button>
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmitting}
          className="flex-1 bg-gray-600 hover:bg-gray-700 disabled:bg-gray-500 text-white font-medium py-2 px-4 rounded-lg transition"
        >
          Cancel
        </button>
      </div>
    </form>
  );
};
