import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Navbar } from "../components/Navbar";
import { FoodIngredientsPicker } from "../components/FoodIngredientsPicker";
import { foodService } from "../services/foodService";
import type { FoodCreateDto } from "../types";

interface SelectedIngredient {
  id: number;
  name: string;
  unit?: string;
  caloriesPerUnit?: number;
  quantity: number;
}

export const CreateFoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [formData, setFormData] = useState({
    name: "",
    type: "",
    calories: 0,
  });

  const [selectedIngredients, setSelectedIngredients] = useState<
    SelectedIngredient[]
  >([]);

  const totalCalories =
    formData.calories +
    selectedIngredients.reduce(
      (sum, ing) => sum + ing.quantity * (ing.caloriesPerUnit || 0),
      0,
    );

  const ingredientCalories = selectedIngredients.reduce(
    (sum, ing) => sum + ing.quantity * (ing.caloriesPerUnit || 0),
    0,
  );

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "calories" ? parseFloat(value) || 0 : value,
    }));
  };

  const handleAddIngredient = (ingredient: SelectedIngredient) => {
    setSelectedIngredients((prev) => [...prev, ingredient]);
  };

  const handleRemoveIngredient = (ingredientId: number) => {
    setSelectedIngredients((prev) =>
      prev.filter((ing) => ing.id !== ingredientId),
    );
  };

  const handleQuantityChange = (ingredientId: number, quantity: number) => {
    setSelectedIngredients((prev) =>
      prev.map((ing) => (ing.id === ingredientId ? { ...ing, quantity } : ing)),
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.name.trim()) {
      setError("Food name is required");
      return;
    }

    if (!formData.type.trim()) {
      setError("Food type is required");
      return;
    }

    try {
      setLoading(true);
      setError("");

      const foodData: FoodCreateDto = {
        name: formData.name,
        type: formData.type,
        calories: totalCalories,
      };

      await foodService.createFood(foodData);
      navigate("/foods");
    } catch (err) {
      const message =
        err instanceof Error ? err.message : "Failed to create food";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-950">
      <Navbar />

      <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="mb-8 rounded-3xl border border-white/10 bg-white/5 p-6 shadow-2xl shadow-black/20 backdrop-blur">
          <h1 className="text-4xl font-bold text-white">Create Food Item</h1>
          <p className="mt-2 text-gray-300">
            Add a new food to your collection with ingredients
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-8">
          {/* Basic Food Information */}
          <div className="rounded-2xl border border-white/10 bg-gradient-to-br from-slate-900 to-slate-800 p-6 shadow-2xl shadow-black/20 backdrop-blur">
            <h2 className="mb-4 text-lg font-semibold text-white">
              Food Information
            </h2>

            <div className="space-y-4">
              {error && (
                <div className="rounded-lg border border-red-500/30 bg-red-500/10 p-4 text-sm text-red-200">
                  {error}
                </div>
              )}

              <div>
                <label
                  htmlFor="name"
                  className="block text-sm font-medium text-gray-200"
                >
                  Food Name *
                </label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  className="mt-1 block w-full rounded-lg border border-white/15 bg-white/10 px-3 py-2.5 text-white shadow-sm placeholder:text-gray-400 focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/20"
                  placeholder="e.g., Grilled Chicken Salad"
                  disabled={loading}
                />
              </div>

              <div>
                <label
                  htmlFor="type"
                  className="block text-sm font-medium text-gray-200"
                >
                  Food Type *
                </label>
                <input
                  type="text"
                  id="type"
                  name="type"
                  value={formData.type}
                  onChange={handleInputChange}
                  className="mt-1 block w-full rounded-lg border border-white/15 bg-white/10 px-3 py-2.5 text-white shadow-sm placeholder:text-gray-400 focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/20"
                  placeholder="e.g., Salad, Entree"
                  disabled={loading}
                />
              </div>

              <div>
                <label
                  htmlFor="calories"
                  className="block text-sm font-medium text-gray-200"
                >
                  Base Calories
                </label>
                <input
                  type="number"
                  id="calories"
                  name="calories"
                  value={formData.calories}
                  onChange={handleInputChange}
                  className="mt-1 block w-full rounded-lg border border-white/15 bg-white/10 px-3 py-2.5 text-white shadow-sm placeholder:text-gray-400 focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/20"
                  placeholder="0"
                  min="0"
                  step="0.1"
                  disabled={loading}
                />
              </div>
            </div>
          </div>

          {/* Ingredients Section */}
          <div className="rounded-2xl border border-white/10 bg-gradient-to-br from-slate-900 to-slate-800 p-6 shadow-2xl shadow-black/20 backdrop-blur">
            <FoodIngredientsPicker
              selectedIngredients={selectedIngredients}
              onIngredientAdd={handleAddIngredient}
              onIngredientRemove={handleRemoveIngredient}
              onQuantityChange={handleQuantityChange}
            />
          </div>

          {/* Nutrition Summary */}
          <div className="rounded-2xl border border-white/10 bg-gradient-to-br from-slate-900 to-slate-800 p-6 shadow-2xl shadow-black/20">
            <h3 className="mb-4 text-lg font-semibold text-white">
              Nutrition Summary
            </h3>

            <div className="grid gap-4 sm:grid-cols-3">
              <div className="rounded-xl border border-white/10 bg-white/5 p-4">
                <p className="text-sm text-gray-300">Base Calories</p>
                <p className="text-2xl font-bold text-white">
                  {formData.calories.toFixed(1)}
                </p>
              </div>

              <div className="rounded-xl border border-white/10 bg-white/5 p-4">
                <p className="text-sm text-gray-300">Ingredients Calories</p>
                <p className="text-2xl font-bold text-white">
                  {ingredientCalories.toFixed(1)}
                </p>
              </div>

              <div className="rounded-xl border border-purple-500/30 bg-gradient-to-br from-purple-500/20 to-blue-500/20 p-4">
                <p className="text-sm font-medium text-gray-200">
                  Total Calories
                </p>
                <p className="text-3xl font-bold text-white">
                  {totalCalories.toFixed(1)}
                </p>
              </div>
            </div>
          </div>

          {/* Form Actions */}
          <div className="flex gap-4">
            <button
              type="submit"
              disabled={loading}
              className="flex-1 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-3 text-white font-semibold shadow-lg shadow-purple-500/20 transition-all hover:from-purple-700 hover:to-blue-700 disabled:bg-gray-400"
            >
              {loading ? "Creating..." : "Create Food Item"}
            </button>
            <button
              type="button"
              onClick={() => navigate("/foods")}
              disabled={loading}
              className="flex-1 rounded-lg border border-white/10 bg-white/5 px-6 py-3 font-semibold text-white transition-all hover:bg-white/10 disabled:bg-white/5 disabled:text-gray-400"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
