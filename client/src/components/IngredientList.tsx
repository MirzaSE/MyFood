import React from 'react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  page: number;
  onPageChange: (page: number) => void;
  search: string;
  onSearchChange: (value: string) => void;
  onEdit: (ingredient: Ingredient) => void;
  onView: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients, isLoading = false, page, onPageChange, search, onSearchChange, onEdit, onView, onDelete,
}) => {
  if (isLoading) {
    return <div className="text-gray-300">Loading ingredients...</div>;
  }

  return (
    <div className="space-y-4">
      <input value={search} onChange={(e) => onSearchChange(e.target.value)} placeholder="Search ingredients" className="w-full p-2 rounded bg-slate-800 border border-slate-600 text-white" />
      {ingredients.length === 0 ? (
        <div className="text-gray-300">No ingredients found.</div>
      ) : (
        <table className="w-full text-left text-gray-200 border-collapse">
          <thead>
            <tr className="border-b border-slate-700">
              <th className="p-2">Name</th>
              <th className="p-2">Unit</th>
              <th className="p-2">Calories</th>
              <th className="p-2">Actions</th>
            </tr>
          </thead>
          <tbody>
            {ingredients.map((ingredient) => (
              <tr key={ingredient.id} className="border-b border-slate-800">
                <td className="p-2">{ingredient.name}</td>
                <td className="p-2">{ingredient.unit}</td>
                <td className="p-2">{ingredient.caloriesPerUnit}</td>
                <td className="p-2 space-x-2">
                  <button className="px-2 py-1 bg-slate-700 rounded" onClick={() => onView(ingredient)}>View</button>
                  <button className="px-2 py-1 bg-blue-700 rounded" onClick={() => onEdit(ingredient)}>Edit</button>
                  <button className="px-2 py-1 bg-red-700 rounded" onClick={() => onDelete(ingredient.id)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
      <div className="flex gap-2">
        <button className="px-3 py-1 bg-slate-700 rounded text-white" disabled={page <= 1} onClick={() => onPageChange(page - 1)}>Prev</button>
        <span className="text-gray-300">Page {page}</span>
        <button className="px-3 py-1 bg-slate-700 rounded text-white" onClick={() => onPageChange(page + 1)}>Next</button>
      </div>
    </div>
  );
};
