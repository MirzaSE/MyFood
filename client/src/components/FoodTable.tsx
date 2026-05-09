import React from 'react';
import { Edit, Trash2, UtensilsCrossed } from 'lucide-react';
import type { Food } from '../types';

interface FoodTableProps {
  foods: Food[];
  onEdit: (food: Food) => void;
  onDelete: (id: number) => Promise<void>;
  isLoading?: boolean;
}

export const FoodTable: React.FC<FoodTableProps> = ({
  foods,
  onEdit,
  onDelete,
  isLoading = false,
}) => {
  const [deletingId, setDeletingId] = React.useState<number | null>(null);
  const [showConfirm, setShowConfirm] = React.useState<number | null>(null);

  const handleDelete = async (id: number) => {
    try {
      setDeletingId(id);
      await onDelete(id);
      setShowConfirm(null);
    } finally {
      setDeletingId(null);
    }
  };

  if (foods.length === 0) {
    return (
      <div className="text-center py-16">
        <div className="flex justify-center mb-4">
          <div className="p-4 bg-purple-500/20 rounded-full">
            <UtensilsCrossed size={32} className="text-purple-400" />
          </div>
        </div>
        <p className="text-gray-300 text-lg font-medium">No foods found</p>
        <p className="text-gray-400 text-sm mt-1">Create one to get started!</p>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        {foods.map((food) => (
          <div
            key={food.id}
            className="group bg-gradient-to-br from-slate-800 to-slate-900 border border-white/10 hover:border-purple-500/50 rounded-xl p-4 transition-all duration-300 hover:shadow-lg hover:shadow-purple-500/20"
          >
            {/* Card Header */}
            <div className="flex items-start justify-between mb-4">
              <div className="flex-1">
                <h3 className="text-lg font-bold text-white group-hover:text-transparent group-hover:bg-clip-text group-hover:bg-gradient-to-r group-hover:from-purple-400 group-hover:to-blue-400 transition-all">
                  {food.name}
                </h3>
                <span className="inline-block mt-2 px-3 py-1 bg-purple-500/30 text-purple-300 text-xs font-medium rounded-full border border-purple-500/50">
                  {food.type}
                </span>
              </div>
            </div>

            {/* Card Content */}
            <div className="space-y-3 mb-4">
              <div className="flex items-center justify-between p-3 bg-white/5 rounded-lg border border-white/5">
                <span className="text-gray-400 text-sm">Calories</span>
                <span className="text-white font-semibold">{food.calories} kcal</span>
              </div>
              <div className="grid grid-cols-3 gap-2">
                <div className="p-2 bg-white/5 rounded-lg border border-white/5">
                  <span className="block text-gray-400 text-xs">Protein</span>
                  <span className="text-white text-sm font-semibold">{(food.protein ?? 0).toFixed(1)}g</span>
                </div>
                <div className="p-2 bg-white/5 rounded-lg border border-white/5">
                  <span className="block text-gray-400 text-xs">Carbs</span>
                  <span className="text-white text-sm font-semibold">{(food.carbs ?? 0).toFixed(1)}g</span>
                </div>
                <div className="p-2 bg-white/5 rounded-lg border border-white/5">
                  <span className="block text-gray-400 text-xs">Fat</span>
                  <span className="text-white text-sm font-semibold">{(food.fat ?? 0).toFixed(1)}g</span>
                </div>
              </div>
              <div className="flex items-center justify-between p-3 bg-white/5 rounded-lg border border-white/5">
                <span className="text-gray-400 text-sm">Added</span>
                <span className="text-gray-300 text-sm">{new Date(food.created).toLocaleDateString()}</span>
              </div>
            </div>

            {/* Card Actions */}
            <div className="flex space-x-2 gap-2">
              <button
                onClick={() => onEdit(food)}
                className="flex-1 flex items-center justify-center space-x-2 px-3 py-2 bg-blue-500/20 hover:bg-blue-500/30 text-blue-300 hover:text-blue-200 rounded-lg transition-all duration-200 border border-blue-500/30 hover:border-blue-500/50 disabled:opacity-50 disabled:cursor-not-allowed"
                disabled={isLoading || deletingId === food.id}
                title="Edit"
              >
                <Edit size={16} />
                <span className="text-sm font-medium">Edit</span>
              </button>

              <div className="relative flex-1">
                <button
                  onClick={() => setShowConfirm(food.id)}
                  className="w-full flex items-center justify-center space-x-2 px-3 py-2 bg-red-500/20 hover:bg-red-500/30 text-red-300 hover:text-red-200 rounded-lg transition-all duration-200 border border-red-500/30 hover:border-red-500/50 disabled:opacity-50 disabled:cursor-not-allowed"
                  disabled={isLoading || deletingId !== null}
                  title="Delete"
                >
                  <Trash2 size={16} />
                  <span className="text-sm font-medium">Delete</span>
                </button>

                {showConfirm === food.id && (
                  <div className="absolute right-0 top-full mt-2 bg-slate-900 border border-red-500/50 rounded-lg p-4 z-10 w-56 shadow-xl">
                    <p className="text-sm text-gray-200 mb-3 font-medium">Delete this item?</p>
                    <p className="text-xs text-gray-400 mb-4">This action cannot be undone.</p>
                    <div className="flex space-x-2">
                      <button
                        onClick={() => setShowConfirm(null)}
                        className="flex-1 px-3 py-2 bg-slate-700 hover:bg-slate-600 text-gray-300 rounded-lg text-sm font-medium transition-all disabled:opacity-50"
                        disabled={deletingId === food.id}
                      >
                        Cancel
                      </button>
                      <button
                        onClick={() => handleDelete(food.id)}
                        className="flex-1 px-3 py-2 bg-red-500 hover:bg-red-600 text-white rounded-lg text-sm font-medium transition-all disabled:opacity-50"
                        disabled={deletingId === food.id}
                      >
                        {deletingId === food.id ? 'Deleting...' : 'Delete'}
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
