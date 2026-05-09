import { useState } from 'react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading: boolean;
  onView: (ingredient: Ingredient) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => void;
  onSearch: (term: string) => void;
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export function IngredientList({
  ingredients,
  isLoading,
  onView,
  onEdit,
  onDelete,
  onSearch,
  currentPage,
  totalPages,
  onPageChange,
}: IngredientListProps) {
  const [searchTerm, setSearchTerm] = useState('');

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const term = e.target.value;
    setSearchTerm(term);
    onSearch(term);
  };

  if (isLoading && ingredients.length === 0) {
    return <div className="text-center py-8">Loading ingredients...</div>;
  }

  if (ingredients.length === 0) {
    return (
      <div>
        <input
          type="text"
          placeholder="Search ingredients..."
          value={searchTerm}
          onChange={handleSearchChange}
          className="mb-4 w-full px-4 py-2 border border-gray-300 rounded-md"
        />
        <div className="text-center py-8 text-gray-600">
          No ingredients found. Create your first ingredient!
        </div>
      </div>
    );
  }

  return (
    <div>
      <input
        type="text"
        placeholder="Search ingredients..."
        value={searchTerm}
        onChange={handleSearchChange}
        className="mb-4 w-full px-4 py-2 border border-gray-300 rounded-md"
      />

      <div className="overflow-x-auto">
        <table className="w-full border-collapse">
          <thead>
            <tr className="bg-gray-200">
              <th className="border px-4 py-2 text-left">Name</th>
              <th className="border px-4 py-2 text-left">Unit</th>
              <th className="border px-4 py-2 text-right">Calories/Unit</th>
              <th className="border px-4 py-2 text-right">Protein (g)</th>
              <th className="border px-4 py-2 text-right">Carbs (g)</th>
              <th className="border px-4 py-2 text-right">Fat (g)</th>
              <th className="border px-4 py-2 text-center">Actions</th>
            </tr>
          </thead>
          <tbody>
            {ingredients.map(ingredient => (
              <tr key={ingredient.id} className="hover:bg-gray-50">
                <td className="border px-4 py-2">{ingredient.name}</td>
                <td className="border px-4 py-2">{ingredient.unit}</td>
                <td className="border px-4 py-2 text-right">{ingredient.caloriesPerUnit.toFixed(2)}</td>
                <td className="border px-4 py-2 text-right">{ingredient.protein.toFixed(2)}</td>
                <td className="border px-4 py-2 text-right">{ingredient.carbs.toFixed(2)}</td>
                <td className="border px-4 py-2 text-right">{ingredient.fat.toFixed(2)}</td>
                <td className="border px-4 py-2 text-center">
                  <button
                    onClick={() => onView(ingredient)}
                    className="bg-gray-500 text-white px-3 py-1 rounded mr-2 hover:bg-gray-600"
                  >
                    View
                  </button>
                  <button
                    onClick={() => onEdit(ingredient)}
                    className="bg-blue-500 text-white px-3 py-1 rounded mr-2 hover:bg-blue-600"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => {
                      if (confirm('Are you sure?')) {
                        onDelete(ingredient.id);
                      }
                    }}
                    className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-4">
          <button
            onClick={() => onPageChange(currentPage - 1)}
            disabled={currentPage === 1}
            className="px-4 py-2 border border-gray-300 rounded disabled:opacity-50"
          >
            Previous
          </button>
          <span className="px-4 py-2">
            Page {currentPage} of {totalPages}
          </span>
          <button
            onClick={() => onPageChange(currentPage + 1)}
            disabled={currentPage === totalPages}
            className="px-4 py-2 border border-gray-300 rounded disabled:opacity-50"
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}
