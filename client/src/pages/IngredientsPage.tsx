import React, { useEffect, useMemo, useState } from 'react';
import { AlertCircle, Plus } from 'lucide-react';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { Navbar } from '../components/Navbar';
import { getApiErrorMessage } from '../services/api';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

const pageCount = 8;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [viewIngredient, setViewIngredient] = useState<Ingredient | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        setIsLoading(true);
        setError(null);
        setIngredients(await ingredientService.getAllIngredients());
      } catch (loadError: unknown) {
        setError(getApiErrorMessage(loadError, 'Failed to load ingredients'));
      } finally {
        setIsLoading(false);
      }
    };

    void loadIngredients();
  }, []);

  const filteredIngredients = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();
    if (!normalizedSearch) return ingredients;

    return ingredients.filter((ingredient) => (
      ingredient.name.toLowerCase().includes(normalizedSearch)
      || ingredient.unit.toLowerCase().includes(normalizedSearch)
    ));
  }, [ingredients, search]);

  const pagedIngredients = useMemo(() => {
    const start = (page - 1) * pageCount;
    return filteredIngredients.slice(start, start + pageCount);
  }, [filteredIngredients, page]);

  const handleSaved = (ingredient: Ingredient) => {
    setIngredients((currentIngredients) => {
      const exists = currentIngredients.some((item) => item.id === ingredient.id);
      if (exists) {
        return currentIngredients.map((item) => item.id === ingredient.id ? ingredient : item);
      }

      return [...currentIngredients, ingredient].sort((a, b) => a.name.localeCompare(b.name));
    });
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      setIngredients((currentIngredients) => currentIngredients.filter((ingredient) => ingredient.id !== id));
    } catch (deleteError: unknown) {
      setError(getApiErrorMessage(deleteError, 'Failed to delete ingredient'));
      throw deleteError;
    }
  };

  const handleSearchChange = (nextSearch: string) => {
    setSearch(nextSearch);
    setPage(1);
  };

  const openCreateModal = () => {
    setSelectedIngredient(null);
    setModalOpen(true);
  };

  const openEditModal = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalOpen(true);
  };

  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top_left,_rgba(16,185,129,0.22),_transparent_35%),linear-gradient(135deg,_#0f172a,_#111827_45%,_#042f2e)]">
      <Navbar />

      <main className="container mx-auto px-6 py-14">
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start gap-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-300 flex-shrink-0 mt-0.5" />
            <p className="text-red-100">{error}</p>
          </div>
        )}

        <div className="mb-10 flex flex-col sm:flex-row sm:items-end sm:justify-between gap-6">
          <div>
            <p className="text-emerald-300 font-semibold tracking-wide uppercase text-sm">Pantry intelligence</p>
            <h1 className="text-4xl sm:text-5xl font-black text-white mt-2">Ingredients</h1>
            <p className="text-gray-400 mt-3 max-w-2xl">Create reusable ingredients, track calories and macros, then pick them when building foods.</p>
          </div>
          <button
            className="inline-flex items-center justify-center gap-2 bg-gradient-to-r from-emerald-500 to-cyan-500 hover:from-emerald-600 hover:to-cyan-600 text-white px-6 py-3 rounded-xl transition-all disabled:opacity-50 shadow-lg hover:shadow-emerald-500/30 font-semibold"
            disabled={isLoading}
            onClick={openCreateModal}
            type="button"
          >
            <Plus size={20} />
            New Ingredient
          </button>
        </div>

        <IngredientList
          ingredients={pagedIngredients}
          isLoading={isLoading}
          onDelete={handleDelete}
          onEdit={openEditModal}
          onPageChange={setPage}
          onSearchChange={handleSearchChange}
          onView={setViewIngredient}
          page={page}
          pageCount={pageCount}
          search={search}
          totalCount={filteredIngredients.length}
        />
      </main>

      <IngredientModal
        initialData={selectedIngredient}
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSaved={handleSaved}
      />

      {viewIngredient && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-slate-900 border border-white/20 rounded-2xl max-w-md w-full p-6 shadow-2xl">
            <h2 className="text-2xl font-bold text-white">{viewIngredient.name}</h2>
            <p className="text-gray-400 mt-1">Per {viewIngredient.unit}</p>
            <div className="grid grid-cols-2 gap-3 mt-6 text-sm">
              <div className="rounded-xl bg-white/5 p-4"><span className="text-gray-400 block">Calories</span><strong className="text-white">{viewIngredient.caloriesPerUnit}</strong></div>
              <div className="rounded-xl bg-white/5 p-4"><span className="text-gray-400 block">Protein</span><strong className="text-white">{viewIngredient.protein}</strong></div>
              <div className="rounded-xl bg-white/5 p-4"><span className="text-gray-400 block">Carbs</span><strong className="text-white">{viewIngredient.carbs}</strong></div>
              <div className="rounded-xl bg-white/5 p-4"><span className="text-gray-400 block">Fat</span><strong className="text-white">{viewIngredient.fat}</strong></div>
            </div>
            <button
              className="mt-6 w-full px-4 py-3 rounded-lg border border-white/20 text-gray-200 hover:border-white/40"
              onClick={() => setViewIngredient(null)}
              type="button"
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
