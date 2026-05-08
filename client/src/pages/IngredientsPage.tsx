import React, { useState, useEffect } from 'react';
import { Plus, Search, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [isSearching, setIsSearching] = useState(false);

  useEffect(() => {
    loadIngredients();
  }, []);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await ingredientService.getAllIngredients();
      setIngredients(data);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to load ingredients';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearch = async (query: string) => {
    setSearchQuery(query);

    if (!query.trim()) {
      loadIngredients();
      return;
    }

    try {
      setIsSearching(true);
      setError(null);
      const results = await ingredientService.searchIngredients(query.trim());
      setIngredients(results);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Search failed';
      setError(message);
    } finally {
      setIsSearching(false);
    }
  };

  const handleCreateClick = () => {
    setSelectedIngredient(null);
    setModalOpen(true);
  };

  const handleEditClick = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalOpen(true);
  };

  const handleModalSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setError(null);

      if (selectedIngredient) {
        const updated = await ingredientService.updateIngredient(selectedIngredient.id, data);
        setIngredients(prev =>
          prev.map(i => (i.id === selectedIngredient.id ? updated : i))
        );
      } else {
        const created = await ingredientService.createIngredient(data);
        setIngredients(prev => [...prev, created]);
      }

      setModalOpen(false);
      setSelectedIngredient(null);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to save ingredient';
      setError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      setIngredients(prev => prev.filter(i => i.id !== id));
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to delete ingredient';
      setError(message);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {/* Error Banner */}
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        {/* Page Header */}
        <div className="mb-10">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-3">
                Ingredient Management
              </h1>
              <p className="text-gray-400">
                {ingredients.length} {ingredients.length === 1 ? 'ingredient' : 'ingredients'} in your collection
              </p>
            </div>
            <button
              onClick={handleCreateClick}
              disabled={isLoading || isSubmitting}
              className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-purple-500/50 font-semibold"
            >
              <Plus size={20} />
              <span>New Ingredient</span>
            </button>
          </div>
        </div>

        {/* Search Bar */}
        <div className="mb-8">
          <div className="relative max-w-md">
            <Search
              size={18}
              className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none"
            />
            <input
              type="text"
              value={searchQuery}
              onChange={e => handleSearch(e.target.value)}
              placeholder="Search ingredients..."
              className="w-full pl-10 pr-4 py-2.5 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
            />
            {isSearching && (
              <div className="absolute right-3 top-1/2 -translate-y-1/2">
                <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"></div>
              </div>
            )}
          </div>
        </div>

        {/* Ingredient List */}
        <IngredientList
          ingredients={ingredients}
          onEdit={handleEditClick}
          onDelete={handleDelete}
          isLoading={isLoading}
        />
      </div>

      {/* Modal */}
      <IngredientModal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          setSelectedIngredient(null);
        }}
        onSubmit={handleModalSubmit}
        initialData={selectedIngredient}
        isLoading={isSubmitting}
      />
    </div>
  );
};
