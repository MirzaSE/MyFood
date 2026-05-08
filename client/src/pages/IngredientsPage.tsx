import { useEffect, useState } from 'react';
import { Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type {
  Ingredient,
  IngredientCreateDto,
} from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [viewIngredient, setViewIngredient] = useState<Ingredient | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const pageSize = 5;

  const loadIngredients = async (search = searchTerm) => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await ingredientService.getIngredients(1, 100, search);
      setIngredients(data);
      setCurrentPage(1);
    } catch {
      setError('Failed to load ingredients.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadIngredients('');
  }, []);

  const handleSearchChange = (value: string) => {
    setSearchTerm(value);
    loadIngredients(value);
  };

  const handleCreateClick = () => {
    setSelectedIngredient(null);
    setMessage(null);
    setError(null);
    setModalOpen(true);
  };

  const handleEditClick = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setMessage(null);
    setError(null);
    setModalOpen(true);
  };

  const handleSubmit = async (data: IngredientCreateDto) => {
    try {
      setIsSubmitting(true);
      setError(null);

      if (selectedIngredient) {
        await ingredientService.updateIngredient(selectedIngredient.id, data);
        setMessage('Ingredient updated successfully.');
      } else {
        await ingredientService.createIngredient(data);
        setMessage('Ingredient created successfully.');
      }

      await loadIngredients(searchTerm);
      setModalOpen(false);
      setSelectedIngredient(null);
    } catch {
      setError('Failed to save ingredient.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await ingredientService.deleteIngredient(id);
      await loadIngredients(searchTerm);
      setMessage('Ingredient deleted successfully.');
    } catch {
      setError('Failed to delete ingredient.');
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />

      <main className="mx-auto max-w-6xl p-6">
        {error && (
          <div className="mb-4 rounded-lg bg-red-50 p-3 text-red-700">
            {error}
          </div>
        )}

        {message && (
          <div className="mb-4 rounded-lg bg-green-50 p-3 text-green-700">
            {message}
          </div>
        )}

        <div className="mb-6 flex flex-col justify-between gap-4 md:flex-row md:items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">
              Ingredients
            </h1>
            <p className="text-gray-600">
              Manage ingredient nutrition data for your foods.
            </p>
          </div>

          <button
            type="button"
            onClick={handleCreateClick}
            className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-white"
          >
            <Plus size={18} />
            New Ingredient
          </button>
        </div>

        {viewIngredient && (
          <div className="mb-4 rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
            <div className="flex justify-between">
              <div>
                <h2 className="text-xl font-semibold">{viewIngredient.name}</h2>
                <p className="text-sm text-gray-500">
                  Unit: {viewIngredient.unit}
                </p>
                <p className="text-sm text-gray-500">
                  Calories: {viewIngredient.caloriesPerUnit}
                </p>
                <p className="text-sm text-gray-500">
                  Protein: {viewIngredient.protein}, Carbs: {viewIngredient.carbs}, Fat: {viewIngredient.fat}
                </p>
              </div>
              <button
                type="button"
                onClick={() => setViewIngredient(null)}
                className="text-sm text-gray-500"
              >
                Close
              </button>
            </div>
          </div>
        )}

        <IngredientList
          ingredients={ingredients}
          isLoading={isLoading}
          searchTerm={searchTerm}
          currentPage={currentPage}
          pageSize={pageSize}
          onSearchChange={handleSearchChange}
          onPageChange={setCurrentPage}
          onEdit={handleEditClick}
          onDelete={handleDelete}
          onView={setViewIngredient}
        />

        <IngredientModal
          isOpen={modalOpen}
          initialData={selectedIngredient}
          isLoading={isSubmitting}
          message={null}
          error={error}
          onClose={() => {
            setModalOpen(false);
            setSelectedIngredient(null);
          }}
          onSubmit={handleSubmit}
        />
      </main>
    </div>
  );
};