import React, { useState, useEffect } from 'react';
import { Edit, Trash2, Eye, Search, Loader } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading?: boolean;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => void;
  onView?: (ingredient: Ingredient) => void;
  onSearch?: (query: string) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading = false,
  onEdit,
  onDelete,
  onView,
  onSearch,
}) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    const query = e.target.value;
    setSearchQuery(query);
    onSearch?.(query);
  };

  const filteredIngredients = ingredients
  .filter((ing) => ing && ing.name) // Add null check
  .filter((ing) =>
    ing.name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const totalPages = Math.ceil(filteredIngredients.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const paginatedIngredients = filteredIngredients.slice(
    startIndex,
    startIndex + itemsPerPage
  );

  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader size={32} className="text-blue-400 animate-spin" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Search Bar */}
      <div className="relative">
        <Search size={20} className="absolute left-3 top-3 text-gray-400" />
        <input
          type="text"
          placeholder="Search ingredients..."
          value={searchQuery}
          onChange={handleSearch}
          className="w-full pl-10 pr-4 py-2 bg-gray-700 border border-gray-600 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {/* Table */}
      <div className="overflow-x-auto">
        <table className="w-full">
          <thead>
            <tr className="border-b border-gray-600">
              <th className="text-left px-4 py-3 font-semibold text-gray-300">Name</th>
              <th className="text-left px-4 py-3 font-semibold text-gray-300">Quantity</th>
              <th className="text-left px-4 py-3 font-semibold text-gray-300">Food ID</th>
              <th className="text-right px-4 py-3 font-semibold text-gray-300">Actions</th>
            </tr>
          </thead>
          <tbody>
            {paginatedIngredients.length === 0 ? (
              <tr>
                <td colSpan={4} className="text-center py-8 text-gray-400">
                  {filteredIngredients.length === 0 ? 'No ingredients found' : 'No ingredients to display'}
                </td>
              </tr>
            ) : (
              paginatedIngredients.map((ingredient) => ( ingredient &&
                <tr key={ingredient.id} className="border-b border-gray-700 hover:bg-gray-700/30 transition">
                  <td className="px-4 py-3 text-gray-200">{ingredient.name}</td>
                  <td className="px-4 py-3 text-gray-200">{ingredient.quantity}</td>
                  <td className="px-4 py-3 text-gray-200">{ingredient.foodEntityId}</td>
                  <td className="px-4 py-3 text-right">
                    <div className="flex justify-end space-x-2">
                      {onView && (
                        <button
                          onClick={() => onView(ingredient)}
                          className="p-2 text-blue-400 hover:bg-blue-500/20 rounded transition"
                          title="View"
                        >
                          <Eye size={18} />
                        </button>
                      )}
                      <button
                        onClick={() => onEdit(ingredient)}
                        className="p-2 text-yellow-400 hover:bg-yellow-500/20 rounded transition"
                        title="Edit"
                      >
                        <Edit size={18} />
                      </button>
                      <button
                        onClick={() => {
                          if (window.confirm('Are you sure you want to delete this ingredient?')) {
                            onDelete(ingredient.id);
                          }
                        }}
                        className="p-2 text-red-400 hover:bg-red-500/20 rounded transition"
                        title="Delete"
                      >
                        <Trash2 size={18} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-gray-400">
            Page {currentPage} of {totalPages}
          </p>
          <div className="flex space-x-2">
            <button
              onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
              disabled={currentPage === 1}
              className="px-3 py-1 bg-gray-700 disabled:bg-gray-800 disabled:text-gray-500 text-gray-300 rounded hover:bg-gray-600 transition"
            >
              Previous
            </button>
            <button
              onClick={() => setCurrentPage(Math.min(totalPages, currentPage + 1))}
              disabled={currentPage === totalPages}
              className="px-3 py-1 bg-gray-700 disabled:bg-gray-800 disabled:text-gray-500 text-gray-300 rounded hover:bg-gray-600 transition"
            >
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
