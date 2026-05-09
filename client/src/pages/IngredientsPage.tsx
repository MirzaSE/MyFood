import React, { useState, useEffect } from 'react';
import { Plus, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [filteredIngredients, setFilteredIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    loadIngredients();
  }, []);

  useEffect(() => {
    if (searchQuery.trim()) {
      setFilteredIngredients(
        ingredients.filter((i) =>
          i.name.toLowerCase().includes(searchQuery.toLowerCase())
        )
      );
    } else {
      setFilteredIngredients(ingredients);
    }
  }, [searchQuery, ingredients]);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await ingredientService.getAllIngredients();
      setIngredients(data);
      setFilteredIngredients(data);
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
        await ingredientService.updateIngredient(selectedIngredient.id, data);
        setIngredients(
          ingredients.map((i) =>
            i.id === selectedIngredient.id ? { ...i, ...data } : i
          )
        );
      } else {
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
      setIngredients(ingredients.filter((i) => i.id !== id));
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete ingredient');
      throw err;
    }
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

        <div className="mb-16">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-10">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-4">
                Ingredients
              </h1>
              <p className="text-gray-400">
                {ingredients.length} {ingredients.length === 1 ? 'ingredient' : 'ingredients'} in
                your collection
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

        <IngredientList
          ingredients={filteredIngredients}
          onEdit={handleEditClick}
          onDelete={handleDelete}
          searchQuery={searchQuery}
          onSearchChange={setSearchQuery}
          isLoading={isLoading}
        />
      </div>

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
