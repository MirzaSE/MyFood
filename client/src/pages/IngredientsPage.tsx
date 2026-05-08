import React, { useEffect, useState } from 'react';
import { Plus } from 'lucide-react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types';
import { IngredientModal } from '../components/IngredientModal';
import { IngredientList } from '../components/IngredientList';

export const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalTitle, setModalTitle] = useState('');
  const [selected, setSelected] = useState<Ingredient | null>(null);

  const fetchIngredients = async () => {
    setLoading(true);
    try {
      const data = await ingredientService.getAllIngredients({ page: 1, pageCount: 50 });
      setIngredients(data);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchIngredients();
  }, []);

  const handleCreate = async (dto: IngredientCreateDto) => {
    setLoading(true);
    try {
      await ingredientService.createIngredient(dto);
      await fetchIngredients();
      setModalOpen(false);
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = async (dto: IngredientCreateDto) => {
    if (!selected) return;
    setLoading(true);
    try {
      await ingredientService.updateIngredient(selected.id, dto);
      await fetchIngredients();
      setModalOpen(false);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (ingredient: Ingredient) => {
    if (!confirm(`Delete ${ingredient.name}?`)) return;
    setLoading(true);
    try {
      await ingredientService.deleteIngredient(ingredient.id);
      await fetchIngredients();
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto px-6 py-10">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-white">Ingredients</h2>
        <div>
          <button
            onClick={() => {
              setSelected(null);
              setModalTitle('Create Ingredient');
              setModalOpen(true);
            }}
            className="inline-flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg shadow"
          >
            <Plus />
            Add Ingredient
          </button>
        </div>
      </div>

      <IngredientList
        ingredients={ingredients}
        onView={(i) => {
          setSelected(i);
          setModalTitle('View Ingredient');
          setModalOpen(true);
        }}
        onEdit={(i) => {
          setSelected(i);
          setModalTitle('Edit Ingredient');
          setModalOpen(true);
        }}
        onDelete={handleDelete}
        isLoading={loading}
      />

      <IngredientModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSubmit={selected ? handleEdit : handleCreate}
        initialData={selected}
        title={modalTitle}
        isLoading={loading}
        readOnly={modalTitle === 'View Ingredient'}
      />
    </div>
  );
};
