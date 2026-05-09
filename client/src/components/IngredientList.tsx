import React, { useState, useEffect } from 'react';
import type { Ingredient } from '../types/ingredient';
import { ingredientService } from '..//services/ingredientService';
import { IngredientModal } from './IngredientModal';
import { Search, Plus, Edit, Trash2, ChevronLeft, ChevronRight } from 'lucide-react';

export const IngredientList: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [page, setPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [selected, setSelected] = useState<Ingredient | undefined>();
  const pageCount = 10;

  const load = async () => {
    setLoading(true);
    try {
      const data = searchQuery
        ? await ingredientService.search(searchQuery)
        : await ingredientService.getAll(page, pageCount);
      setIngredients(data);
    } catch {
      setIngredients([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [page, searchQuery]);

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this ingredient?')) return;
    try {
      await ingredientService.delete(id);
      load();
    } catch {
      alert('Failed to delete.');
    }
  };

  const handleEdit = (ingredient: Ingredient) => {
    setSelected(ingredient);
    setShowModal(true);
  };

  const handleNew = () => {
    setSelected(undefined);
    setShowModal(true);
  };

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex flex-col sm:flex-row gap-3 justify-between items-start sm:items-center">
        <div className="relative flex-1 max-w-sm">
          <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            className="w-full pl-9 pr-3 py-2 bg-white/5 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 text-sm"
            placeholder="Search ingredients..."
            value={searchQuery}
            onChange={e => { setSearchQuery(e.target.value); setPage(1); }}
          />
        </div>
        <button
          onClick={handleNew}
          className="flex items-center gap-2 px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg font-medium transition-colors text-sm"
        >
          <Plus size={16} /> New Ingredient
        </button>
      </div>

      {/* Table */}
      {loading ? (
        <div className="text-center py-12 text-gray-400">Loading...</div>
      ) : ingredients.length === 0 ? (
        <div className="text-center py-12 text-gray-400">No ingredients found.</div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-white/10">
          <table className="w-full text-sm">
            <thead className="bg-white/5 text-gray-400 uppercase text-xs">
              <tr>
                <th className="px-4 py-3 text-left">Name</th>
                <th className="px-4 py-3 text-left">Unit</th>
                <th className="px-4 py-3 text-left">Calories</th>
                <th className="px-4 py-3 text-left">Protein</th>
                <th className="px-4 py-3 text-left">Carbs</th>
                <th className="px-4 py-3 text-left">Fat</th>
                <th className="px-4 py-3 text-left">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/5">
              {ingredients.map(i => (
                <tr key={i.id} className="hover:bg-white/5 transition-colors">
                  <td className="px-4 py-3 text-white font-medium">{i.name}</td>
                  <td className="px-4 py-3 text-gray-300">{i.unit}</td>
                  <td className="px-4 py-3 text-gray-300">{i.caloriesPerUnit}</td>
                  <td className="px-4 py-3 text-gray-300">{i.protein}g</td>
                  <td className="px-4 py-3 text-gray-300">{i.carbs}g</td>
                  <td className="px-4 py-3 text-gray-300">{i.fat}g</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button onClick={() => handleEdit(i)} className="p-1.5 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 rounded-lg transition-colors">
                        <Edit size={14} />
                      </button>
                      <button onClick={() => handleDelete(i.id)} className="p-1.5 bg-red-500/20 hover:bg-red-500/30 text-red-300 rounded-lg transition-colors">
                        <Trash2 size={14} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Pagination */}
      {!searchQuery && (
        <div className="flex justify-center gap-3 items-center">
          <button
            onClick={() => setPage(p => Math.max(1, p - 1))}
            disabled={page === 1}
            className="p-2 bg-white/5 hover:bg-white/10 text-white rounded-lg disabled:opacity-40 transition-colors"
          >
            <ChevronLeft size={16} />
          </button>
          <span className="text-gray-400 text-sm">Page {page}</span>
          <button
            onClick={() => setPage(p => p + 1)}
            disabled={ingredients.length < pageCount}
            className="p-2 bg-white/5 hover:bg-white/10 text-white rounded-lg disabled:opacity-40 transition-colors"
          >
            <ChevronRight size={16} />
          </button>
        </div>
      )}

      {showModal && (
        <IngredientModal
          onClose={() => setShowModal(false)}
          onSuccess={load}
          existing={selected}
        />
      )}
    </div>
  );
};