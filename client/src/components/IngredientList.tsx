import { Edit, Eye, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading: boolean;
  searchTerm: string;
  currentPage: number;
  pageSize: number;
  onSearchChange: (value: string) => void;
  onPageChange: (page: number) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
  onView: (ingredient: Ingredient) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading,
  searchTerm,
  currentPage,
  pageSize,
  onSearchChange,
  onPageChange,
  onEdit,
  onDelete,
  onView,
}) => {
  const totalPages = Math.max(1, Math.ceil(ingredients.length / pageSize));
  const startIndex = (currentPage - 1) * pageSize;
  const visibleIngredients = ingredients.slice(startIndex, startIndex + pageSize);

  const handleDelete = async (ingredient: Ingredient) => {
    const confirmed = window.confirm(`Delete ${ingredient.name}?`);

    if (confirmed) {
      await onDelete(ingredient.id);
    }
  };

  if (isLoading) {
    return (
      <div className="rounded-xl border border-gray-200 bg-white p-8 text-center">
        Loading ingredients...
      </div>
    );
  }

  return (
    <div className="rounded-xl border border-gray-200 bg-white shadow-sm">
      <div className="border-b border-gray-200 p-4">
        <input
          value={searchTerm}
          onChange={(event) => onSearchChange(event.target.value)}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
          placeholder="Search ingredients..."
        />
      </div>

      {ingredients.length === 0 ? (
        <div className="p-8 text-center text-gray-500">
          No ingredients found.
        </div>
      ) : (
        <>
          <div className="overflow-x-auto">
            <table className="w-full text-left">
              <thead className="bg-gray-50 text-sm text-gray-600">
                <tr>
                  <th className="px-4 py-3">Name</th>
                  <th className="px-4 py-3">Unit</th>
                  <th className="px-4 py-3">Calories</th>
                  <th className="px-4 py-3">Protein</th>
                  <th className="px-4 py-3">Carbs</th>
                  <th className="px-4 py-3">Fat</th>
                  <th className="px-4 py-3 text-right">Actions</th>
                </tr>
              </thead>
              <tbody>
                {visibleIngredients.map((ingredient) => (
                  <tr key={ingredient.id} className="border-t border-gray-100">
                    <td className="px-4 py-3 font-medium text-gray-900">
                      {ingredient.name}
                    </td>
                    <td className="px-4 py-3">{ingredient.unit}</td>
                    <td className="px-4 py-3">{ingredient.caloriesPerUnit}</td>
                    <td className="px-4 py-3">{ingredient.protein}</td>
                    <td className="px-4 py-3">{ingredient.carbs}</td>
                    <td className="px-4 py-3">{ingredient.fat}</td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                        <button
                          type="button"
                          onClick={() => onView(ingredient)}
                          className="rounded-lg p-2 text-gray-600 hover:bg-gray-100"
                          title="View"
                        >
                          <Eye size={18} />
                        </button>
                        <button
                          type="button"
                          onClick={() => onEdit(ingredient)}
                          className="rounded-lg p-2 text-blue-600 hover:bg-blue-50"
                          title="Edit"
                        >
                          <Edit size={18} />
                        </button>
                        <button
                          type="button"
                          onClick={() => handleDelete(ingredient)}
                          className="rounded-lg p-2 text-red-600 hover:bg-red-50"
                          title="Delete"
                        >
                          <Trash2 size={18} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex items-center justify-between border-t border-gray-200 p-4">
            <span className="text-sm text-gray-600">
              Page {currentPage} of {totalPages}
            </span>

            <div className="flex gap-2">
              <button
                type="button"
                onClick={() => onPageChange(Math.max(1, currentPage - 1))}
                disabled={currentPage === 1}
                className="rounded-lg border border-gray-300 px-3 py-1 disabled:opacity-50"
              >
                Previous
              </button>
              <button
                type="button"
                onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
                disabled={currentPage === totalPages}
                className="rounded-lg border border-gray-300 px-3 py-1 disabled:opacity-50"
              >
                Next
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
};