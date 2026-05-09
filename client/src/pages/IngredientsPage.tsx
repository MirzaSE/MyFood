import React from 'react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';

export const IngredientsPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <div className="max-w-6xl mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-800">Ingredients</h1>
          <p className="text-gray-500 mt-1">Manage your ingredient library with nutritional information.</p>
        </div>
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
          <IngredientList />
        </div>
      </div>
    </div>
  );
};
