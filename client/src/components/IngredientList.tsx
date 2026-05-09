import React, { useState, useEffect, useCallback } from 'react';
import { Plus, Search, Edit2, Trash2, ChevronLeft, ChevronRight } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';
import { IngredientModal } from './IngredientModal';

export const IngredientList: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editIngredient, setEditIngredient] = useState<Ingredient | undefined>();
  const [deleteConfirm, setDeleteConfirm] = useState<number | null>(null);
  const PAGE_SIZE = 10;

  const loadIngredients = useCallback(async () => {
    setIsLoading(true);
    setError('');
    try {
      if (search.trim()) {
        const results = await ingredientService.search(search.trim());
        setIngredients(results);
        setTotalPages(1);
      } else {
        const response = await fetch(
          `${import.meta.env.VITE_API_URL || 'http://localhost:8080'}/api/v1/ingredients?page=${page}&pageCount=${PAGE_SIZE}`,
          { headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } }
        );
        const pagination = response.headers.get('X-Pagination');
        const data = await response.json();
        setIngredients(data);
        if (pagination) {
          const p = JSON.parse(pagination);
          setTotalPages(Math.ceil(p.totalCount / PAGE_SIZE) || 1);
        }
      }
    } catch {
      setError('Failed to load ingredients.');
    } finally {
      setIsLoading(false);
    }
  }, [page, search]);

  useEffect(() => { loadIngredients(); }, [loadIngredients]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    setSearch(searchInput);
  };

  const handleDelete = async (id: number) => {
    try {
      await ingredientService.delete(id);
      loadIngredients();
    } catch {
      setError('Failed to delete ingredient.');
    }
    setDeleteConfirm(null);
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <form onSubmit={handleSearch} className="flex gap-2 flex-1 max-w-sm">
          <div className="relative flex-1">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              value={searchInput}
              onChange={e => setSearchInput(e.target.value)}
              placeholder="Search ingredients..."
              className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <button type="submit" className="px-3 py-2 bg-gray-100 rounded-lg hover:bg-gray-200 text-sm">
            Search
          </button>
          {search && (
            <button type="button" onClick={() => { setSearch(''); setSearchInput(''); setPage(1); }}
              className="px-3 py-2 text-gray-500 hover:text-gray-700 text-sm">
              Clear
            </button>
          )}
        </form>

        <button
          onClick={() => { setEditIngredient(undefined); setShowModal(true); }}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 font-medium text-sm"
        >
          <Plus size={16} /> New Ingredient
        </button>
      </div>

      {error && <div className="bg-red-50 border border-red-200 rounded-lg p-3 mb-4 text-red-600 text-sm">{error}</div>}

      {isLoading ? (
        <div className="flex justify-center py-12">
          <div className="w-8 h-8 border-4 border-blue-500 border-t-transparent rounded-full animate-spin" />
        </div>
      ) : ingredients.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          <p className="text-lg font-medium">No ingredients found</p>
          <p className="text-sm mt-1">Add your first ingredient to get started.</p>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-gray-200">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="text-left px-4 py-3 font-semibold text-gray-600">Name</th>
                <th className="text-left px-4 py-3 font-semibold text-gray-600">Unit</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-600">Cal/unit</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-600">Protein</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-600">Carbs</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-600">Fat</th>
                <th className="text-center px-4 py-3 font-semibold text-gray-600">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {ingredients.map(ing => (
                <tr key={ing.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-4 py-3 font-medium text-gray-800">{ing.name}</td>
                  <td className="px-4 py-3 text-gray-500">{ing.unit || '—'}</td>
                  <td className="px-4 py-3 text-right text-gray-700">{ing.caloriesPerUnit}</td>
                  <td className="px-4 py-3 text-right text-gray-700">{ing.protein}g</td>
                  <td className="px-4 py-3 text-right text-gray-700">{ing.carbs}g</td>
                  <td className="px-4 py-3 text-right text-gray-700">{ing.fat}g</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-center gap-2">
                      <button
                        onClick={() => { setEditIngredient(ing); setShowModal(true); }}
                        className="p-1.5 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                        title="Edit"
                      >
                        <Edit2 size={15} />
                      </button>
                      {deleteConfirm === ing.id ? (
                        <div className="flex gap-1">
                          <button onClick={() => handleDelete(ing.id)}
                            className="px-2 py-1 bg-red-600 text-white rounded text-xs hover:bg-red-700">Yes</button>
                          <button onClick={() => setDeleteConfirm(null)}
                            className="px-2 py-1 bg-gray-200 rounded text-xs hover:bg-gray-300">No</button>
                        </div>
                      ) : (
                        <button
                          onClick={() => setDeleteConfirm(ing.id)}
                          className="p-1.5 text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                          title="Delete"
                        >
                          <Trash2 size={15} />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {!search && totalPages > 1 && (
        <div className="flex items-center justify-center gap-4 mt-6">
          <button
            onClick={() => setPage(p => Math.max(1, p - 1))}
            disabled={page === 1}
            className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-40"
          >
            <ChevronLeft size={16} />
          </button>
          <span className="text-sm text-gray-600">Page {page} of {totalPages}</span>
          <button
            onClick={() => setPage(p => Math.min(totalPages, p + 1))}
            disabled={page === totalPages}
            className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-40"
          >
            <ChevronRight size={16} />
          </button>
        </div>
      )}

      {showModal && (
        <IngredientModal
          ingredient={editIngredient}
          onClose={() => setShowModal(false)}
          onSaved={loadIngredients}
        />
      )}
    </div>
  );
};
