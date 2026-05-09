import React, { useEffect, useState } from 'react';
import { X } from 'lucide-react';
import type { Food, FoodCreateDto, FoodIngredientCreateDto } from '../types';
import type { Ingredient, SelectedIngredient } from '../types/ingredient';
import { FoodIngredientsPicker } from './ingredients/FoodIngredientsPicker';
import { ingredientService } from '../services/ingredientService';

interface FoodModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: FoodCreateDto) => Promise<void>;
  initialData?: Food | null;
  isLoading?: boolean;
}

export const FoodModal: React.FC<FoodModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
}) => {
  const [name, setName] = useState('');
  const [type, setType] = useState('');
  const [calories, setCalories] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [allIngredients, setAllIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredient[]>([]);

  useEffect(() => {
    if (isOpen) {
      ingredientService.getAll(1, 100).then(setAllIngredients).catch(console.error);
    }
  }, [isOpen]);

  useEffect(() => {
    if (initialData) {
      setName(initialData.name || '');
      setType(initialData.type || '');
      setCalories(initialData.calories?.toString() || '');
      // Map existing ingredients to SelectedIngredient
      if (initialData.ingredients && initialData.ingredients.length > 0) {
        setSelectedIngredients(
          initialData.ingredients.map((fi) => ({
            ingredientId: fi.ingredientId,
            quantity: fi.quantity,
          }))
        );
      } else {
        setSelectedIngredients([]);
      }
    } else {
      setName('');
      setType('');
      setCalories('');
      setSelectedIngredients([]);
    }
  }, [initialData, isOpen]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};
    if (!name.trim()) newErrors.name = 'Name is required';
    if (!type.trim()) newErrors.type = 'Type is required';
    const cal = parseFloat(calories);
    if (isNaN(cal) || cal < 0) newErrors.calories = 'Calories must be >= 0';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    const ingredients: FoodIngredientCreateDto[] = selectedIngredients.map((si) => ({
      ingredientId: si.ingredientId,
      quantity: si.quantity,
    }));

    const data: FoodCreateDto = {
      name: name.trim(),
      type: type.trim(),
      calories: parseFloat(calories) || 0,
      ingredients: ingredients.length > 0 ? ingredients : undefined,
    };

    try {
      await onSubmit(data);
    } catch (error) {
      console.error('Form submission error:', error);
    }
  };

  if (!isOpen) return null;

  const inputClass = (field: string) =>
    `w-full px-4 py-3 bg-white/10 border ${
      errors[field] ? 'border-red-500/70' : 'border-white/20'
    } rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 transition-all`;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-lg w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center sticky top-0 z-10">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Food' : 'Add New Food'}
          </h2>
          <button
            onClick={onClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-5">
          {/* Name */}
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Food Name</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className={inputClass('name')}
              placeholder="e.g., Grilled Chicken"
              disabled={isLoading}
            />
            {errors.name && <span className="text-red-400 text-xs mt-1 block">{errors.name}</span>}
          </div>

          {/* Type */}
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Food Type</label>
            <input
              type="text"
              value={type}
              onChange={(e) => setType(e.target.value)}
              className={inputClass('type')}
              placeholder="e.g., Protein, Vegetable"
              disabled={isLoading}
            />
            {errors.type && <span className="text-red-400 text-xs mt-1 block">{errors.type}</span>}
          </div>

          {/* Calories */}
          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">Calories</label>
            <input
              type="number"
              value={calories}
              onChange={(e) => setCalories(e.target.value)}
              className={inputClass('calories')}
              placeholder="e.g., 250"
              disabled={isLoading}
              min="0"
            />
            {errors.calories && <span className="text-red-400 text-xs mt-1 block">{errors.calories}</span>}
          </div>

          {/* Ingredients Picker */}
          <div className="border-t border-white/10 pt-4">
            <FoodIngredientsPicker
              availableIngredients={allIngredients}
              selected={selectedIngredients}
              onChange={setSelectedIngredients}
            />
          </div>

          {/* Buttons */}
          <div className="flex space-x-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
              disabled={isLoading}
            >
              {isLoading ? (
                <span className="flex items-center justify-center">
                  <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
                  Saving...
                </span>
              ) : (
                initialData ? 'Update Food' : 'Create Food'
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
