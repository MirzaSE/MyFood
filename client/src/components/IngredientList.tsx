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
        const { data, totalPages: pages } = await ingredientService.getAll(page, PAGE_SIZE);
        setIngredients(data);
        setTotalPages(pages);
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
      <div className="flex items-center justify-between mb-6 gap-4">
        <form onSubmit={handleSearch} className="flex gap-2 flex-1 max-w-sm">
          <div className="relative flex-1">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              value={searchInput}
              onChange={e => setSearchInput(e.target.value)}
              placeholder="Search ingredients..."
              className="w-full pl-9 pr-3 py-2 bg-white/10 border border-white/20 rounded-lg text-sm text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 transition-all"
            />
          </div>
          <button type="submit" className="px-3 py-2 bg-white/10 border border-white/20 rounded-lg hover:bg-white/20 text-sm text-gray-300 transition-colors">
            Search
          </button>
          {search && (
            <button type="button" onClick={() => { setSearch(''); setSearchInput(''); setPage(1); }}
              className="px-3 py-2 text-gray-400 hover:text-white text-sm transition-colors">
              Clear
            </button>
          )}
        </form>

        <button
          onClick={() => { setEditIngredient(undefined); setShowModal(true); }}
          className="flex items-center gap-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-4 py-2 rounded-lg font-medium text-sm transition-all shadow-lg hover:shadow-purple-500/50"
        >
          <Plus size={16} /> New Ingredient
        </button>
      </div>

      {error && (
        <div className="bg-red-500/20 border border-red-500/50 rounded-lg p-3 mb-4 text-red-300 text-sm backdrop-blur">
          {error}
        </div>
      )}

      {isLoading ? (
        <div className="flex flex-col items-center justify-center py-20">
          <div className="relative w-12 h-12 mb-4">
            <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>
            <div className="absolute inset-2 bg-slate-900 rounded-full"></div>
          </div>
          <p className="text-gray-400 text-sm">Loading ingredients...</p>
        </div>
      ) : ingredients.length === 0 ? (
        <div className="text-center py-16">
          <p className="text-gray-300 text-lg font-medium">No ingredients found</p>
          <p className="text-gray-500 text-sm mt-1">Add your first ingredient to get started.</p>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-white/10">
          <table className="w-full text-sm">
            <thead className="bg-white/5 border-b border-white/10">
              <tr>
                <th className="text-left px-4 py-3 font-semibold text-gray-400">Name</th>
                <th className="text-left px-4 py-3 font-semibold text-gray-400">Unit</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-400">Cal/unit</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-400">Protein</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-400">Carbs</th>
                <th className="text-right px-4 py-3 font-semibold text-gray-400">Fat</th>
                <th className="text-center px-4 py-3 font-semibold text-gray-400">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/5">
              {ingredients.map(ing => (
                <tr key={ing.id} className="hover:bg-white/5 transition-colors">
                  <td className="px-4 py-3 font-medium text-white">{ing.name}</td>
                  <td className="px-4 py-3 text-gray-400">{ing.unit || '—'}</td>
                  <td className="px-4 py-3 text-right text-gray-300">{ing.caloriesPerUnit}</td>
                  <td className="px-4 py-3 text-right text-gray-300">{ing.protein}g</td>
                  <td className="px-4 py-3 text-right text-gray-300">{ing.carbs}g</td>
                  <td className="px-4 py-3 text-right text-gray-300">{ing.fat}g</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-center gap-2">
                      <button
                        onClick={() => { setEditIngredient(ing); setShowModal(true); }}
                        className="p-1.5 text-purple-400 hover:bg-purple-500/20 rounded-lg transition-colors"
                        title="Edit"
                      >
                        <Edit2 size={15} />
                      </button>
                      {deleteConfirm === ing.id ? (
                        <div className="flex gap-1">
                          <button onClick={() => handleDelete(ing.id)}
                            className="px-2 py-1 bg-red-600 hover:bg-red-700 text-white rounded text-xs transition-colors">Yes</button>
                          <button onClick={() => setDeleteConfirm(null)}
                            className="px-2 py-1 bg-white/10 hover:bg-white/20 text-gray-300 rounded text-xs transition-colors">No</button>
                        </div>
                      ) : (
                        <button
                          onClick={() => setDeleteConfirm(ing.id)}
                          className="p-1.5 text-red-400 hover:bg-red-500/20 rounded-lg transition-colors"
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
            className="p-2 rounded-lg border border-white/20 hover:bg-white/10 text-gray-400 hover:text-white disabled:opacity-40 transition-colors"
          >
            <ChevronLeft size={16} />
          </button>
          <span className="text-sm text-gray-400">Page {page} of {totalPages}</span>
          <button
            onClick={() => setPage(p => Math.min(totalPages, p + 1))}
            disabled={page === totalPages}
            className="p-2 rounded-lg border border-white/20 hover:bg-white/10 text-gray-400 hover:text-white disabled:opacity-40 transition-colors"
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
