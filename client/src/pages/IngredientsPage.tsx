import React, { useEffect, useMemo, useState } from 'react';
import { Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import { extractApiErrorMessage } from '../services/api';
import type { Ingredient, IngredientFormValues } from '../types/ingredient';

const PAGE_SIZE = 8;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [page, setPage] = useState<number>(1);
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [modalMode, setModalMode] = useState<'create' | 'edit' | 'view'>('create');
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);

  useEffect(() => {
    const loadIngredients = async (): Promise<void> => {
      try {
        setIsLoading(true);
        setError(null);

        const data = searchTerm.trim()
          ? await ingredientService.searchIngredients(searchTerm)
          : await ingredientService.getAllIngredients();

        setIngredients(data);
      } catch (loadError: unknown) {
      const message = extractApiErrorMessage(loadError);

      setError(message);
      } finally {
        setIsLoading(false);
      }
    };

    void loadIngredients();
  }, [searchTerm]);

  const totalPages = useMemo<number>(() => {
    return Math.max(1, Math.ceil(ingredients.length / PAGE_SIZE));
  }, [ingredients]);

  const currentPage = useMemo<number>(() => {
    return Math.min(page, totalPages);
  }, [page, totalPages]);

  const pagedIngredients = useMemo<Ingredient[]>(() => {
    const start = (currentPage - 1) * PAGE_SIZE;

    return ingredients.slice(start, start + PAGE_SIZE);
  }, [ingredients, currentPage]);

  const closeModal = (): void => {
    setModalOpen(false);
    setSelectedIngredient(null);
  };

  const handleCreate = (): void => {
    setModalMode('create');
    setSelectedIngredient(null);
    setModalOpen(true);
  };

  const handleSubmit = async (
    values: IngredientFormValues
  ): Promise<void> => {
    try {
      setIsSubmitting(true);
      setError(null);

      if (modalMode === 'edit' && selectedIngredient) {
        const updated = await ingredientService.updateIngredient(
          selectedIngredient.id,
          values
        );

        setIngredients((current) =>
          current.map((ingredient) =>
            ingredient.id === updated.id ? updated : ingredient
          )
        );
      } else {
        const created = await ingredientService.createIngredient(values);

        setIngredients((current) => [created, ...current]);
      }

      closeModal();
    } catch (submitError: unknown) {
      const message = extractApiErrorMessage(submitError);

      setError(message);

      throw new Error(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number): Promise<void> => {
    try {
      setError(null);

      await ingredientService.deleteIngredient(id);

      setIngredients((current) =>
        current.filter((ingredient) => ingredient.id !== id)
      );
    } catch (deleteError: unknown) {
      const message = extractApiErrorMessage(deleteError);

      setError(message);

      throw new Error(message);
    }
  };

  const handleSearchChange = (value: string): void => {
    setPage(1);
    setSearchTerm(value);
  };

  const handleEdit = (ingredient: Ingredient): void => {
    setModalMode('edit');
    setSelectedIngredient(ingredient);
    setModalOpen(true);
  };

  const handleView = (ingredient: Ingredient): void => {
    setModalMode('view');
    setSelectedIngredient(ingredient);
    setModalOpen(true);
  };

  const handlePageChange = (nextPage: number): void => {
    setPage(Math.min(nextPage, totalPages));
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto space-y-8 px-6 py-12">
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <div>
            <h1 className="text-4xl font-bold text-white">
              Ingredients
            </h1>

            <p className="mt-2 text-gray-400">
              Manage ingredient data, nutrition, and quick actions.
            </p>
          </div>

          <button
            onClick={handleCreate}
            className="inline-flex items-center gap-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-5 py-3 font-semibold text-white"
          >
            <Plus size={18} />
            New Ingredient
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
          onSearchChange={handleSearchChange}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onView={handleView}
          page={currentPage}
          pageSize={PAGE_SIZE}
          totalCount={ingredients.length}
          onPageChange={handlePageChange}
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
