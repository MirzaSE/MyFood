import React, { useEffect, useMemo, useState } from 'react';
import { AlertCircle, Plus, Search } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import { getApiErrorMessage } from '../services/api';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [query, setQuery] = useState('');
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);

  const pageCount = 10;

  useEffect(() => {
    loadIngredients();
  }, [page]);

  const visibleIngredients = useMemo(() => ingredients, [ingredients]);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = query.trim()
        ? await ingredientService.searchIngredients(query, page, pageCount)
        : await ingredientService.getAllIngredients(page, pageCount);
      setIngredients(data);
    } catch (err) {
      setError(getApiErrorMessage(err, 'Failed to load ingredients'));
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearch = async (event: React.FormEvent) => {
    event.preventDefault();
    setPage(1);
    await loadIngredients();
  };

  const handleCreateClick = () => {
    setSelectedIngredient(null);
    setModalOpen(true);
  };

  const handleEditClick = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalOpen(true);
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setError(null);
      setSuccess(null);

      if (selectedIngredient) {
        await ingredientService.updateIngredient(selectedIngredient.id, data);
        setSuccess('Ingredient updated');
      } else {
        await ingredientService.createIngredient(data);
        setSuccess('Ingredient created');
      }

      setModalOpen(false);
      setSelectedIngredient(null);
      await loadIngredients();
    } catch (err) {
      setError(getApiErrorMessage(err, 'Failed to save ingredient'));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      setSuccess(null);
      await ingredientService.deleteIngredient(id);
      setSuccess('Ingredient deleted');
      await loadIngredients();
    } catch (err) {
      setError(getApiErrorMessage(err, 'Failed to delete ingredient'));
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <main className="container mx-auto px-6 py-12">
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start gap-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        {success && (
          <div className="mb-6 p-4 bg-emerald-500/15 border border-emerald-500/40 rounded-lg text-emerald-200">
            {success}
          </div>
        )}

        <div className="mb-8 flex flex-col lg:flex-row lg:items-end lg:justify-between gap-6">
          <div>
            <h1 className="text-4xl font-bold text-white mb-3">Ingredients</h1>
            <p className="text-gray-400">
              {ingredients.length} {ingredients.length === 1 ? 'ingredient' : 'ingredients'} loaded
            </p>
          </div>
          <button
            onClick={handleCreateClick}
            disabled={isLoading || isSubmitting}
            className="inline-flex items-center justify-center gap-2 bg-emerald-600 hover:bg-emerald-700 text-white px-5 py-3 rounded-lg transition-all duration-200 disabled:opacity-50 font-semibold shadow-lg hover:shadow-emerald-500/30"
          >
            <Plus size={20} />
            <span>New Ingredient</span>
          </button>
        </div>

        <form onSubmit={handleSearch} className="mb-6 flex flex-col sm:flex-row gap-3">
          <div className="relative flex-1">
            <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/30 text-white placeholder-gray-400"
              placeholder="Search ingredients"
            />
          </div>
          <button
            type="submit"
            className="px-5 py-3 bg-white/10 hover:bg-white/15 border border-white/20 text-white rounded-lg font-medium"
          >
            Search
          </button>
        </form>

        {isLoading ? (
          <div className="flex flex-col items-center justify-center py-20">
            <div className="w-14 h-14 border-4 border-emerald-500/30 border-t-emerald-400 rounded-full animate-spin mb-4" />
            <p className="text-gray-300 font-medium">Loading ingredients...</p>
          </div>
        ) : (
          <IngredientList
            ingredients={visibleIngredients}
            isLoading={isSubmitting}
            onEdit={handleEditClick}
            onDelete={handleDelete}
          />
        )}

        <div className="mt-6 flex justify-end gap-2">
          <button
            onClick={() => setPage((current) => Math.max(1, current - 1))}
            disabled={page === 1 || isLoading}
            className="px-4 py-2 bg-white/10 border border-white/20 text-gray-200 rounded-lg disabled:opacity-40"
          >
            Previous
          </button>
          <span className="px-4 py-2 text-gray-300">Page {page}</span>
          <button
            onClick={() => setPage((current) => current + 1)}
            disabled={isLoading || ingredients.length < pageCount}
            className="px-4 py-2 bg-white/10 border border-white/20 text-gray-200 rounded-lg disabled:opacity-40"
          >
            Next
          </button>
        </div>
      </main>

      <IngredientModal
        isOpen={modalOpen}
        initialData={selectedIngredient}
        isLoading={isSubmitting}
        onClose={() => {
          setModalOpen(false);
          setSelectedIngredient(null);
        }}
        onSubmit={handleSubmit}
      />
    </div>
  );
};
