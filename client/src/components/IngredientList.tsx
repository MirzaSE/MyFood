import React, { useState, useEffect } from 'react';
import { Pencil, Trash2, Search, ChevronLeft, ChevronRight } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

interface Props {
  onEdit: (ingredient: Ingredient) => void;
  refreshKey: number;
}

export const IngredientList: React.FC<Props> = ({ onEdit, refreshKey }) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const pageSize = 8;

  useEffect(() => {
    loadIngredients();
  }, [refreshKey]);

  const loadIngredients = async () => {
    setIsLoading(true);
    try {
      const data = await ingredientService.getAll();
      setIngredients(data);
    } catch {
      setIngredients([]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this ingredient?')) return;
    try {
      await ingredientService.delete(id);
      loadIngredients();
    } catch {
      alert('Failed to delete.');
    }
  };

  const filtered = ingredients.filter(i =>
    i.name.toLowerCase().includes(search.toLowerCase())
  );
  const totalPages = Math.ceil(filtered.length / pageSize);
  const paginated = filtered.slice((page - 1) * pageSize, page * pageSize);

  return (
    <div>
      {/* Search */}
      <div className="relative mb-4">
        <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
        <input
          value={search}
          onChange={e => { setSearch(e.target.value); setPage(1); }}
          placeholder="Search ingredients..."
          className="w-full pl-9 pr-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white text-sm"
        />
      </div>

      {/* Table */}
      {isLoading ? (
        <div className="text-center py-10 text-gray-400">Loading...</div>
      ) : paginated.length === 0 ? (
        <div className="text-center py-10 text-gray-400">No ingredients found.</div>
      ) : (
        <table className="w-full text-sm text-left">
          <thead>
            <tr className="border-b border-white/10 text-gray-400">
              <th className="pb-3 font-medium">Name</th>
              <th className="pb-3 font-medium">Unit</th>
              <th className="pb-3 font-medium">Calories/Unit</th>
              <th className="pb-3 font-medium text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {paginated.map(ing => (
              <tr key={ing.id} className="border-b border-white/5 hover:bg-white/5">
                <td className="py-3 text-white font-medium">{ing.name}</td>
                <td className="py-3 text-gray-300">{ing.unit}</td>
                <td className="py-3 text-gray-300">{ing.caloriesPerUnit} kcal</td>
                <td className="py-3 text-right space-x-2">
                  <button
                    onClick={() => onEdit(ing)}
                    className="px-3 py-1 bg-blue-600/30 text-blue-300 rounded hover:bg-blue-600/50 transition"
                  >
                    <Pencil size={14} />
                  </button>
                  <button
                    onClick={() => handleDelete(ing.id)}
                    className="px-3 py-1 bg-red-600/30 text-red-300 rounded hover:bg-red-600/50 transition"
                  >
                    <Trash2 size={14} />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between mt-4">
          <span className="text-gray-400 text-sm">
            Page {page} of {totalPages}
          </span>
          <div className="flex space-x-2">
            <button
              onClick={() => setPage(p => Math.max(1, p - 1))}
              disabled={page === 1}
              className="p-1 text-gray-400 hover:text-white disabled:opacity-40"
            >
              <ChevronLeft size={18} />
            </button>
            <button
              onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="p-1 text-gray-400 hover:text-white disabled:opacity-40"
            >
              <ChevronRight size={18} />
            </button>
          </div>
        </div>
      )}
    </div>
  );
};