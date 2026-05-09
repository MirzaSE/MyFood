import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { ArrowLeft, AlertCircle, CheckCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker, type SelectedIngredient } from '../components/FoodIngredientsPicker';
import { foodService } from '../services/foodService';
import type { FoodCreateDto } from '../types';

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [formData, setFormData] = useState<FoodCreateDto>({
    name: '',
    type: '',
    calories: 0,
  });
  const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredient[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const calculateTotalCalories = (): number => {
    // This would calculate total from ingredients if we have calorie data
    // For now, just show the base calories
    return formData.calories;
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Food name is required';
    }

    if (!formData.type.trim()) {
      newErrors.type = 'Food type is required';
    }

    if (formData.calories < 0) {
      newErrors.calories = 'Calories cannot be negative';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: name === 'calories' ? parseFloat(value) || 0 : value,
    });
    // Clear error for this field
    if (errors[name]) {
      setErrors({ ...errors, [name]: '' });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      setIsSubmitting(true);
      setError(null);

      await foodService.createFood(formData);
      setSuccess('Food created successfully!');
      
      setTimeout(() => {
        // Return to referring page or food page
        if (location.state?.from) {
          navigate(location.state.from);
        } else {
          navigate('/foods');
        }
      }, 1500);
    } catch (err: any) {
      setError(err.message || 'Failed to create food');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {/* Back Button */}
        <button
          onClick={() => navigate(-1)}
          className="flex items-center space-x-2 text-blue-400 hover:text-blue-300 mb-8 transition"
        >
          <ArrowLeft size={20} />
          <span>Back</span>
        </button>

        {/* Error Banner */}
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        {/* Success Banner */}
        {success && (
          <div className="mb-6 p-4 bg-green-500/20 border border-green-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <CheckCircle size={20} className="text-green-400 flex-shrink-0 mt-0.5" />
            <p className="text-green-200">{success}</p>
          </div>
        )}

        {/* Page Header */}
        <div className="mb-12">
          <h1 className="text-4xl sm:text-5xl font-bold text-white mb-4">Create Food</h1>
          <p className="text-gray-400">Create a new food item with nutrition details and ingredients</p>
        </div>

        {/* Form Container */}
        <div className="max-w-2xl">
          <form onSubmit={handleSubmit} className="space-y-8">
            {/* Food Details Section */}
            <div className="bg-gray-800/50 backdrop-blur rounded-lg p-8 border border-gray-700">
              <h2 className="text-2xl font-bold text-white mb-6">Food Details</h2>

              {/* Name Field */}
              <div className="mb-6">
                <label className="block text-sm font-medium text-gray-300 mb-2">
                  Food Name *
                </label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  placeholder="e.g., Grilled Chicken Breast"
                  className={`w-full px-4 py-2 bg-gray-700 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 ${
                    errors.name ? 'border-red-500 focus:ring-red-500' : 'border-gray-600 focus:ring-blue-500'
                  }`}
                  disabled={isSubmitting}
                />
                {errors.name && (
                  <p className="mt-1 text-red-400 text-sm">{errors.name}</p>
                )}
              </div>

              {/* Type Field */}
              <div className="mb-6">
                <label className="block text-sm font-medium text-gray-300 mb-2">
                  Food Type *
                </label>
                <input
                  type="text"
                  name="type"
                  value={formData.type}
                  onChange={handleInputChange}
                  placeholder="e.g., Protein, Carbohydrate, Vegetable"
                  className={`w-full px-4 py-2 bg-gray-700 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 ${
                    errors.type ? 'border-red-500 focus:ring-red-500' : 'border-gray-600 focus:ring-blue-500'
                  }`}
                  disabled={isSubmitting}
                />
                {errors.type && (
                  <p className="mt-1 text-red-400 text-sm">{errors.type}</p>
                )}
              </div>

              {/* Calories Field */}
              <div className="mb-6">
                <label className="block text-sm font-medium text-gray-300 mb-2">
                  Calories (per 100g)
                </label>
                <input
                  type="number"
                  name="calories"
                  value={formData.calories}
                  onChange={handleInputChange}
                  placeholder="0"
                  min="0"
                  className={`w-full px-4 py-2 bg-gray-700 border rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 ${
                    errors.calories ? 'border-red-500 focus:ring-red-500' : 'border-gray-600 focus:ring-blue-500'
                  }`}
                  disabled={isSubmitting}
                />
                {errors.calories && (
                  <p className="mt-1 text-red-400 text-sm">{errors.calories}</p>
                )}
              </div>

              {/* Nutrition Summary */}
              <div className="p-4 bg-blue-500/10 border border-blue-500/30 rounded-lg">
                <p className="text-blue-200 text-sm">
                  <span className="font-semibold">Total Calories:</span> {calculateTotalCalories()} kcal
                </p>
                {selectedIngredients.length > 0 && (
                  <p className="text-blue-200 text-sm mt-2">
                    <span className="font-semibold">Ingredients:</span> {selectedIngredients.length} selected
                  </p>
                )}
              </div>
            </div>

            {/* Ingredients Section */}
            <div className="bg-gray-800/50 backdrop-blur rounded-lg p-8 border border-gray-700">
              <h2 className="text-2xl font-bold text-white mb-6">Ingredients</h2>
              <FoodIngredientsPicker
                selectedIngredients={selectedIngredients}
                onIngredientsChange={setSelectedIngredients}
              />
            </div>

            {/* Action Buttons */}
            <div className="flex space-x-3">
              <button
                type="submit"
                disabled={isSubmitting}
                className="flex-1 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-semibold py-3 px-6 rounded-lg transition"
              >
                {isSubmitting ? 'Creating...' : 'Create Food'}
              </button>
              <button
                type="button"
                onClick={() => navigate(-1)}
                disabled={isSubmitting}
                className="flex-1 bg-gray-700 hover:bg-gray-600 disabled:bg-gray-500 text-white font-semibold py-3 px-6 rounded-lg transition"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
