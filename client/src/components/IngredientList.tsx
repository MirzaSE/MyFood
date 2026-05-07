import React from 'react';
import { Pencil, Trash2, Eye, ChevronLeft, ChevronRight } from 'lucide-react';
import type { Ingredient } from '../types/ingredient';

const fmt = (n: number) =>
  typeof n === 'number' && !Number.isNaN(n) ? Number(n).toLocaleString(undefined, { maximumFractionDigits: 2 }) : '—';

export interface IngredientListProps {
  ingredients: Ingredient[];
  isLoading: boolean;
  searchQuery: string;
  page: number;
  pageSize: number;
  onEdit: (row: Ingredient) => void;
  onDelete: (row: Ingredient) => void;
  onView: (row: Ingredient) => void;
  onPageChange: (page: number) => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  ingredients,
  isLoading,
  searchQuery,
  page,
  pageSize,
  onEdit,
  onDelete,
  onView,
  onPageChange,
}) => {
  const total = ingredients.length;
  const totalPages = Math.max(1, Math.ceil(total / pageSize));
  const safePage = Math.min(page, totalPages);
  const start = (safePage - 1) * pageSize;
  const pageRows = ingredients.slice(start, start + pageSize);

  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center py-20 text-gray-300">
        <div className="w-12 h-12 border-2 border-purple-500/30 border-t-purple-400 rounded-full animate-spin mb-4" />
        Loading ingredients…
      </div>
    );
  }

  if (total === 0) {
    return (
      <div className="text-center py-16 text-gray-400 border border-white/10 rounded-xl bg-white/5">
        {searchQuery.trim() ? 'No ingredients match your search.' : 'No ingredients yet. Add one to get started.'}
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-xl border border-white/10 bg-white/5">
      <table className="w-full text-left text-sm text-gray-200">
        <thead className="bg-white/10 text-gray-300 uppercase text-xs tracking-wide">
          <tr>
            <th className="px-4 py-3">Name</th>
            <th className="px-4 py-3">Unit</th>
            <th className="px-4 py-3">Calories / unit</th>
            <th className="px-4 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          {pageRows.map((row) => (
            <tr key={row.id} className="border-t border-white/10 hover:bg-white/5">
              <td className="px-4 py-3 font-medium text-white">{row.name}</td>
              <td className="px-4 py-3">{row.unit}</td>
              <td className="px-4 py-3">{fmt(row.caloriesPerUnit)}</td>
              <td className="px-4 py-3">
                <div className="flex justify-end gap-2">
                  <button
                    type="button"
                    onClick={() => onView(row)}
                    className="p-2 rounded-lg bg-white/10 hover:bg-white/20 text-gray-200"
                    title="View"
                  >
                    <Eye size={16} />
                  </button>
                  <button
                    type="button"
                    onClick={() => onEdit(row)}
                    className="p-2 rounded-lg bg-purple-500/20 hover:bg-purple-500/30 text-purple-200"
                    title="Edit"
                  >
                    <Pencil size={16} />
                  </button>
                  <button
                    type="button"
                    onClick={() => onDelete(row)}
                    className="p-2 rounded-lg bg-red-500/20 hover:bg-red-500/30 text-red-200"
                    title="Delete"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="flex items-center justify-between px-4 py-3 border-t border-white/10 text-sm text-gray-400">
        <span>
          {total === 0 ? '' : `Showing ${start + 1}–${Math.min(start + pageSize, total)} of ${total}`}
        </span>
        <div className="flex items-center gap-2">
          <button
            type="button"
            disabled={safePage <= 1}
            onClick={() => onPageChange(safePage - 1)}
            className="p-2 rounded-lg bg-white/10 disabled:opacity-30 hover:bg-white/20"
          >
            <ChevronLeft size={18} />
          </button>
          <span className="text-gray-300">
            Page {safePage} / {totalPages}
          </span>
          <button
            type="button"
            disabled={safePage >= totalPages}
            onClick={() => onPageChange(safePage + 1)}
            className="p-2 rounded-lg bg-white/10 disabled:opacity-30 hover:bg-white/20"
          >
            <ChevronRight size={18} />
          </button>
        </div>
      </div>
    </div>
  );
};
