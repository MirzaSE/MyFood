import React, { useEffect, useState } from 'react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { IngredientViewModal } from '../components/IngredientViewModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [editing, setEditing] = useState<Ingredient | null>(null);
  const [viewing, setViewing] = useState<Ingredient | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  const loadData = async () => {
    setLoading(true);
    try {
      const data = search.trim()
        ? await ingredientService.searchIngredients(search.trim())
        : await ingredientService.getAllIngredients(page, 10, '');
      setIngredients(data);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [page]);

  useEffect(() => {
    const timeout = setTimeout(() => {
      loadData();
    }, 300);
    return () => clearTimeout(timeout);
  }, [search]);

  const handleSubmit = async (data: IngredientCreateDto) => {
    if (editing) {
      await ingredientService.updateIngredient(editing.id, data);
      setMessage('Ingredient updated successfully.');
    } else {
      await ingredientService.createIngredient(data);
      setMessage('Ingredient created successfully.');
    }

    setModalOpen(false);
    setEditing(null);
    await loadData();
  };

  const handleDelete = async (id: number) => {
    await ingredientService.deleteIngredient(id);
    setMessage('Ingredient deleted successfully.');
    await loadData();
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-10 space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-3xl text-white font-bold">Ingredients</h1>
          <button className="px-4 py-2 rounded bg-blue-600 text-white" onClick={() => { setEditing(null); setModalOpen(true); }}>
            New Ingredient
          </button>
        </div>
        {message && <p className="text-green-300">{message}</p>}
        <IngredientList
          ingredients={ingredients}
          isLoading={loading}
          page={page}
          onPageChange={setPage}
          search={search}
          onSearchChange={setSearch}
          onEdit={(ingredient) => { setEditing(ingredient); setModalOpen(true); }}
          onView={(ingredient) => setViewing(ingredient)}
          onDelete={handleDelete}
        />
      </div>
      <IngredientModal
        isOpen={modalOpen}
        initialData={editing}
        onClose={() => { setModalOpen(false); setEditing(null); }}
        onSubmit={handleSubmit}
        isLoading={loading}
        statusMessage={message}
      />
      <IngredientViewModal
        isOpen={viewing !== null}
        ingredient={viewing}
        onClose={() => setViewing(null)}
      />
    </div>
  );
};
