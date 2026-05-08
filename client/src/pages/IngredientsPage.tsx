import React, { useEffect, useMemo, useState } from 'react';
import { Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import { foodService } from '../services/foodService';
import type { Food, Ingredient, IngredientCreateDto } from '../types';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [foods, setFoods] = useState<Food[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selected, setSelected] = useState<Ingredient | null>(null);
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);

  const pageSize = 10;

  useEffect(() => {
    void loadData();
  }, []);

  const loadData = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const [ingredientsData, foodsData] = await Promise.all([
        ingredientService.getAllIngredients('', 1, 50),
        foodService.getAllFoods(),
      ]);
      setIngredients(ingredientsData);
      setFoods(foodsData);
    } catch {
      setError('Failed to load ingredients.');
    } finally {
      setIsLoading(false);
    }
  };

  const onSubmit = async (dto: IngredientCreateDto) => {
    if (selected) {
      const updated = await ingredientService.updateIngredient(selected.id, dto);
      setIngredients((prev) => prev.map((x) => (x.id === selected.id ? updated : x)));
    } else {
      const created = await ingredientService.createIngredient(dto);
      setIngredients((prev) => [created, ...prev]);
    }
    setIsModalOpen(false);
    setSelected(null);
  };

  const onDelete = async (id: number) => {
    await ingredientService.deleteIngredient(id);
    setIngredients((prev) => prev.filter((x) => x.id !== id));
  };

  const titleCount = useMemo(() => ingredients.length, [ingredients.length]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />
      <div className="container mx-auto px-6 py-12">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-3xl font-bold text-white">Ingredients</h1>
            <p className="text-gray-400">{titleCount} total ingredients</p>
          </div>
          <button
            className="flex items-center gap-2 px-4 py-2 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 text-white"
            onClick={() => {
              setSelected(null);
              setIsModalOpen(true);
            }}
          >
            <Plus size={18} />
            New Ingredient
          </button>
        </div>

        {error && <div className="mb-4 text-red-300">{error}</div>}

        <IngredientList
          ingredients={ingredients}
          currentPage={page}
          pageSize={pageSize}
          search={search}
          isLoading={isLoading}
          onSearchChange={setSearch}
          onPageChange={setPage}
          onEdit={(ingredient) => {
            setSelected(ingredient);
            setIsModalOpen(true);
          }}
          onDelete={onDelete}
        />
      </div>

      <IngredientModal
        isOpen={isModalOpen}
        foods={foods}
        initialData={selected}
        onClose={() => {
          setIsModalOpen(false);
          setSelected(null);
        }}
        onSubmit={onSubmit}
      />
    </div>
  );
};
