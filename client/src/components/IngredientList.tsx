import React from 'react';
import { Eye, Pencil, Trash2 } from 'lucide-react';
import type { Ingredient } from '../types';

interface IngredientListProps {
  ingredients: Ingredient[];
  currentPage: number;
  pageSize: number;
  search: string;
  isLoading?: boolean;
  onSearchChange: (value: string) => void;
  onPageChange: (page: number) => void;
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => Promise<void>;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  currentPage,
  pageSize,
  search,
  isLoading = false,
  onSearchChange,
  onPageChange,
  onEdit,
  onDelete,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);

  const filtered = ingredients.filter((x) => x.name.toLowerCase().includes(search.toLowerCase()));
  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
  const start = (currentPage - 1) * pageSize;
  const pageItems = filtered.slice(start, start + pageSize);

  const handleDelete = async (id: number) => {
    setDeletingId(id);
    try {
      await onDelete(id);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-4">
      <input
        value={search}
        onChange={(e) => onSearchChange(e.target.value)}
        placeholder="Search ingredients..."
        className="w-full px-3 py-2 rounded-lg bg-white/10 border border-white/20 text-white"
      />

      {isLoading ? (
        <p className="text-gray-300">Loading ingredients...</p>
      ) : pageItems.length === 0 ? (
        <p className="text-gray-400">No ingredients found.</p>
      ) : (
        <div className="overflow-x-auto rounded-lg border border-white/10">
          <table className="w-full text-sm text-left">
            <thead className="bg-white/5 text-gray-200">
              <tr>
                <th className="px-3 py-2">Name</th>
                <th className="px-3 py-2">Unit</th>
                <th className="px-3 py-2">Calories</th>
                <th className="px-3 py-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {pageItems.map((item) => (
                <tr key={item.id} className="border-t border-white/10 text-gray-100">
                  <td className="px-3 py-2">{item.name}</td>
                  <td className="px-3 py-2">unit</td>
                  <td className="px-3 py-2">{item.quantity}</td>
                  <td className="px-3 py-2">
                    <div className="flex items-center gap-2">
                      <button title="View" className="text-sky-300">
                        <Eye size={16} />
                      </button>
                      <button title="Edit" className="text-amber-300" onClick={() => onEdit(item)}>
                        <Pencil size={16} />
                      </button>
                      <button
                        title="Delete"
                        className="text-red-300"
                        onClick={() => handleDelete(item.id)}
                        disabled={deletingId === item.id}
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
      )}

      <div className="flex items-center justify-between text-sm text-gray-300">
        <span>
          Page {currentPage} / {totalPages}
        </span>
        <div className="flex gap-2">
          <button
            className="px-3 py-1 rounded border border-white/20 disabled:opacity-40"
            disabled={currentPage <= 1}
            onClick={() => onPageChange(currentPage - 1)}
          >
            Prev
          </button>
          <button
            className="px-3 py-1 rounded border border-white/20 disabled:opacity-40"
            disabled={currentPage >= totalPages}
            onClick={() => onPageChange(currentPage + 1)}
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
};
