import React, { useState, useEffect } from 'react';
import { Plus, AlertCircle } from 'lucide-react';
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

  // Load ingredients on component mount
  useEffect(() => {
    loadIngredients();
  }, []);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const response = await ingredientService.getAllIngredients(1, 100);
// Filter out null values
const filteredData = (response.value || []).filter((ing: Ingredient | null) => ing !== null);
setIngredients(filteredData);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
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
        // Update existing ingredient
        const updatedIngredient = await ingredientService.updateIngredient(
          selectedIngredient.id,
          data
        );
        setIngredients(
          ingredients.map((ing) =>
            ing.id === selectedIngredient.id ? updatedIngredient : ing
          )
        );
      } else {
        // Create new ingredient
        const newIngredient = await ingredientService.createIngredient(data);
        setIngredients([...ingredients, newIngredient]);
      }

      setModalOpen(false);
      setSelectedIngredient(null);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to save ingredient');
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      setIngredients(ingredients.filter((ing) => ing.id !== id));
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete ingredient');
      throw err;
    }
  };

  const handleSearch = async (query: string) => {
    try {
      setError(null);
      if (query.trim()) {
        const response = await ingredientService.searchIngredients(query);
        setIngredients(response.value);
      } else {
        await loadIngredients();
      }
    } catch (err: any) {
      setError('Search failed');
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
        <div className="mb-16">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-10">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-4">
                Ingredient Management
              </h1>
              <p className="text-gray-400">
                Manage your food ingredients and nutrition information
              </p>
            </div>
            <button
              onClick={handleCreateClick}
              className="flex items-center justify-center space-x-2 px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition"
            >
              <Plus size={20} />
              <span>New Ingredient</span>
            </button>
          </div>
        </div>

        {/* Content */}
        <div className="bg-gray-800/50 backdrop-blur rounded-lg p-8 border border-gray-700">
          <IngredientList
            ingredients={ingredients}
            isLoading={isLoading}
            onEdit={handleEditClick}
            onDelete={handleDelete}
            onSearch={handleSearch}
          />
        </div>
      </div>

      {/* Ingredient Modal */}
      <IngredientModal
        isOpen={modalOpen}
        ingredient={selectedIngredient}
        onClose={() => {
          setModalOpen(false);
          setSelectedIngredient(null);
        }}
        onSubmit={handleModalSubmit}
        isSubmitting={isSubmitting}
      />
    </div>
  );
};
