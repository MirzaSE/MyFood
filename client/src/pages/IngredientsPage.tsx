import React, { useEffect, useState } from 'react';
import { Plus, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type {
  Ingredient,
  IngredientCreateDto,
  PaginationMetadata,
} from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [pagination, setPagination] = useState<PaginationMetadata | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [page, setPage] = useState(1);
  const pageCount = 10;

  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Ingredient | null>(null);

  const load = async (pg: number, q: string) => {
    try {
      setIsLoading(true);
      setError(null);
      // For search, use the dedicated search endpoint (case-insensitive).
      // For regular listing, use paginated GET.
      if (q.trim()) {
        const items = await ingredientService.search(q.trim());
        setIngredients(items);
        setPagination(null);
      } else {
        const { items, pagination: pg2 } = await ingredientService.getAll(pg, pageCount);
        setIngredients(items);
        setPagination(pg2);
      }
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  };

  // Debounced search; immediate page changes.
  useEffect(() => {
    const t = setTimeout(() => load(page, searchQuery), searchQuery ? 250 : 0);
    return () => clearTimeout(t);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, searchQuery]);

  const onSearchChange = (q: string) => {
    setSearchQuery(q);
    setPage(1);
  };

  const onPageChange = (newPage: number) => setPage(newPage);

  const handleCreateClick = () => {
    setEditing(null);
    setModalOpen(true);
  };

  const handleEditClick = (ing: Ingredient) => {
    setEditing(ing);
    setModalOpen(true);
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    if (editing) {
      const updated = await ingredientService.update(editing.id, data);
      setIngredients((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
    } else {
      const created = await ingredientService.create(data);
      setIngredients((prev) => [created, ...prev]);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.delete(id);
      setIngredients((prev) => prev.filter((i) => i.id !== id));
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to delete ingredient');
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
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-3">Ingredients</h1>
              <p className="text-gray-400">
                {pagination?.totalCount ?? ingredients.length}{' '}
                {(pagination?.totalCount ?? ingredients.length) === 1 ? 'item' : 'items'} in your catalog
              </p>
            </div>
            <button
              onClick={handleCreateClick}
              className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 shadow-lg hover:shadow-purple-500/50 font-semibold"
            >
              <Plus size={20} />
              <span>New Ingredient</span>
            </button>
          </div>
        </div>

        <IngredientList
          ingredients={ingredients}
          pagination={pagination}
          isLoading={isLoading}
          searchQuery={searchQuery}
          onSearchChange={onSearchChange}
          onPageChange={onPageChange}
          onEdit={handleEditClick}
          onDelete={handleDelete}
        />
      </div>

      <IngredientModal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          setEditing(null);
        }}
        onSubmit={handleSubmit}
        initialData={editing}
      />
    </div>
  );
};
