import React, { useEffect, useState } from 'react';
import { Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { extractApiErrorMessage } from '../services/api';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageCount] = useState(6);
  const [search, setSearch] = useState('');
  const [totalCount, setTotalCount] = useState(0);

  useEffect(() => {
    void loadIngredients();
  }, [page, search]);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const result = await ingredientService.getAllIngredients({ page, pageCount, query: search });
      setIngredients(result.items);
      setTotalCount(result.totalCount);
    } catch (err: any) {
      setError(extractApiErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setError(null);
      setMessage(null);

      if (selectedIngredient) {
        await ingredientService.updateIngredient(selectedIngredient.id, data);
        setMessage('Ingredient updated successfully.');
      } else {
        await ingredientService.createIngredient(data);
        setMessage('Ingredient created successfully.');
      }

      setIsModalOpen(false);
      setSelectedIngredient(null);
      await loadIngredients();
    } catch (err: any) {
      setError(extractApiErrorMessage(err));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      setMessage(null);
      await ingredientService.deleteIngredient(id);
      setMessage('Ingredient deleted successfully.');
      await loadIngredients();
    } catch (err: any) {
      setError(extractApiErrorMessage(err));
      throw err;
    }
  };

  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top,_rgba(245,158,11,0.18),_transparent_38%),linear-gradient(135deg,#0f172a_0%,#1c1917_48%,#111827_100%)]">
      <Navbar />
      <div className="container mx-auto px-6 py-14">
        <div className="mb-10 flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="mb-3 text-sm uppercase tracking-[0.3em] text-amber-300">Ingredient Library</p>
            <h1 className="text-4xl font-black text-white">Manage reusable ingredients</h1>
            <p className="mt-3 max-w-2xl text-gray-400">Create a clean catalog for quantities, calories, and macros before using ingredients inside foods.</p>
          </div>
          <button
            onClick={() => {
              setSelectedIngredient(null);
              setMessage(null);
              setIsModalOpen(true);
            }}
            className="inline-flex items-center justify-center gap-2 rounded-xl bg-amber-500 px-5 py-3 font-semibold text-slate-900 transition hover:bg-amber-400"
          >
            <Plus size={18} />
            New Ingredient
          </button>
        </div>

        {error && <div className="mb-6 rounded-xl border border-red-500/30 bg-red-500/10 p-4 text-red-100">{error}</div>}
        {message && <div className="mb-6 rounded-xl border border-emerald-500/30 bg-emerald-500/10 p-4 text-emerald-100">{message}</div>}

        <IngredientList
          ingredients={ingredients}
          totalCount={totalCount}
          page={page}
          pageCount={pageCount}
          search={search}
          isLoading={isLoading}
          onSearchChange={(value) => {
            setSearch(value);
            setPage(1);
          }}
          onPageChange={setPage}
          onEdit={(ingredient) => {
            setSelectedIngredient(ingredient);
            setMessage(null);
            setIsModalOpen(true);
          }}
          onDelete={handleDelete}
        />
      </div>

      <IngredientModal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setSelectedIngredient(null);
        }}
        onSubmit={handleSubmit}
        initialData={selectedIngredient}
        isLoading={isSubmitting}
        error={error}
        message={message}
      />
    </div>
  );
};
