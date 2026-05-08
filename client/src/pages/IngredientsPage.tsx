import React, { useEffect, useMemo, useState } from 'react';
import { Plus, Search } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import { getApiErrorMessage } from '../services/apiError';
import type { Ingredient, IngredientCreateDto } from '../types';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(false);
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [pageCount] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalTitle, setModalTitle] = useState('');
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [statusMessage, setStatusMessage] = useState<string | null>(null);
  const [statusType, setStatusType] = useState<'success' | 'error' | null>(null);

  const fetchIngredients = async () => {
    setLoading(true);
    setError(null);

    try {
      const result = search.trim()
        ? await ingredientService.searchIngredients(search, { page, pageCount })
        : await ingredientService.getAllIngredients({ page, pageCount, query: search });

      setIngredients(result.items);
      setTotalPages(result.pagination.totalPages || 1);
    } catch (err) {
      setError(getApiErrorMessage(err, 'Failed to load ingredients'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void fetchIngredients();
  }, [page, search, pageCount]);

  const openCreateModal = () => {
    setSelectedIngredient(null);
    setModalTitle('New Ingredient');
    setStatusMessage(null);
    setStatusType(null);
    setModalOpen(true);
  };

  const openViewModal = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalTitle('View Ingredient');
    setStatusMessage(null);
    setStatusType(null);
    setModalOpen(true);
  };

  const openEditModal = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalTitle('Edit Ingredient');
    setStatusMessage(null);
    setStatusType(null);
    setModalOpen(true);
  };

  const handleCreate = async (dto: IngredientCreateDto) => {
    setLoading(true);

    try {
      await ingredientService.createIngredient(dto);
      setStatusType('success');
      setStatusMessage('Ingredient created successfully.');
      await fetchIngredients();
      window.setTimeout(() => setModalOpen(false), 600);
    } catch (err) {
      setStatusType('error');
      setStatusMessage(getApiErrorMessage(err, 'Failed to create ingredient'));
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = async (dto: IngredientCreateDto) => {
    if (!selectedIngredient) {
      return;
    }

    setLoading(true);

    try {
      await ingredientService.updateIngredient(selectedIngredient.id, dto);
      setStatusType('success');
      setStatusMessage('Ingredient updated successfully.');
      await fetchIngredients();
      window.setTimeout(() => setModalOpen(false), 600);
    } catch (err) {
      setStatusType('error');
      setStatusMessage(getApiErrorMessage(err, 'Failed to update ingredient'));
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (ingredient: Ingredient) => {
    if (!confirm(`Delete ${ingredient.name}?`)) {
      return;
    }

    setLoading(true);

    try {
      await ingredientService.deleteIngredient(ingredient.id);
      await fetchIngredients();
    } catch (err) {
      setError(getApiErrorMessage(err, 'Failed to delete ingredient'));
    } finally {
      setLoading(false);
    }
  };

  const paginationLabel = useMemo(() => `${page} / ${totalPages}`, [page, totalPages]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-10">
        <div className="mb-6 flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <h2 className="text-3xl font-bold text-white">Ingredients</h2>
            <p className="mt-2 text-gray-400">Manage ingredient nutrition and units.</p>
          </div>

          <button
            onClick={openCreateModal}
            className="inline-flex items-center gap-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-4 py-2 text-white shadow"
          >
            <Plus size={18} />
            New Ingredient
          </button>
        </div>

        <div className="mb-6 flex items-center gap-3 rounded-xl border border-white/10 bg-white/5 px-4 py-3">
          <Search className="text-gray-400" size={18} />
          <input
            value={search}
            onChange={(event) => {
              setSearch(event.target.value);
              setPage(1);
            }}
            placeholder="Search ingredients"
            className="w-full bg-transparent text-white outline-none placeholder:text-gray-500"
          />
        </div>

        {error && (
          <div className="mb-6 rounded-lg border border-red-500/40 bg-red-500/10 px-4 py-3 text-red-200">
            {error}
          </div>
        )}

        <IngredientList
          ingredients={ingredients}
          onView={openViewModal}
          onEdit={openEditModal}
          onDelete={handleDelete}
          isLoading={loading}
        />

        <div className="mt-6 flex items-center justify-between rounded-xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-gray-300">
          <button
            type="button"
            className="rounded-lg border border-white/10 px-3 py-2 disabled:opacity-40"
            onClick={() => setPage((current) => Math.max(1, current - 1))}
            disabled={page <= 1 || loading}
          >
            Previous
          </button>
          <span>Page {paginationLabel}</span>
          <button
            type="button"
            className="rounded-lg border border-white/10 px-3 py-2 disabled:opacity-40"
            onClick={() => setPage((current) => Math.min(totalPages, current + 1))}
            disabled={page >= totalPages || loading}
          >
            Next
          </button>
        </div>

        <IngredientModal
          isOpen={modalOpen}
          onClose={() => setModalOpen(false)}
          onSubmit={selectedIngredient ? handleEdit : handleCreate}
          initialData={selectedIngredient}
          title={modalTitle}
          isLoading={loading}
          readOnly={modalTitle === 'View Ingredient'}
          statusMessage={statusMessage}
          statusType={statusType}
        />
      </div>
    </div>
  );
};
