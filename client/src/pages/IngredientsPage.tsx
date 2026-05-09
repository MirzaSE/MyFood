import React, { useState } from 'react';
import { Plus } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import type { Ingredient } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [modalOpen, setModalOpen] = useState(false);
  const [selected, setSelected] = useState<Ingredient | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const openCreate = () => {
    setSelected(null);
    setModalOpen(true);
  };

  const openEdit = (ingredient: Ingredient) => {
    setSelected(ingredient);
    setModalOpen(true);
  };

  const handleSaved = () => {
    setRefreshKey((k) => k + 1);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-12">
        <div className="mb-10 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-6">
          <div>
            <h1 className="text-4xl sm:text-5xl font-bold text-white mb-3">Ingredients</h1>
            <p className="text-gray-400">Manage the ingredient catalog used to build foods.</p>
          </div>
          <button
            onClick={openCreate}
            className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 shadow-lg hover:shadow-purple-500/50 font-semibold"
          >
            <Plus size={20} />
            <span>New Ingredient</span>
          </button>
        </div>

        <IngredientList refreshKey={refreshKey} onEdit={openEdit} />
      </div>

      <IngredientModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSaved={handleSaved}
        initialData={selected}
      />
    </div>
  );
};
