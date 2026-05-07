import React from 'react';
import { AlertCircle, Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { foodService } from '../services/foodService';
import { ingredientService } from '../services/ingredientService';
import type { Food, Ingredient, IngredientFormData } from '../types';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = React.useState<Ingredient[]>([]);
  const [foods, setFoods] = React.useState<Food[]>([]);
  const [selectedIngredient, setSelectedIngredient] = React.useState<Ingredient | null>(null);
  const [viewIngredient, setViewIngredient] = React.useState<Ingredient | null>(null);
  const [isModalOpen, setIsModalOpen] = React.useState(false);
  const [isLoading, setIsLoading] = React.useState(true);
  const [isSubmitting, setIsSubmitting] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);
  const [search, setSearch] = React.useState('');
  const [currentPage, setCurrentPage] = React.useState(1);
  const [totalPages, setTotalPages] = React.useState(1);

  const defaultFoodId = foods[0]?.id;

  const loadData = React.useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      const [ingredientResponse, foodItems] = await Promise.all([
        ingredientService.getAllIngredients({
          page: currentPage,
          pageCount: 10,
          query: search,
        }),
        foodService.getAllFoods(),
      ]);
      setIngredients(ingredientResponse.items);
      setTotalPages(ingredientResponse.totalPages);
      setFoods(foodItems);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to load ingredients.');
    } finally {
      setIsLoading(false);
    }
  }, [currentPage, search]);

  React.useEffect(() => {
    loadData();
  }, [loadData]);

  const openNewModal = () => {
    setSelectedIngredient(null);
    setIsModalOpen(true);
  };

  const openEditModal = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setIsModalOpen(true);
  };

  const handleSubmit = async (formData: IngredientFormData) => {
    if (!defaultFoodId && !selectedIngredient?.foodEntityId) {
      throw new Error('No foods found. Create a food item first.');
    }

    try {
      setIsSubmitting(true);
      const foodEntityId = selectedIngredient?.foodEntityId ?? defaultFoodId!;

      if (selectedIngredient) {
        await ingredientService.updateIngredient(selectedIngredient.id, formData, foodEntityId);
      } else {
        await ingredientService.createIngredient(formData, foodEntityId);
      }

      await loadData();
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (ingredient: Ingredient) => {
    await ingredientService.deleteIngredient(ingredient.id);
    await loadData();
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

        <div className="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 className="text-4xl font-bold text-white">Ingredients</h1>
            <p className="text-gray-400 mt-1">Manage ingredient definitions and nutritional values.</p>
          </div>
          <button
            onClick={openNewModal}
            className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 shadow-lg hover:shadow-purple-500/50 font-semibold"
          >
            <Plus size={20} />
            <span>New Ingredient</span>
          </button>
        </div>

        <IngredientList
          ingredients={ingredients}
          isLoading={isLoading}
          search={search}
          onSearchChange={(value) => {
            setSearch(value);
            setCurrentPage(1);
          }}
          onEdit={openEditModal}
          onDelete={handleDelete}
          onView={setViewIngredient}
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={setCurrentPage}
        />
      </div>

      <IngredientModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        initialData={selectedIngredient}
        onSubmit={handleSubmit}
        isLoading={isSubmitting}
      />

      {viewIngredient && (
        <div className="fixed inset-0 z-40 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4">
          <div className="w-full max-w-lg rounded-2xl border border-white/20 bg-slate-900 p-6">
            <h3 className="text-2xl font-bold text-white mb-4">{viewIngredient.name}</h3>
            <div className="grid grid-cols-2 gap-3 text-sm">
              <InfoRow label="Unit" value={viewIngredient.unit} />
              <InfoRow label="Calories / Unit" value={viewIngredient.caloriesPerUnit.toFixed(1)} />
              <InfoRow label="Protein" value={viewIngredient.protein.toFixed(1)} />
              <InfoRow label="Carbs" value={viewIngredient.carbs.toFixed(1)} />
              <InfoRow label="Fat" value={viewIngredient.fat.toFixed(1)} />
            </div>
            <div className="mt-6 flex justify-end">
              <button
                onClick={() => setViewIngredient(null)}
                className="px-4 py-2 rounded-lg bg-slate-700 hover:bg-slate-600 text-white"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

const InfoRow: React.FC<{ label: string; value: string }> = ({ label, value }) => (
  <div className="p-3 rounded-lg bg-slate-800/80 border border-white/10">
    <p className="text-gray-400 uppercase text-xs">{label}</p>
    <p className="text-white font-semibold">{value}</p>
  </div>
);
