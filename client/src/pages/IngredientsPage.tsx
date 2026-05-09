import React, { useEffect, useMemo, useState } from 'react';
import { AlertCircle, Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [viewIngredient, setViewIngredient] = useState<Ingredient | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);

  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [error, setError] = useState<string | null>(null);
  const [modalError, setModalError] = useState<string | null>(null);
  const [modalMessage, setModalMessage] = useState<string | null>(null);

  const pageSize = 5;

  useEffect(() => {
    loadIngredients();
  }, []);

  const loadIngredients = async () => {
    try {
      setIsLoading(true);
      setError(null);

      const data = await ingredientService.getAllIngredients();
      setIngredients(data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  };

  const filteredIngredients = useMemo(() => {
    return ingredients.filter((ingredient) =>
      ingredient.name.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [ingredients, searchTerm]);

  const handleCreateClick = () => {
    setSelectedIngredient(null);
    setModalError(null);
    setModalMessage(null);
    setModalOpen(true);
  };

  const handleEditClick = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalError(null);
    setModalMessage(null);
    setModalOpen(true);
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setModalError(null);
      setModalMessage(null);

      if (selectedIngredient) {
        const updated = await ingredientService.updateIngredient(selectedIngredient.id, data);
        setIngredients((previous) =>
          previous.map((ingredient) =>
            ingredient.id === selectedIngredient.id ? updated : ingredient
          )
        );
        setModalMessage('Ingredient updated successfully.');
      } else {
        const created = await ingredientService.createIngredient(data);
        setIngredients((previous) => [...previous, created]);
        setModalMessage('Ingredient created successfully.');
      }

      setModalOpen(false);
      setSelectedIngredient(null);
    } catch (err: any) {
      setModalError(err.response?.data?.message || 'Failed to save ingredient');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      setIngredients((previous) => previous.filter((ingredient) => ingredient.id !== id));
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete ingredient');
    }
  };

  const handleSearchChange = (value: string) => {
    setSearchTerm(value);
    setCurrentPage(1);
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

        <div className="mb-12 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-8">
          <div>
            <h1 className="text-4xl sm:text-5xl font-bold text-white mb-4">
              Ingredient Management
            </h1>
            <p className="text-gray-400">
              {ingredients.length} {ingredients.length === 1 ? 'ingredient' : 'ingredients'} in your collection
            </p>
          </div>

          <button
            onClick={handleCreateClick}
            disabled={isLoading || isSubmitting}
            className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all disabled:opacity-50 shadow-lg font-semibold"
          >
            <Plus size={20} />
            <span>New Ingredient</span>
          </button>
        </div>

        <IngredientList
          ingredients={filteredIngredients}
          searchTerm={searchTerm}
          currentPage={currentPage}
          pageSize={pageSize}
          isLoading={isLoading}
          onSearchChange={handleSearchChange}
          onPageChange={setCurrentPage}
          onEdit={handleEditClick}
          onDelete={handleDelete}
          onView={setViewIngredient}
        />
      </div>

      <IngredientModal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          setSelectedIngredient(null);
        }}
        onSubmit={handleSubmit}
        initialData={selectedIngredient}
        isLoading={isSubmitting}
        message={modalMessage}
        error={modalError}
      />

      {viewIngredient && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-md w-full p-8">
            <h2 className="text-2xl font-bold text-white mb-4">
              {viewIngredient.name}
            </h2>

            <div className="space-y-3 text-gray-300">
              <p>Unit: {viewIngredient.unit}</p>
              <p>Calories: {viewIngredient.caloriesPerUnit} kcal</p>
              <p>Protein: {viewIngredient.protein} g</p>
              <p>Carbs: {viewIngredient.carbs} g</p>
              <p>Fat: {viewIngredient.fat} g</p>
            </div>

            <button
              onClick={() => setViewIngredient(null)}
              className="mt-6 w-full px-4 py-3 bg-purple-600 hover:bg-purple-700 text-white rounded-lg"
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
};