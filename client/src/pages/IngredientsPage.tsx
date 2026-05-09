import React, { useState, useEffect, useCallback } from 'react';
import { ingredientService } from '../services/ingredientService';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingIngredient, setEditingIngredient] = useState<Ingredient | null>(null);
  const [viewingIngredient, setViewingIngredient] = useState<Ingredient | null>(null);

  const fetchIngredients = useCallback(async () => {
    setIsLoading(true);
    try {
      if (searchQuery) {
        const result = await ingredientService.search(searchQuery, currentPage, 10);
        setIngredients(result.items);
        setTotalPages(result.totalPages);
      } else {
        const result = await ingredientService.getAll(currentPage, 10);
        setIngredients(result.items);
        setTotalPages(result.totalPages);
      }
    } catch (error) {
      console.error('Failed to fetch ingredients:', error);
    } finally {
      setIsLoading(false);
    }
  }, [currentPage, searchQuery]);

  useEffect(() => {
    fetchIngredients();
  }, [fetchIngredients]);

  const handleCreateOrUpdate = async (data: IngredientCreateDto) => {
    if (editingIngredient) {
      await ingredientService.update(editingIngredient.id, data);
    } else {
      await ingredientService.create(data);
    }
    fetchIngredients();
  };

  const handleDelete = async (ingredient: Ingredient) => {
    if (window.confirm(`Are you sure you want to delete "${ingredient.name}"?`)) {
      try {
        await ingredientService.delete(ingredient.id);
        fetchIngredients();
      } catch (error) {
        console.error('Failed to delete ingredient:', error);
        alert('Failed to delete ingredient');
      }
    }
  };

  const handleEdit = (ingredient: Ingredient) => {
    setEditingIngredient(ingredient);
    setViewingIngredient(null);
    setIsModalOpen(true);
  };

  const handleView = (ingredient: Ingredient) => {
    setViewingIngredient(ingredient);
    setEditingIngredient(null);
    setIsModalOpen(true);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handleSearchChange = (query: string) => {
    setSearchQuery(query);
    setCurrentPage(1);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingIngredient(null);
    setViewingIngredient(null);
  };

  return (
    <div className="min-h-screen bg-slate-900 p-6">
      <div className="max-w-6xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-2xl font-bold text-white">Ingredients</h1>
          <button
            onClick={() => {
              setEditingIngredient(null);
              setViewingIngredient(null);
              setIsModalOpen(true);
            }}
            className="px-4 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg hover:from-purple-700 hover:to-blue-700 transition-all"
          >
            + New Ingredient
          </button>
        </div>

        <IngredientList
          ingredients={ingredients}
          isLoading={isLoading}
          currentPage={currentPage}
          totalPages={totalPages}
          searchQuery={searchQuery}
          onSearchChange={handleSearchChange}
          onPageChange={handlePageChange}
          onEdit={handleEdit}
          onDelete={handleDelete}
          onView={handleView}
        />

        {isModalOpen && (
          viewingIngredient ? (
            <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
              <div className="w-full max-w-lg bg-slate-800 rounded-xl border border-white/10 shadow-2xl p-6">
                <h2 className="text-xl font-bold text-white mb-4">{viewingIngredient.name}</h2>
                <div className="space-y-2 text-gray-300">
                  <p><span className="text-gray-400">Unit:</span> {viewingIngredient.unit}</p>
                  <p><span className="text-gray-400">Calories per {viewingIngredient.unit}:</span> {viewingIngredient.caloriesPerUnit}</p>
                  <p><span className="text-gray-400">Protein:</span> {viewingIngredient.protein}g</p>
                  <p><span className="text-gray-400">Carbs:</span> {viewingIngredient.carbs}g</p>
                  <p><span className="text-gray-400">Fat:</span> {viewingIngredient.fat}g</p>
                </div>
                <button
                  onClick={handleCloseModal}
                  className="mt-4 w-full px-4 py-2 rounded-lg bg-slate-700 text-white hover:bg-slate-600 transition-colors"
                >
                  Close
                </button>
              </div>
            </div>
          ) : (
            <IngredientModal
              isOpen={isModalOpen}
              ingredient={editingIngredient}
              onClose={handleCloseModal}
              onSubmit={handleCreateOrUpdate}
            />
          )
        )}
      </div>
    </div>
  );
};