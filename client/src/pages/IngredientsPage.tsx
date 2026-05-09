import React, { useState } from 'react';
import { Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import type { Ingredient } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingIngredient, setEditingIngredient] = useState<Ingredient | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleEdit = (ingredient: Ingredient) => {
    setEditingIngredient(ingredient);
    setIsModalOpen(true);
  };

  const handleAdd = () => {
    setEditingIngredient(null);
    setIsModalOpen(true);
  };

  const handleSuccess = () => {
    setRefreshKey(k => k + 1);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-950 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-8">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-4xl font-bold text-white">Ingredients</h1>
            <p className="text-gray-400 mt-1">Manage your ingredient library</p>
          </div>
          <button
            onClick={handleAdd}
            className="flex items-center space-x-2 px-5 py-2.5 bg-purple-600 text-white rounded-xl hover:bg-purple-700 transition font-medium"
          >
            <Plus size={18} />
            <span>New Ingredient</span>
          </button>
        </div>

        <div className="bg-white/5 border border-white/10 rounded-2xl p-6">
          <IngredientList onEdit={handleEdit} refreshKey={refreshKey} />
        </div>
      </div>

      <IngredientModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSuccess={handleSuccess}
        editingIngredient={editingIngredient}
      />
    </div>
  );
};