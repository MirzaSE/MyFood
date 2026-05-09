import React, { useCallback, useEffect, useState } from 'react';
import { Plus, AlertCircle, ChefHat } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

const PAGE_SIZE = 10;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selected, setSelected] = useState<Ingredient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [modalError, setModalError] = useState<string | null>(null);
  const [modalSuccess, setModalSuccess] = useState<string | null>(null);
  const [viewing, setViewing] = useState<Ingredient | null>(null);

  const loadIngredients = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const result = search.trim()
        ? await ingredientService.searchIngredients(search.trim(), page, PAGE_SIZE)
        : await ingredientService.getAllIngredients(page, PAGE_SIZE);
      setIngredients(result.items);
      setTotalPages(result.totalPages);
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  }, [page, search]);

  useEffect(() => {
    const handle = setTimeout(loadIngredients, 250);
    return () => clearTimeout(handle);
  }, [loadIngredients]);

  const openCreate = () => {
    setSelected(null);
    setModalError(null);
    setModalSuccess(null);
    setModalOpen(true);
  };

  const openEdit = (ingredient: Ingredient) => {
    setSelected(ingredient);
    setModalError(null);
    setModalSuccess(null);
    setModalOpen(true);
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setModalError(null);
      if (selected) {
        const updated = await ingredientService.updateIngredient(selected.id, data);
        setIngredients((prev) => prev.map((i) => (i.id === selected.id ? updated : i)));
        setModalSuccess('Ingredient updated.');
      } else {
        await ingredientService.createIngredient(data);
        setModalSuccess('Ingredient created.');
        await loadIngredients();
      }
      setTimeout(() => {
        setModalOpen(false);
        setSelected(null);
        setModalSuccess(null);
      }, 600);
    } catch (err: any) {
      setModalError(err?.response?.data?.message ?? 'Failed to save ingredient');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await ingredientService.deleteIngredient(id);
      setIngredients((prev) => prev.filter((i) => i.id !== id));
      // refresh count/pagination
      await loadIngredients();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? 'Failed to delete ingredient');
      throw err;
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-12">
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <div className="mb-10">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-3 flex items-center space-x-3">
                <ChefHat size={36} className="text-purple-400" />
                <span>Ingredients</span>
              </h1>
              <p className="text-gray-400">Manage the catalog of ingredients used to compose foods.</p>
            </div>
            <button
              onClick={openCreate}
              className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 disabled:opacity-50 shadow-lg hover:shadow-purple-500/50 font-semibold"
            >
              <Plus size={20} />
              <span>New Ingredient</span>
            </button>
          </div>
        </div>

        <IngredientList
          ingredients={ingredients}
          isLoading={isLoading}
          search={search}
          onSearchChange={(value) => {
            setSearch(value);
            setPage(1);
          }}
          page={page}
          totalPages={totalPages}
          onPageChange={setPage}
          onEdit={openEdit}
          onView={(ingredient) => setViewing(ingredient)}
          onDelete={handleDelete}
        />
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
        errorMessage={modalError}
        successMessage={modalSuccess}
      />

      {viewing && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4" onClick={() => setViewing(null)}>
          <div onClick={(e) => e.stopPropagation()} className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-md w-full overflow-hidden">
            <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-5">
              <h2 className="text-xl font-bold text-white">{viewing.name}</h2>
              <p className="text-purple-100 text-sm">Per {viewing.unit}</p>
            </div>
            <div className="p-6 space-y-3 text-gray-200">
              <Row label="Calories" value={`${viewing.caloriesPerUnit} kcal`} />
              <Row label="Protein" value={`${viewing.protein.toFixed(2)} g`} />
              <Row label="Carbs" value={`${viewing.carbs.toFixed(2)} g`} />
              <Row label="Fat" value={`${viewing.fat.toFixed(2)} g`} />
              <button onClick={() => setViewing(null)} className="w-full mt-4 px-4 py-2 bg-white/10 hover:bg-white/20 rounded-lg border border-white/20 text-white">Close</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

const Row: React.FC<{ label: string; value: string }> = ({ label, value }) => (
  <div className="flex items-center justify-between p-3 bg-white/5 rounded-lg border border-white/5">
    <span className="text-gray-400 text-sm">{label}</span>
    <span className="text-white font-semibold">{value}</span>
  </div>
);
