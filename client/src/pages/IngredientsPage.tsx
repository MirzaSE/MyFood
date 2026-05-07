import React, { useCallback, useEffect, useState } from 'react';
import { Plus, Search, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientFormValues } from '../types/ingredient';
import type { IngredientFormMode } from '../components/IngredientForm';

const PAGE_SIZE = 8;

export const IngredientsPage: React.FC = () => {
  const [items, setItems] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [page, setPage] = useState(1);

  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<IngredientFormMode>('create');
  const [selected, setSelected] = useState<Ingredient | null>(null);

  const load = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const q = searchQuery.trim();
      const data = q ? await ingredientService.search(q) : await ingredientService.getAll();
      setItems(data);
      setPage(1);
    } catch (err: unknown) {
      const msg =
        typeof err === 'object' && err !== null && 'response' in err
          ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
          : undefined;
      setError(msg || 'Failed to load ingredients.');
    } finally {
      setIsLoading(false);
    }
  }, [searchQuery]);

  useEffect(() => {
    const t = setTimeout(() => {
      void load();
    }, 300);
    return () => clearTimeout(t);
  }, [load]);

  const openCreate = () => {
    setSelected(null);
    setModalMode('create');
    setModalOpen(true);
  };

  const openEdit = (row: Ingredient) => {
    setSelected(row);
    setModalMode('edit');
    setModalOpen(true);
  };

  const openView = (row: Ingredient) => {
    setSelected(row);
    setModalMode('view');
    setModalOpen(true);
  };

  const handleModalSave = async (data: IngredientFormValues) => {
    if (modalMode === 'view') return;
    if (modalMode === 'edit' && selected) {
      await ingredientService.update(selected.id, data);
    } else {
      await ingredientService.create(data);
    }
    await load();
  };

  const handleDelete = async (row: Ingredient) => {
    if (!window.confirm(`Delete "${row.name}"?`)) return;
    try {
      setError(null);
      await ingredientService.delete(row.id);
      await load();
    } catch {
      setError('Could not delete ingredient.');
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-12">
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start gap-3">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6 mb-10">
          <div>
            <h1 className="text-4xl font-bold text-white mb-2">Ingredients</h1>
            <p className="text-gray-400">Manage your ingredient catalog</p>
          </div>
          <button
            type="button"
            onClick={openCreate}
            className="inline-flex items-center justify-center gap-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg font-semibold shadow-lg"
          >
            <Plus size={20} />
            New ingredient
          </button>
        </div>

        <div className="mb-6 relative">
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500" size={20} />
          <input
            type="search"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search by name…"
            className="w-full pl-12 pr-4 py-3 bg-white/10 border border-white/20 rounded-xl text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-purple-500/40"
          />
        </div>

        <IngredientList
          ingredients={items}
          isLoading={isLoading}
          searchQuery={searchQuery}
          page={page}
          pageSize={PAGE_SIZE}
          onEdit={openEdit}
          onDelete={handleDelete}
          onView={openView}
          onPageChange={setPage}
        />
      </div>

      <IngredientModal
        isOpen={modalOpen}
        mode={modalMode}
        ingredient={selected}
        onClose={() => setModalOpen(false)}
        onSave={handleModalSave}
      />
    </div>
  );
};

export default IngredientsPage;
