import React, { useState, useEffect, useCallback } from 'react';
import { Plus, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';

const PAGE_SIZE = 10;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [viewIngredient, setViewIngredient] = useState<Ingredient | null>(null);

  const loadIngredients = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      if (searchQuery.trim()) {
        const data = await ingredientService.searchIngredients(searchQuery.trim());
        setIngredients(data);
        setTotalCount(data.length);
      } else {
        const data = await ingredientService.getAllIngredients(page, PAGE_SIZE);
        setIngredients(data);
        setTotalCount(data.length >= PAGE_SIZE ? page * PAGE_SIZE + 1 : (page - 1) * PAGE_SIZE + data.length);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  }, [page, searchQuery]);

  useEffect(() => {
    const timer = setTimeout(loadIngredients, searchQuery ? 300 : 0);
    return () => clearTimeout(timer);
  }, [loadIngredients, searchQuery]);

  const handleSearchChange = (query: string) => {
    setSearchQuery(query);
    setPage(1);
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      setIngredients(prev => prev.filter(i => i.id !== id));
      setTotalCount(prev => prev - 1);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete ingredient');
      throw err;
    }
  };

  const handleModalSuccess = (ingredient: Ingredient) => {
    if (selectedIngredient) {
      setIngredients(prev => prev.map(i => i.id === ingredient.id ? ingredient : i));
    } else {
      setIngredients(prev => [ingredient, ...prev]);
      setTotalCount(prev => prev + 1);
    }
    setModalOpen(false);
    setSelectedIngredient(null);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <div className="mb-10">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
            <div>
              <h1 className="text-4xl font-bold text-white mb-2">Ingredients</h1>
              <p className="text-gray-400">{totalCount} ingredient{totalCount !== 1 ? 's' : ''} in your library</p>
            </div>
            <button
              onClick={() => { setSelectedIngredient(null); setModalOpen(true); }}
              className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 shadow-lg hover:shadow-purple-500/50 font-semibold"
            >
              <Plus size={20} />
              <span>New Ingredient</span>
            </button>
          </div>
        </div>

        <IngredientList
          ingredients={ingredients}
          totalCount={totalCount}
          page={page}
          pageCount={PAGE_SIZE}
          searchQuery={searchQuery}
          isLoading={isLoading}
          onEdit={ingredient => { setSelectedIngredient(ingredient); setModalOpen(true); }}
          onDelete={handleDelete}
          onView={setViewIngredient}
          onPageChange={setPage}
          onSearchChange={handleSearchChange}
        />
      </div>

      <IngredientModal
        isOpen={modalOpen}
        onClose={() => { setModalOpen(false); setSelectedIngredient(null); }}
        onSuccess={handleModalSuccess}
        initialData={selectedIngredient}
      />

      {viewIngredient && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4"
          onClick={() => setViewIngredient(null)}>
          <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-sm w-full p-6"
            onClick={e => e.stopPropagation()}>
            <h3 className="text-xl font-bold text-white mb-4">{viewIngredient.name}</h3>
            <div className="space-y-2 text-sm">
              {[
                ['Unit', viewIngredient.unit],
                ['Calories/Unit', `${viewIngredient.caloriesPerUnit} kcal`],
                ['Protein', `${viewIngredient.protein}g`],
                ['Carbs', `${viewIngredient.carbs}g`],
                ['Fat', `${viewIngredient.fat}g`],
              ].map(([label, value]) => (
                <div key={label} className="flex justify-between text-gray-300">
                  <span className="text-gray-400">{label}</span>
                  <span>{value}</span>
                </div>
              ))}
            </div>
            <button
              onClick={() => setViewIngredient(null)}
              className="mt-5 w-full py-2 border border-white/20 rounded-lg text-gray-300 hover:text-white hover:border-white/40 transition-all"
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
