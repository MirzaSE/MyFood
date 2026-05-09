import React, { useEffect, useMemo, useState } from 'react';
import { Edit, Trash2, Eye, Search, Leaf, ChevronLeft, ChevronRight } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

interface IngredientListProps {
  refreshKey?: number;
  onEdit: (ingredient: Ingredient) => void;
  onView?: (ingredient: Ingredient) => void;
}

const PAGE_SIZE = 10;

export const IngredientList: React.FC<IngredientListProps> = ({ refreshKey, onEdit, onView }) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [confirmId, setConfirmId] = useState<number | null>(null);

  const load = useMemo(
    () => async () => {
      setIsLoading(true);
      setError(null);
      try {
        if (search.trim()) {
          const items = await ingredientService.search(search.trim());
          setIngredients(items);
          setTotalPages(1);
        } else {
          const result = await ingredientService.getAll(page, PAGE_SIZE);
          setIngredients(result.items);
          setTotalPages(result.pagination?.totalPages ?? 1);
        }
      } catch (err: unknown) {
        const e = err as { response?: { data?: { message?: string } } };
        setError(e.response?.data?.message ?? 'Failed to load ingredients.');
      } finally {
        setIsLoading(false);
      }
    },
    [page, search]
  );

  useEffect(() => {
    load();
  }, [load, refreshKey]);

  const handleDelete = async (id: number) => {
    setDeletingId(id);
    try {
      await ingredientService.remove(id);
      setIngredients((prev) => prev.filter((i) => i.id !== id));
      setConfirmId(null);
    } catch (err: unknown) {
      const e = err as { response?: { data?: { message?: string } } };
      setError(e.response?.data?.message ?? 'Failed to delete ingredient.');
    } finally {
      setDeletingId(null);
    }
  };

  const submitSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    setSearch(searchInput);
  };

  const clearSearch = () => {
    setSearchInput('');
    setSearch('');
    setPage(1);
  };

  return (
    <div className="space-y-4">
      {/* Search bar */}
      <form onSubmit={submitSearch} className="flex gap-2">
        <div className="relative flex-1">
          <Search
            size={18}
            className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none"
          />
          <input
            type="text"
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            placeholder="Search ingredients by name…"
            className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
          />
        </div>
        <button
          type="submit"
          className="px-5 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg font-medium transition-all shadow-lg"
        >
          Search
        </button>
        {search && (
          <button
            type="button"
            onClick={clearSearch}
            className="px-5 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all font-medium"
          >
            Clear
          </button>
        )}
      </form>

      {error && (
        <div className="p-3 bg-red-500/20 border border-red-500/50 rounded-lg text-red-200 text-sm">
          {error}
        </div>
      )}

      {/* Loading state */}
      {isLoading ? (
        <div className="flex items-center justify-center py-20">
          <div className="relative w-12 h-12">
            <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin" />
            <div className="absolute inset-2 bg-slate-900 rounded-full" />
          </div>
        </div>
      ) : ingredients.length === 0 ? (
        // Empty state
        <div className="text-center py-16">
          <div className="flex justify-center mb-4">
            <div className="p-4 bg-purple-500/20 rounded-full">
              <Leaf size={32} className="text-purple-400" />
            </div>
          </div>
          <p className="text-gray-300 text-lg font-medium">No ingredients found</p>
          <p className="text-gray-400 text-sm mt-1">
            {search ? 'Try a different search term.' : 'Create one to get started!'}
          </p>
        </div>
      ) : (
        <>
          {/* Table */}
          <div className="overflow-x-auto bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 rounded-xl">
            <table className="w-full text-left">
              <thead>
                <tr className="border-b border-white/10 text-gray-300 text-sm">
                  <th className="px-5 py-3 font-semibold">Name</th>
                  <th className="px-5 py-3 font-semibold">Unit</th>
                  <th className="px-5 py-3 font-semibold">Calories</th>
                  <th className="px-5 py-3 font-semibold text-right">Actions</th>
                </tr>
              </thead>
              <tbody>
                {ingredients.map((ing) => (
                  <tr
                    key={ing.id}
                    className="border-b border-white/5 last:border-0 hover:bg-white/5 transition-colors"
                  >
                    <td className="px-5 py-3 text-white font-medium">{ing.name}</td>
                    <td className="px-5 py-3 text-gray-300">{ing.unit ?? '—'}</td>
                    <td className="px-5 py-3 text-gray-300">
                      {ing.caloriesPerUnit} kcal / {ing.unit ?? 'unit'}
                    </td>
                    <td className="px-5 py-3">
                      <div className="flex justify-end items-center gap-2">
                        {onView && (
                          <button
                            onClick={() => onView(ing)}
                            className="p-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 rounded-lg border border-blue-500/30 transition-all"
                            title="View"
                          >
                            <Eye size={16} />
                          </button>
                        )}
                        <button
                          onClick={() => onEdit(ing)}
                          className="p-2 bg-purple-500/20 hover:bg-purple-500/30 text-purple-300 rounded-lg border border-purple-500/30 transition-all"
                          title="Edit"
                        >
                          <Edit size={16} />
                        </button>
                        <div className="relative">
                          <button
                            onClick={() => setConfirmId(ing.id)}
                            disabled={deletingId !== null}
                            className="p-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 rounded-lg border border-red-500/30 transition-all disabled:opacity-50"
                            title="Delete"
                          >
                            <Trash2 size={16} />
                          </button>
                          {confirmId === ing.id && (
                            <div className="absolute right-0 top-full mt-2 bg-slate-900 border border-red-500/50 rounded-lg p-3 z-10 w-56 shadow-xl">
                              <p className="text-sm text-gray-200 mb-2 font-medium">
                                Delete this ingredient?
                              </p>
                              <p className="text-xs text-gray-400 mb-3">
                                This action cannot be undone.
                              </p>
                              <div className="flex space-x-2">
                                <button
                                  onClick={() => setConfirmId(null)}
                                  disabled={deletingId === ing.id}
                                  className="flex-1 px-3 py-1.5 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded text-sm transition-all disabled:opacity-50"
                                >
                                  Cancel
                                </button>
                                <button
                                  onClick={() => handleDelete(ing.id)}
                                  disabled={deletingId === ing.id}
                                  className="flex-1 px-3 py-1.5 bg-red-500 hover:bg-red-600 text-white rounded text-sm transition-all disabled:opacity-50"
                                >
                                  {deletingId === ing.id ? 'Deleting…' : 'Delete'}
                                </button>
                              </div>
                            </div>
                          )}
                        </div>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Pagination — only when not searching */}
          {!search && totalPages > 1 && (
            <div className="flex items-center justify-between text-sm text-gray-300">
              <span>
                Page {page} of {totalPages}
              </span>
              <div className="flex gap-2">
                <button
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1}
                  className="flex items-center gap-1 px-3 py-2 bg-white/10 hover:bg-white/20 border border-white/20 rounded-lg disabled:opacity-40 disabled:cursor-not-allowed transition-all"
                >
                  <ChevronLeft size={16} /> Previous
                </button>
                <button
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages}
                  className="flex items-center gap-1 px-3 py-2 bg-white/10 hover:bg-white/20 border border-white/20 rounded-lg disabled:opacity-40 disabled:cursor-not-allowed transition-all"
                >
                  Next <ChevronRight size={16} />
                </button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
};
