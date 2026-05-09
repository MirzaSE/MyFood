import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Plus, Search, AlertCircle, ChevronLeft, ChevronRight } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto, PaginationMeta } from '../types';

const PAGE_SIZE = 10;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [pagination, setPagination] = useState<PaginationMeta>({
    totalCount: 0,
    pageSize: PAGE_SIZE,
    currentPage: 1,
    totalPages: 0,
  });
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState('');
  const [activeSearch, setActiveSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [pageError, setPageError] = useState<string | null>(null);

  const [modalOpen, setModalOpen] = useState(false);
  const [selected, setSelected] = useState<Ingredient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [modalError, setModalError] = useState<string | null>(null);
  const [modalSuccess, setModalSuccess] = useState<string | null>(null);

  const loadIngredients = useCallback(async () => {
    try {
      setIsLoading(true);
      setPageError(null);
      const result = activeSearch
        ? await ingredientService.searchIngredients(activeSearch, { page, pageCount: PAGE_SIZE })
        : await ingredientService.getAllIngredients({ page, pageCount: PAGE_SIZE });
      setIngredients(result.items);
      setPagination(result.pagination);
    } catch (err: any) {
      setPageError(err.response?.data?.message || err.response?.data || 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  }, [activeSearch, page]);

  useEffect(() => {
    loadIngredients();
  }, [loadIngredients]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    setActiveSearch(searchInput.trim());
  };

  const handleClearSearch = () => {
    setSearchInput('');
    setActiveSearch('');
    setPage(1);
  };

  const handleCreate = () => {
    setSelected(null);
    setModalError(null);
    setModalSuccess(null);
    setModalOpen(true);
  };

  const handleEdit = (ingredient: Ingredient) => {
    setSelected(ingredient);
    setModalError(null);
    setModalSuccess(null);
    setModalOpen(true);
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setModalError(null);
      setModalSuccess(null);
      if (selected) {
        await ingredientService.updateIngredient(selected.id, data);
        setModalSuccess('Ingredient updated.');
      } else {
        await ingredientService.createIngredient(data);
        setModalSuccess('Ingredient created.');
      }
      await loadIngredients();
      setTimeout(() => {
        setModalOpen(false);
        setSelected(null);
        setModalSuccess(null);
      }, 600);
    } catch (err: any) {
      setModalError(err.response?.data?.message || err.response?.data || 'Failed to save ingredient');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setPageError(null);
      await ingredientService.deleteIngredient(id);
      await loadIngredients();
    } catch (err: any) {
      setPageError(err.response?.data?.message || err.response?.data || 'Failed to delete ingredient');
      throw err;
    }
  };

  const totalPages = useMemo(() => Math.max(pagination.totalPages, 1), [pagination.totalPages]);
  const canPrev = page > 1;
  const canNext = page < totalPages;

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {pageError && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{pageError}</p>
          </div>
        )}

        <div className="mb-12">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-3">Ingredients</h1>
              <p className="text-gray-400">
                {pagination.totalCount} {pagination.totalCount === 1 ? 'ingredient' : 'ingredients'} total
              </p>
            </div>
            <button
              onClick={handleCreate}
              disabled={isLoading || isSubmitting}
              className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-purple-500/50 font-semibold"
            >
              <Plus size={20} />
              <span>New Ingredient</span>
            </button>
          </div>
        </div>

        <form onSubmit={handleSearchSubmit} className="mb-8 flex gap-3">
          <div className="relative flex-1">
            <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              placeholder="Search ingredients by name..."
              className="w-full pl-10 pr-3 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            />
          </div>
          <button
            type="submit"
            className="px-5 py-3 bg-purple-500/30 hover:bg-purple-500/40 text-purple-100 border border-purple-500/40 rounded-lg font-medium transition-all"
          >
            Search
          </button>
          {activeSearch && (
            <button
              type="button"
              onClick={handleClearSearch}
              className="px-5 py-3 border border-white/20 text-gray-300 hover:text-white hover:border-white/40 rounded-lg font-medium transition-all"
            >
              Clear
            </button>
          )}
        </form>

        <IngredientList
          ingredients={ingredients}
          isLoading={isLoading}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />

        {!isLoading && ingredients.length > 0 && (
          <div className="mt-6 flex items-center justify-between text-sm">
            <p className="text-gray-400">
              Page {pagination.currentPage} of {totalPages}
            </p>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={!canPrev}
                className="flex items-center gap-1 px-4 py-2 border border-white/20 text-gray-300 hover:text-white hover:border-white/40 rounded-lg transition-all disabled:opacity-40 disabled:cursor-not-allowed"
              >
                <ChevronLeft size={16} /> Prev
              </button>
              <button
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={!canNext}
                className="flex items-center gap-1 px-4 py-2 border border-white/20 text-gray-300 hover:text-white hover:border-white/40 rounded-lg transition-all disabled:opacity-40 disabled:cursor-not-allowed"
              >
                Next <ChevronRight size={16} />
              </button>
            </div>
          </div>
        )}
      </div>

      <IngredientModal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          setSelected(null);
          setModalError(null);
          setModalSuccess(null);
        }}
        onSubmit={handleSubmit}
        initialData={selected}
        isLoading={isSubmitting}
        successMessage={modalSuccess}
        errorMessage={modalError}
      />
    </div>
  );
};
