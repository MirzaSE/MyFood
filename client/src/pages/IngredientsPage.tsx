import React from 'react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';

export const IngredientsPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900/20 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-8">
        <div className="mb-6">
          <h2 className="text-3xl font-bold text-white">Ingredients</h2>
          <p className="text-gray-400 mt-1">Manage your ingredient library</p>
        </div>
        <div className="bg-white/5 border border-white/10 rounded-2xl p-6">
          <IngredientList />
        </div>
      </div>
    </div>
  );
};