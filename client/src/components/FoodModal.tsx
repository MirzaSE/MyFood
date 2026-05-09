import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import { X } from "lucide-react";
import type { Food, FoodCreateDto } from "../types";
import type { SelectedIngredient } from "../types/ingredient";
import { FoodIngredientsPicker } from "./FoodIngredientsPicker";

interface FoodModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: FoodCreateDto) => Promise<void>;
  initialData?: Food | null;
  isLoading?: boolean;
}

export const FoodModal: React.FC<FoodModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  isLoading = false,
}) => {
  const [selectedIngredients, setSelectedIngredients] = React.useState<SelectedIngredient[]>([]);
  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<FoodCreateDto>({
    defaultValues: {
      name: "",
      type: "",
      calories: 0,
      protein: 0,
      carbs: 0,
      fat: 0,
      ingredients: [],
    },
  });

  const nutritionTotals = React.useMemo(
    () =>
      selectedIngredients.reduce(
        (total, item) => ({
          calories: total.calories + item.ingredient.caloriesPerUnit * item.quantity,
          protein: total.protein + item.ingredient.protein * item.quantity,
          carbs: total.carbs + item.ingredient.carbs * item.quantity,
          fat: total.fat + item.ingredient.fat * item.quantity,
        }),
        { calories: 0, protein: 0, carbs: 0, fat: 0 },
      ),
    [selectedIngredients],
  );

  const hasSelectedIngredients = selectedIngredients.length > 0;
  const baseCalories = watch("calories");
  const totalCalories =
    (Number.isFinite(baseCalories) ? baseCalories : 0) + nutritionTotals.calories;

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    reset({
      name: initialData?.name ?? "",
      type: initialData?.type ?? "",
      calories: initialData?.calories ?? 0,
      protein: initialData?.protein ?? 0,
      carbs: initialData?.carbs ?? 0,
      fat: initialData?.fat ?? 0,
      ingredients: [],
    });
    setSelectedIngredients([]);
  }, [isOpen, initialData, reset]);

  const handleClose = () => {
    reset();
    setSelectedIngredients([]);
    onClose();
  };

  const onSubmitForm = async (data: FoodCreateDto) => {
    try {
      await onSubmit({
        name: data.name.trim(),
        type: data.type.trim(),
        calories: data.calories,
        protein: data.protein,
        carbs: data.carbs,
        fat: data.fat,
        ingredients: selectedIngredients.map((item) => ({
          ingredientId: item.ingredient.id,
          quantity: item.quantity,
        })),
      });
      reset();
    } catch (error) {
      console.error("Form submission error:", error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gradient-to-br from-slate-800 to-slate-900 border border-white/20 rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-6 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white">
            {initialData ? "Edit Food" : "Add New Food"}
          </h2>
          <button
            onClick={handleClose}
            className="text-white/80 hover:text-white transition-colors disabled:opacity-50"
            disabled={isLoading}
          >
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmitForm)} className="food-form p-8">
          <div className="mt-8">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Name
            </label>
            <input
              {...register("name", {
                required: "Name is required",
                validate: (value) =>
                  value.trim().length > 0 || "Name cannot be empty",
              })}
              type="text"
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 text-base transition-all"
              placeholder="e.g., Grilled Chicken"
              disabled={isLoading}
            />
            {errors.name && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors.name.message}
              </span>
            )}
          </div>

          <div className="mt-8">
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Food Type
            </label>
            <input
              {...register("type", {
                required: "Type is required",
                validate: (value) =>
                  value.trim().length > 0 || "Type cannot be empty",
              })}
              type="text"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., Protein, Vegetable"
              disabled={isLoading}
            />
            {errors.type && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors.type.message}
              </span>
            )}
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-300 mb-2">
              Calories
            </label>
            <input
              {...register("calories", {
                valueAsNumber: true,
                validate: (value) => {
                  if (!Number.isFinite(value)) {
                    return "Calories is required";
                  }

                  return value > 0 || "Calories must be greater than 0";
                },
              })}
              type="number"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg focus:outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30 text-white placeholder-gray-400 transition-all"
              placeholder="e.g., 250"
              disabled={isLoading}
            />
            {errors.calories && (
              <span className="text-red-400 text-xs mt-1 block">
                {errors.calories.message}
              </span>
            )}
          </div>

          <FoodIngredientsPicker
            selected={selectedIngredients}
            onChange={setSelectedIngredients}
            disabled={isLoading}
          />

          <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 mt-5">
            {[
              ['Calories', `${Math.round(totalCalories)} kcal`],
              ['Protein', `${hasSelectedIngredients ? nutritionTotals.protein.toFixed(1) : '0.0'}g`],
              ['Carbs', `${hasSelectedIngredients ? nutritionTotals.carbs.toFixed(1) : '0.0'}g`],
              ['Fat', `${hasSelectedIngredients ? nutritionTotals.fat.toFixed(1) : '0.0'}g`],
            ].map(([label, value]) => (
              <div key={label} className="rounded-lg border border-white/10 bg-white/[0.04] p-3">
                <p className="text-xs text-gray-400">{label}</p>
                <p className="text-sm font-semibold text-white">{value}</p>
              </div>
            ))}
          </div>

          <div className="flex space-x-3 pt-6">
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 px-4 py-3 border border-white/20 hover:border-white/40 text-gray-300 hover:text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium"
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="flex-1 px-4 py-3 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white rounded-lg transition-all duration-200 disabled:opacity-50 font-medium shadow-lg hover:shadow-purple-500/50"
              disabled={isLoading}
            >
              {isLoading ? (
                <span className="flex items-center justify-center">
                  <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2"></span>
                  Saving...
                </span>
              ) : initialData ? (
                "Update Food"
              ) : (
                "Create Food"
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
