import React, { useState, useEffect, useCallback } from 'react';
import { Plus, Search, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

const PAGE_SIZE = 10;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selected, setSelected] = useState<Ingredient | null>(null);

  const load = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const { items, total: t } = await ingredientService.getAllIngredients(page, PAGE_SIZE);
      const filtered = search.trim()
        ? items.filter((i) => i.name?.toLowerCase().includes(search.toLowerCase()))
        : items;
      setIngredients(filtered);
      setTotal(t);
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  }, [page, search]);

  useEffect(() => { load(); }, [load]);

  const handleCreate = () => { setSelected(null); setModalOpen(true); };
  const handleEdit = (ing: Ingredient) => { setSelected(ing); setModalOpen(true); };

  const handleSubmit = async (data: IngredientCreateDto) => {
    setIsSubmitting(true);
    try {
      if (selected) {
        const updated = await ingredientService.updateIngredient(selected.id, data);
        setIngredients((prev) => prev.map((i) => (i.id === selected.id ? updated : i)));
      } else {
        const created = await ingredientService.createIngredient(data);
        setIngredients((prev) => [created, ...prev]);
        setTotal((t) => t + 1);
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      setIngredients((prev) => prev.filter((i) => i.id !== id));
      setTotal((t) => t - 1);
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Failed to delete ingredient');
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {/* Error banner */}
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        {/* Header */}
        <div className="mb-10">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-2">Ingredients</h1>
              <p className="text-gray-400">{total} {total === 1 ? 'item' : 'items'} in your library</p>
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

          {/* Search */}
          <div className="mt-6 relative max-w-md">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1); }}
              placeholder="Search ingredients…"
              className="w-full pl-9 pr-4 py-2.5 bg-white/5 border border-white/10 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500 transition"
            />
          </div>
        </div>

        {/* List */}
        <IngredientList
          ingredients={ingredients}
          total={total}
          page={page}
          pageCount={PAGE_SIZE}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onPageChange={setPage}
          isLoading={isLoading}
        />
      </div>

      <IngredientModal
        isOpen={modalOpen}
        onClose={() => { setModalOpen(false); setSelected(null); }}
        onSubmit={handleSubmit}
        initialData={selected}
        isLoading={isSubmitting}
      />
    </div>
  );
};
