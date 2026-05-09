import React from 'react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';

export const IngredientsPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-16">
        <div className="mb-12">
          <h1 className="text-4xl sm:text-5xl font-bold text-white mb-4">
            Ingredients
          </h1>
          <p className="text-gray-400">Manage your ingredient library with nutritional information.</p>
        </div>
        <div className="bg-white/5 backdrop-blur border border-white/10 rounded-2xl p-6">
          <IngredientList />
        </div>
      </div>
    </div>
  );
};
