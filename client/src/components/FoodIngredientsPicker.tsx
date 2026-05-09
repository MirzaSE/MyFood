import React, { useState, useEffect } from "react";
import type { Ingredient } from "../types";
import { ingredientService } from "../services/ingredientService";

interface SelectedIngredient {
  id: number;
  name: string;
  unit?: string;
  caloriesPerUnit?: number;
  quantity: number;
}

interface FoodIngredientsPickerProps {
  selectedIngredients: SelectedIngredient[];
  onIngredientAdd: (ingredient: SelectedIngredient) => void;
  onIngredientRemove: (ingredientId: number) => void;
  onQuantityChange: (ingredientId: number, quantity: number) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  selectedIngredients,
  onIngredientAdd,
  onIngredientRemove,
  onQuantityChange,
}) => {
  const [availableIngredients, setAvailableIngredients] = useState<
    Ingredient[]
  >([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedIngredientId, setSelectedIngredientId] = useState<number | "">(
    "",
  );
  const [quantity, setQuantity] = useState("");

  useEffect(() => {
    const loadIngredients = async () => {
      try {
        setLoading(true);
        const data = await ingredientService.getAllIngredients({
          pageNumber: 1,
          pageSize: 100,
        });
        setAvailableIngredients(data.value ?? []);
      } catch (error) {
        console.error("Failed to load ingredients:", error);
      } finally {
        setLoading(false);
      }
    };

    loadIngredients();
  }, []);

  const handleAddIngredient = () => {
    if (!selectedIngredientId || !quantity) {
      alert("Please select an ingredient and enter a quantity");
      return;
    }

    const ingredient = availableIngredients.find(
      (i) => i.id === selectedIngredientId,
    );
    if (!ingredient) return;

    const isAlreadySelected = selectedIngredients.some(
      (i) => i.id === selectedIngredientId,
    );
    if (isAlreadySelected) {
      alert("This ingredient is already selected");
      return;
    }

    onIngredientAdd({
      id: ingredient.id,
      name: ingredient.name,
      unit: ingredient.unit,
      caloriesPerUnit: ingredient.caloriesPerUnit,
      quantity: parseFloat(quantity),
    });

    setSelectedIngredientId("");
    setQuantity("");
  };

  const totalCalories = selectedIngredients.reduce((sum, ing) => {
    return sum + (ing.quantity || 0) * (ing.caloriesPerUnit || 0);
  }, 0);

  const filteredIngredients = availableIngredients.filter(
    (ing) =>
      ing.name.toLowerCase().includes(searchTerm.toLowerCase()) &&
      !selectedIngredients.some((s) => s.id === ing.id),
  );

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold text-white">Add Ingredients</h3>

      <div className="rounded-2xl border border-white/10 bg-white/5 p-4 shadow-lg shadow-slate-950/20 backdrop-blur">
        <div className="space-y-3">
          <div>
            <label className="block text-sm font-medium text-gray-200">
              Search Ingredients
            </label>
            <input
              type="text"
              placeholder="Search..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/20"
            />
            {/* Suggestions dropdown */}
            {searchTerm.trim() !== "" && (
              <div className="mt-2 max-h-44 overflow-auto rounded-lg border border-slate-200 bg-white shadow-lg z-10">
                {availableIngredients
                  .filter(
                    (ing) =>
                      ing.name
                        .toLowerCase()
                        .includes(searchTerm.toLowerCase()) &&
                      !selectedIngredients.some((s) => s.id === ing.id),
                  )
                  .slice(0, 10)
                  .map((ing) => (
                    <button
                      key={ing.id}
                      type="button"
                      onClick={() => {
                        setSelectedIngredientId(ing.id);
                        setSearchTerm(ing.name);
                      }}
                      className="w-full text-left px-3 py-2 hover:bg-slate-100 text-slate-800"
                    >
                      {ing.name} {ing.unit ? `· ${ing.unit}` : ""}
                    </button>
                  ))}
                {availableIngredients.filter(
                  (ing) =>
                    ing.name.toLowerCase().includes(searchTerm.toLowerCase()) &&
                    !selectedIngredients.some((s) => s.id === ing.id),
                ).length === 0 && (
                  <div className="px-3 py-2 text-sm text-slate-500">
                    No matches
                  </div>
                )}
              </div>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-200">
              Select Ingredient
            </label>
            <select
              value={selectedIngredientId}
              onChange={(e) =>
                setSelectedIngredientId(
                  e.target.value ? parseInt(e.target.value) : "",
                )
              }
              disabled={loading}
              className="mt-1 w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/20"
            >
              <option value="">Choose an ingredient...</option>
              {filteredIngredients.map((ing) => (
                <option key={ing.id} value={ing.id}>
                  {ing.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-200">
              Quantity
            </label>
            <input
              type="number"
              placeholder="Enter quantity"
              value={quantity}
              onChange={(e) => setQuantity(e.target.value)}
              min="0"
              step="0.1"
              className="mt-1 w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/20"
            />
          </div>

          <button
            onClick={handleAddIngredient}
            className="w-full rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-4 py-2.5 font-medium text-white shadow-lg shadow-purple-500/20 transition-all hover:from-purple-700 hover:to-blue-700"
          >
            Add Ingredient
          </button>
        </div>
      </div>

      {selectedIngredients.length > 0 && (
        <div className="space-y-3">
          <h4 className="font-medium text-white">Selected Ingredients</h4>
          <div className="space-y-2">
            {selectedIngredients.map((ing) => (
              <div
                key={ing.id}
                className="flex items-center justify-between rounded-xl border border-white/10 bg-white/5 p-3"
              >
                <div className="flex-1">
                  <p className="font-medium text-white">{ing.name}</p>
                  <p className="text-sm text-gray-300">
                    {ing.quantity} {ing.unit || "unit(s)"}
                  </p>
                  <p className="text-xs text-gray-400">
                    {ing.quantity.toFixed(2)} ×{" "}
                    {(ing.caloriesPerUnit || 0).toFixed(2)} kcal ={" "}
                    {(ing.quantity * (ing.caloriesPerUnit || 0)).toFixed(2)}{" "}
                    kcal
                  </p>
                </div>
                <div className="flex gap-2">
                  <input
                    type="number"
                    value={ing.quantity}
                    onChange={(e) =>
                      onQuantityChange(ing.id, parseFloat(e.target.value) || 0)
                    }
                    min="0"
                    step="0.1"
                    className="w-24 rounded-lg border border-slate-300 bg-white px-2 py-1 text-sm text-slate-900"
                  />
                  <button
                    onClick={() => onIngredientRemove(ing.id)}
                    className="rounded-lg border border-red-500/30 bg-red-500/15 px-3 py-1 text-sm font-medium text-red-200 transition-all hover:bg-red-500/25"
                  >
                    Remove
                  </button>
                </div>
              </div>
            ))}
          </div>

          <div className="rounded-xl border border-emerald-500/20 bg-emerald-500/10 p-3">
            <p className="text-sm font-medium text-emerald-100">
              Total Calories:{" "}
              <span className="text-lg">{totalCalories.toFixed(1)}</span>
            </p>
          </div>
        </div>
      )}
    </div>
  );
};
