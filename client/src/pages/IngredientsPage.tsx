import React, { useEffect, useState } from 'react';
import { Plus, AlertCircle } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import type { IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedIngredient, setSelectedIngredient] = useState<Ingredient | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    setLoading(true);
    try {
      const data = await ingredientService.getAll(1, 50);
      setIngredients(data);
    } catch (e) {
      setError('Failed to load ingredients');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setSelectedIngredient(null);
    setModalOpen(true);
  };

  const handleEdit = (ingredient: Ingredient) => {
    setSelectedIngredient(ingredient);
    setModalOpen(true);
  };

  const handleSubmit = async (data: IngredientCreateDto | IngredientUpdateDto) => {
    setIsSubmitting(true);
    try {
      setError(null);
      if (selectedIngredient) {
        const updated = await ingredientService.update(selectedIngredient.id, data);
        setIngredients(ingredients.map((ingredient) => ingredient.id === selectedIngredient.id ? updated : ingredient));
      } else {
        const created = await ingredientService.create(data as IngredientCreateDto);
        setIngredients([...ingredients, created]);
      }
      setModalOpen(false);
      setSelectedIngredient(null);
    } catch (err) {
      setError('Failed to save ingredient');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    setError(null);
    await ingredientService.remove(id);
    setIngredients(ingredients.filter((ingredient) => ingredient.id !== id));
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-16">
        {error && (
          <div className="mb-6 flex items-start gap-3 rounded-lg border border-red-500/40 bg-red-500/15 p-4 text-red-100">
            <AlertCircle size={20} className="mt-0.5 text-red-300" />
            <p>{error}</p>
          </div>
        )}

        <div className="mb-10 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-4xl font-bold text-white">Ingredients</h1>
            <p className="mt-2 text-gray-400">Create and manage ingredient records.</p>
          </div>
          <button onClick={handleCreate} className="inline-flex items-center justify-center gap-2 rounded-lg bg-gradient-to-r from-cyan-500 to-blue-500 px-5 py-3 font-semibold text-white shadow-lg shadow-cyan-950/20 transition hover:from-cyan-400 hover:to-blue-400">
            <Plus size={18} /> Add Ingredient
          </button>
        </div>

        {loading ? (
          <p className="text-gray-300">Loading...</p>
        ) : ingredients.length === 0 ? (
          <p className="text-gray-400">No ingredients found.</p>
        ) : (
          <IngredientList ingredients={ingredients} onEdit={handleEdit} onDelete={handleDelete} isLoading={isSubmitting} />
        )}
      </div>

      <IngredientModal
        isOpen={modalOpen}
        initialData={selectedIngredient}
        isLoading={isSubmitting}
        onClose={() => {
          setModalOpen(false);
          setSelectedIngredient(null);
        }}
        onSubmit={handleSubmit}
      />
    </div>
  );
};

export default IngredientsPage;
