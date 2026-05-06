import React, { useEffect, useMemo, useState } from 'react';
import { Navbar } from '../components/Navbar';
import { FoodIngredientsPicker } from '../components/FoodIngredientsPicker';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, FoodIngredientSelection } from '../types/ingredient';

export const CreateFoodPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredients, setSelectedIngredients] = useState<FoodIngredientSelection[]>([]);
  const [name, setName] = useState('');
  const [type, setType] = useState('');

  useEffect(() => {
    ingredientService.getAllIngredients(1, 50, '').then(setIngredients).catch(() => setIngredients([]));
  }, []);

  const nutrition = useMemo(() => {
    return selectedIngredients.reduce((acc, item) => {
      const ingredient = ingredients.find((x) => x.id === item.ingredientId);
      if (!ingredient) return acc;
      return {
        calories: acc.calories + ingredient.caloriesPerUnit * item.quantity,
        protein: acc.protein + ingredient.protein * item.quantity,
        carbs: acc.carbs + ingredient.carbs * item.quantity,
        fat: acc.fat + ingredient.fat * item.quantity,
      };
    }, { calories: 0, protein: 0, carbs: 0, fat: 0 });
  }, [ingredients, selectedIngredients]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-10 space-y-6">
        <h1 className="text-3xl text-white font-bold">Create Food</h1>
        <div className="grid gap-4">
          <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Food name" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
          <input value={type} onChange={(e) => setType(e.target.value)} placeholder="Food type" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
        </div>

        <FoodIngredientsPicker ingredients={ingredients} selected={selectedIngredients} onChange={setSelectedIngredients} />

        <div className="rounded-lg border border-slate-700 p-4 bg-slate-900/60 text-gray-200">
          <p className="font-semibold text-white mb-2">Calculated Nutrition</p>
          <p>Calories: {nutrition.calories.toFixed(2)}</p>
          <p>Protein: {nutrition.protein.toFixed(2)}</p>
          <p>Carbs: {nutrition.carbs.toFixed(2)}</p>
          <p>Fat: {nutrition.fat.toFixed(2)}</p>
        </div>
      </div>
    </div>
  );
};
