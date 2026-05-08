import React, { useEffect, useMemo, useState } from 'react';
import { Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientFormValues } from '../types/ingredient';

const PAGE_SIZE = 8;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<'create' | 'edit' | 'view'>('create');
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const data = searchTerm.trim()
          ? await ingredientService.searchIngredients(searchTerm)
          : await ingredientService.getAllIngredients();
        setIngredients(data);
      } catch (loadError) {
        const message = loadError instanceof Error ? loadError.message : 'Failed to load ingredients';
        setError(message);
      } finally {
        setIsLoading(false);
      }
    };

    void loadIngredients();
  }, [searchTerm]);

  const pagedIngredients = useMemo(() => {
    const start = (page - 1) * PAGE_SIZE;
    return ingredients.slice(start, start + PAGE_SIZE);
  }, [ingredients, page]);

  useEffect(() => {
    const totalPages = Math.max(1, Math.ceil(ingredients.length / PAGE_SIZE));
    if (page > totalPages) {
      setPage(totalPages);
    }
  }, [ingredients.length, page]);

  const closeModal = () => {
    setModalOpen(false);
    setSelectedIngredient(null);
  };

  const handleCreate = () => {
    setModalMode('create');
    setSelectedIngredient(null);
    setModalOpen(true);
  };

  const handleSubmit = async (values: IngredientFormValues) => {
    try {
      setIsSubmitting(true);
      setError(null);

      if (modalMode === 'edit' && selectedIngredient) {
        const updated = await ingredientService.updateIngredient(selectedIngredient.id, values);
        setIngredients((current) => current.map((ingredient) => (ingredient.id === updated.id ? updated : ingredient)));
      } else {
        const created = await ingredientService.createIngredient(values);
        setIngredients((current) => [created, ...current]);
      }
    } catch (submitError) {
      const message = submitError instanceof Error ? submitError.message : 'Failed to save ingredient';
      setError(message);
      throw submitError;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      setIngredients((current) => current.filter((ingredient) => ingredient.id !== id));
    } catch (deleteError) {
      const message = deleteError instanceof Error ? deleteError.message : 'Failed to delete ingredient';
      setError(message);
      throw deleteError;
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-12 space-y-8">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h1 className="text-4xl font-bold text-white">Ingredients</h1>
            <p className="mt-2 text-gray-400">Manage ingredient data, nutrition, and quick actions.</p>
          </div>
          <button
            onClick={handleCreate}
            className="inline-flex items-center gap-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-5 py-3 font-semibold text-white"
          >
            <Plus size={18} /> New Ingredient
          </button>
        </div>

        {error && (
          <div className="rounded-lg border border-red-500/40 bg-red-500/10 px-4 py-3 text-red-200">
            {error}
          </div>
        )}

        <IngredientList
          ingredients={pagedIngredients}
          isLoading={isLoading}
          searchTerm={searchTerm}
          onSearchChange={(value) => {
            setPage(1);
            setSearchTerm(value);
          }}
          onEdit={(ingredient) => {
            setModalMode('edit');
            setSelectedIngredient(ingredient);
            setModalOpen(true);
          }}
          onDelete={handleDelete}
          onView={(ingredient) => {
            setModalMode('view');
            setSelectedIngredient(ingredient);
            setModalOpen(true);
          }}
          page={page}
          pageSize={PAGE_SIZE}
          totalCount={ingredients.length}
          onPageChange={setPage}
        />
      </div>

      <IngredientModal
        isOpen={modalOpen}
        onClose={closeModal}
        onSubmit={handleSubmit}
        initialData={selectedIngredient}
        isLoading={isSubmitting}
        mode={modalMode}
      />
    </div>
  );
};
