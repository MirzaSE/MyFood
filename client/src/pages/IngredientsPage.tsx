import React, { useState, useEffect, useCallback } from 'react';
import { Plus, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/ingredients/IngredientList';
import { IngredientModal } from '../components/ingredients/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import { formatApiError } from '../utils/formatApiError';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

const PAGE_SIZE = 10;

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [searchMode, setSearchMode] = useState(false);

  const loadIngredients = useCallback(async (page = 1) => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await ingredientService.getAll(page, PAGE_SIZE);
      setIngredients(data);
      if (data.length < PAGE_SIZE) {
        setTotalPages(page);
      } else {
        setTotalPages(page + 1);
      }
      setCurrentPage(page);
      setSearchMode(false);
    } catch (err: any) {
      setError(formatApiError(err) || 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  }, []);

  const handleSearch = useCallback(async (query: string) => {
    if (!query.trim()) {
      loadIngredients(1);
      return;
    }
    try {
      setIsLoading(true);
      setError(null);
      const data = await ingredientService.search(query.trim());
      setIngredients(data);
      setSearchMode(true);
      setTotalPages(1);
      setCurrentPage(1);
    } catch (err: any) {
      setError(formatApiError(err) || 'Failed to search ingredients');
    } finally {
      setIsLoading(false);
    }
  }, [loadIngredients]);

  useEffect(() => {
    loadIngredients(1);
  }, [loadIngredients]);

  const handleCreateClick = () => {
    setSelectedIngredient(null);
    setModalOpen(true);
    setSuccessMessage(null);
  };

  const handleEditClick = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalOpen(true);
    setSuccessMessage(null);
  };

  const handleModalSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setError(null);

      if (selectedIngredient) {
        const updateData: IngredientUpdateDto = {
          name: data.name,
          unit: data.unit,
          caloriesPerUnit: data.caloriesPerUnit,
          protein: data.protein,
          carbs: data.carbs,
          fat: data.fat,
        };
        const updated = await ingredientService.update(selectedIngredient.id, updateData);
        setIngredients((prev) =>
          prev.map((i) => (i.id === selectedIngredient.id ? updated : i))
        );
        setSuccessMessage('Ingredient updated successfully!');
      } else {
        const newIngredient = await ingredientService.create(data);
        setIngredients((prev) => [...prev, newIngredient]);
        setSuccessMessage('Ingredient created successfully!');
      }

      setModalOpen(false);
      setSelectedIngredient(null);
    } catch (err: any) {
      setError(formatApiError(err) || 'Failed to save ingredient');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.delete(id);
      setIngredients((prev) => prev.filter((i) => i.id !== id));
      setSuccessMessage('Ingredient deleted.');
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err: any) {
      setError(formatApiError(err) || 'Failed to delete ingredient');
      throw err;
    }
  };

  const handlePageChange = (page: number) => {
    loadIngredients(page);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {/* Error Banner */}
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <div className="flex-1">
              <p className="text-red-200 whitespace-pre-line">{error}</p>
            </div>
            <button
              onClick={() => setError(null)}
              className="text-red-400 hover:text-red-200 transition-colors"
            >
              ×
            </button>
          </div>
        )}

        {/* Success Banner */}
        {successMessage && (
          <div className="mb-6 p-4 bg-green-500/20 border border-green-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <span className="text-green-200">{successMessage}</span>
            <button
              onClick={() => setSuccessMessage(null)}
              className="text-green-400 hover:text-green-200 transition-colors ml-auto"
            >
              ×
            </button>
          </div>
        )}

        {/* Page Header */}
        <div className="mb-16">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-10">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-4">
                Ingredients
              </h1>
              <p className="text-gray-400">
                {ingredients.length} {ingredients.length === 1 ? 'ingredient' : 'ingredients'}
                {searchMode && ' (search results)'}
              </p>
            </div>
            <button
              onClick={handleCreateClick}
              disabled={isLoading || isSubmitting}
              className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-purple-500/50 font-semibold"
            >
              <Plus size={20} />
              <span>Add Ingredient</span>
            </button>
          </div>
        </div>

        {/* Content */}
        <IngredientList
          ingredients={ingredients}
          onEdit={handleEditClick}
          onDelete={handleDelete}
          onSearch={handleSearch}
          isLoading={isLoading}
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={searchMode ? undefined : handlePageChange}
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
