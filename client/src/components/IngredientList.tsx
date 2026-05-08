import React from 'react';
import { Edit, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => void;
  isLoading: boolean;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  onEdit,
  onDelete,
  isLoading,
}) => {
  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center py-20">
        <div className="relative w-16 h-16 mb-4">
          <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>
          <div className="absolute inset-2 bg-slate-900 rounded-full"></div>
        </div>
        <p className="text-gray-300 font-medium">Loading ingredients...</p>
      </div>
    );
  }

  if (ingredients.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-20 text-center">
        <div className="w-16 h-16 mb-4 bg-white/5 rounded-full flex items-center justify-center">
          <span className="text-3xl">🥗</span>
        </div>
        <p className="text-gray-300 font-medium text-lg mb-2">No ingredients found</p>
        <p className="text-gray-500 text-sm">Click "New Ingredient" to add your first ingredient.</p>
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-xl border border-white/10">
      <table className="w-full text-sm text-left">
        <thead>
          <tr className="bg-white/5 border-b border-white/10">
            <th className="px-4 py-3 text-gray-400 font-semibold uppercase tracking-wider">Name</th>
            <th className="px-4 py-3 text-gray-400 font-semibold uppercase tracking-wider">Unit</th>
            <th className="px-4 py-3 text-gray-400 font-semibold uppercase tracking-wider text-right">Cal/Unit</th>
            <th className="px-4 py-3 text-gray-400 font-semibold uppercase tracking-wider text-right">Protein</th>
            <th className="px-4 py-3 text-gray-400 font-semibold uppercase tracking-wider text-right">Carbs</th>
            <th className="px-4 py-3 text-gray-400 font-semibold uppercase tracking-wider text-right">Fat</th>
            <th className="px-4 py-3 text-gray-400 font-semibold uppercase tracking-wider text-center">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-white/5">
          {ingredients.map(ingredient => (
            <tr
              key={ingredient.id}
              className="hover:bg-white/5 transition-colors duration-150"
            >
              <td className="px-4 py-3 text-white font-medium">{ingredient.name}</td>
              <td className="px-4 py-3 text-gray-300">{ingredient.unit}</td>
              <td className="px-4 py-3 text-gray-300 text-right">{ingredient.caloriesPerUnit}</td>
              <td className="px-4 py-3 text-gray-300 text-right">{ingredient.protein}g</td>
              <td className="px-4 py-3 text-gray-300 text-right">{ingredient.carbs}g</td>
              <td className="px-4 py-3 text-gray-300 text-right">{ingredient.fat}g</td>
              <td className="px-4 py-3">
                <div className="flex items-center justify-center space-x-2">
                  <button
                    onClick={() => onEdit(ingredient)}
                    disabled={isLoading}
                    className="p-2 text-blue-400 hover:text-blue-300 hover:bg-blue-500/20 rounded-lg transition-all duration-200 disabled:opacity-50"
                    title="Edit"
                  >
                    <Edit size={16} />
                  </button>
                  <button
                    onClick={() => onDelete(ingredient.id)}
                    disabled={isLoading}
                    className="p-2 text-red-400 hover:text-red-300 hover:bg-red-500/20 rounded-lg transition-all duration-200 disabled:opacity-50"
                    title="Delete"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
