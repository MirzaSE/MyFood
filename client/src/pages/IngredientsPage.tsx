import { useState, useEffect } from 'react';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';
import { Navbar } from '../components/Navbar';
import { ingredientService } from '../services/ingredientService';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';

export function IngredientsPage() {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingIngredient, setEditingIngredient] = useState<Ingredient | undefined>();
  const [viewingIngredient, setViewingIngredient] = useState<Ingredient | undefined>();
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const loadIngredients = async (page: number = 1, searchTerm: string = '') => {
    setIsLoading(true);
    setError(null);

    try {
      let data: Ingredient[];
      if (searchTerm) {
        data = await ingredientService.search(searchTerm);
        setTotalPages(1);
      } else {
        data = await ingredientService.getAll(page, 10);
        setTotalPages(Math.ceil(data.length / 10) || 1);
      }
      setIngredients(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load ingredients');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadIngredients(currentPage);
  }, [currentPage]);

  const handleCreate = async (data: IngredientCreateDto) => {
    await ingredientService.create(data);
    await loadIngredients(currentPage);
  };

  const handleUpdate = async (data: IngredientUpdateDto) => {
    if (!editingIngredient) return;
    await ingredientService.update(editingIngredient.id, data);
    await loadIngredients(currentPage);
  };

  const handleSave = async (data: IngredientCreateDto | IngredientUpdateDto) => {
    if (editingIngredient) {
      await handleUpdate(data);
      return;
    }

    await handleCreate(data as IngredientCreateDto);
  };

  const handleDelete = async (id: number) => {
    try {
      await ingredientService.delete(id);
      await loadIngredients(currentPage);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete ingredient');
    }
  };

  const handleSearch = (term: string) => {
    loadIngredients(1, term);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const openCreateModal = () => {
    setEditingIngredient(undefined);
    setModalOpen(true);
  };

  const openEditModal = (ingredient: Ingredient) => {
    setEditingIngredient(ingredient);
    setModalOpen(true);
  };

  const openViewModal = (ingredient: Ingredient) => {
    setViewingIngredient(ingredient);
  };

  const closeModal = () => {
    setModalOpen(false);
    setEditingIngredient(undefined);
  };

  return (
    <div className="min-h-screen bg-slate-50">
      <Navbar />
      <main className="container mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold">Ingredients</h1>
          <button
            onClick={openCreateModal}
            className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
          >
            New Ingredient
          </button>
        </div>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        <IngredientList
          ingredients={ingredients}
          isLoading={isLoading}
          onView={openViewModal}
          onEdit={openEditModal}
          onDelete={handleDelete}
          onSearch={handleSearch}
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={handlePageChange}
        />

        <IngredientModal
          isOpen={modalOpen}
          ingredient={editingIngredient}
          onClose={closeModal}
          onSave={handleSave}
        />

        {viewingIngredient && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-md">
              <div className="flex items-start justify-between mb-4">
                <h2 className="text-2xl font-bold">{viewingIngredient.name}</h2>
                <button
                  onClick={() => setViewingIngredient(undefined)}
                  className="text-gray-500 hover:text-gray-800"
                >
                  Close
                </button>
              </div>
              <dl className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <dt className="font-semibold text-gray-600">Unit</dt>
                  <dd>{viewingIngredient.unit}</dd>
                </div>
                <div>
                  <dt className="font-semibold text-gray-600">Calories</dt>
                  <dd>{viewingIngredient.caloriesPerUnit.toFixed(2)}</dd>
                </div>
                <div>
                  <dt className="font-semibold text-gray-600">Protein</dt>
                  <dd>{viewingIngredient.protein.toFixed(2)}g</dd>
                </div>
                <div>
                  <dt className="font-semibold text-gray-600">Carbs</dt>
                  <dd>{viewingIngredient.carbs.toFixed(2)}g</dd>
                </div>
                <div>
                  <dt className="font-semibold text-gray-600">Fat</dt>
                  <dd>{viewingIngredient.fat.toFixed(2)}g</dd>
                </div>
              </dl>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
