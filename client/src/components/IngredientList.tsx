import React, { useState, useEffect } from "react";
import type { Ingredient } from "../types";
import { ingredientService } from "../services/ingredientService";

interface IngredientListProps {
  onEdit: (ingredient: Ingredient) => void;
  onDelete: (id: number) => void;
  refresh?: boolean;
  onRefreshComplete?: () => void;
}

export const IngredientList: React.FC<IngredientListProps> = ({
  onEdit,
  onDelete,
  refresh = false,
  onRefreshComplete,
}) => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  const loadIngredients = async () => {
    try {
      setLoading(true);
      setError("");
      const data = await ingredientService.getAllIngredients({
        pageNumber: 1,
        pageSize: 1000,
      });
      setIngredients(data.value ?? []);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Failed to load ingredients",
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadIngredients();
  }, []);

  useEffect(() => {
    if (refresh) {
      loadIngredients();
      onRefreshComplete?.();
    }
  }, [refresh]);

  const handleSearch = (value: string) => {
    setSearchTerm(value);
    setCurrentPage(1);
  };

  const filteredIngredients = ingredients.filter((ingredient) =>
    ingredient.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  const totalPages = Math.ceil(filteredIngredients.length / pageSize);
  const paginatedIngredients = filteredIngredients.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize,
  );

  if (loading && ingredients.length === 0) {
    return (
      <div className="rounded-xl border border-white/10 bg-gradient-to-r from-slate-900 to-slate-800 py-10 text-center text-gray-300 shadow-inner">
        Loading ingredients...
      </div>
    );
  }

  return (
    <div className="space-y-5">
      <div className="rounded-xl border border-slate-200 bg-slate-50 p-3 shadow-sm">
        <input
          type="text"
          placeholder="Search ingredients..."
          value={searchTerm}
          onChange={(e) => handleSearch(e.target.value)}
          className="w-full rounded-lg border border-slate-300 bg-white px-4 py-3 text-sm text-slate-900 placeholder:text-slate-400 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/20"
        />
      </div>

      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700 shadow-sm">
          {error}
        </div>
      )}

      {paginatedIngredients.length === 0 ? (
        <div className="rounded-xl border border-dashed border-slate-300 bg-gradient-to-br from-slate-50 to-slate-100 p-10 text-center shadow-sm">
          <p className="text-slate-500">
            {searchTerm
              ? "No ingredients found matching your search."
              : "No ingredients yet. Create one to get started!"}
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-2xl border border-white/10 bg-gradient-to-br from-slate-900 to-slate-800 shadow-xl shadow-slate-950/20">
          <table className="w-full">
            <thead className="bg-white/5">
              <tr>
                <th className="px-6 py-4 text-left text-sm font-semibold text-slate-100">
                  Name
                </th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-slate-100">
                  Unit
                </th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-slate-100">
                  Calories
                </th>
                <th className="px-6 py-4 text-right text-sm font-semibold text-slate-100">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-white/10">
              {paginatedIngredients.map((ingredient) => (
                <tr
                  key={ingredient.id}
                  className="transition-colors hover:bg-white/5"
                >
                  <td className="px-6 py-4 text-sm text-slate-100">
                    {ingredient.name}
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-300">
                    {ingredient.unit || "—"}
                  </td>
                  <td className="px-6 py-4 text-sm text-slate-300">
                    {ingredient.caloriesPerUnit ?? "—"}
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <button
                        onClick={() => onEdit(ingredient)}
                        className="rounded-lg border border-blue-500/30 bg-blue-500/15 px-3 py-1.5 text-sm font-medium text-blue-200 transition-all hover:bg-blue-500/25"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => onDelete(ingredient.id)}
                        className="rounded-lg border border-red-500/30 bg-red-500/15 px-3 py-1.5 text-sm font-medium text-red-200 transition-all hover:bg-red-500/25"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {totalPages > 1 && (
        <div className="flex items-center justify-between rounded-xl border border-slate-200 bg-slate-50 px-4 py-3">
          <p className="text-sm text-slate-600">
            Page {currentPage} of {totalPages} ({filteredIngredients.length}{" "}
            total)
          </p>
          <div className="flex gap-2">
            <button
              onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
              disabled={currentPage === 1}
              className="rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-sm text-slate-700 shadow-sm transition-all hover:bg-slate-50 disabled:bg-slate-100 disabled:text-slate-400"
            >
              Previous
            </button>
            <button
              onClick={() =>
                setCurrentPage(Math.min(totalPages, currentPage + 1))
              }
              disabled={currentPage === totalPages}
              className="rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-sm text-slate-700 shadow-sm transition-all hover:bg-slate-50 disabled:bg-slate-100 disabled:text-slate-400"
            >
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
