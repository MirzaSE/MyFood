import React, { useEffect } from "react";
import { useForm } from "react-hook-form";
import type {
  IngredientCreateDto,
  IngredientUpdateDto,
  Ingredient,
} from "../types";

interface IngredientFormProps {
  onSubmit: (data: IngredientCreateDto | IngredientUpdateDto) => void;
  onCancel: () => void;
  initialData?: Ingredient;
  loading?: boolean;
}

export const IngredientForm: React.FC<IngredientFormProps> = ({
  onSubmit,
  onCancel,
  initialData,
  loading = false,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<IngredientCreateDto>({
    defaultValues: {
      name: initialData?.name || "",
      unit: initialData?.unit || "",
      caloriesPerUnit:
        initialData?.caloriesPerUnit || (undefined as unknown as number),
      protein: initialData?.protein || (undefined as unknown as number),
      carbs: initialData?.carbs || (undefined as unknown as number),
      fat: initialData?.fat || (undefined as unknown as number),
      foodId: initialData?.foodId || 1,
    },
  });

  useEffect(() => {
    if (initialData) {
      reset({
        name: initialData.name,
        unit: initialData.unit || "",
        caloriesPerUnit:
          initialData.caloriesPerUnit || (undefined as unknown as number),
        protein: initialData.protein || (undefined as unknown as number),
        carbs: initialData.carbs || (undefined as unknown as number),
        fat: initialData.fat || (undefined as unknown as number),
        foodId: initialData.foodId || 1,
      });
    }
  }, [initialData, reset]);

  const onSubmitForm = (data: IngredientCreateDto) => {
    onSubmit({
      ...data,
      foodId: initialData?.foodId || data.foodId || 1,
    });
  };

  return (
    <form onSubmit={handleSubmit(onSubmitForm)} className="space-y-5">
      <div className="grid gap-5 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <label className="mb-2 block text-sm font-semibold text-gray-300">
            Ingredient Name
          </label>
          <input
            {...register("name", { required: "Name is required" })}
            type="text"
            className="w-full rounded-lg border border-white/20 bg-white/10 px-4 py-3 text-white placeholder-gray-400 transition-all focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/30"
            placeholder="e.g., Tomato"
            disabled={loading}
          />
          {errors.name && (
            <span className="mt-1 block text-xs text-red-400">
              {errors.name.message}
            </span>
          )}
        </div>

        <div className="sm:col-span-2">
          <label className="mb-2 block text-sm font-semibold text-gray-300">
            Unit
          </label>
          <input
            {...register("unit", { required: "Unit is required" })}
            type="text"
            className="w-full rounded-lg border border-white/20 bg-white/10 px-4 py-3 text-white placeholder-gray-400 transition-all focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/30"
            placeholder="e.g., cup, tsp, gram"
            disabled={loading}
          />
          {errors.unit && (
            <span className="mt-1 block text-xs text-red-400">
              {errors.unit.message}
            </span>
          )}
        </div>

        <div>
          <label className="mb-2 block text-sm font-semibold text-gray-300">
            Calories Per Unit
          </label>
          <input
            {...register("caloriesPerUnit", {
              required: "Calories per unit is required",
              valueAsNumber: true,
              min: { value: 0.0001, message: "Must be greater than 0" },
            })}
            type="number"
            step="0.01"
            className="w-full rounded-lg border border-white/20 bg-white/10 px-4 py-3 text-white placeholder-gray-400 transition-all focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/30"
            placeholder="e.g., 45"
            disabled={loading}
          />
          {errors.caloriesPerUnit && (
            <span className="mt-1 block text-xs text-red-400">
              {errors.caloriesPerUnit.message}
            </span>
          )}
        </div>

        <div>
          <label className="mb-2 block text-sm font-semibold text-gray-300">
            Protein
          </label>
          <input
            {...register("protein", {
              required: "Protein is required",
              valueAsNumber: true,
              min: { value: 0.0001, message: "Must be greater than 0" },
            })}
            type="number"
            step="0.01"
            className="w-full rounded-lg border border-white/20 bg-white/10 px-4 py-3 text-white placeholder-gray-400 transition-all focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/30"
            placeholder="e.g., 2.5"
            disabled={loading}
          />
          {errors.protein && (
            <span className="mt-1 block text-xs text-red-400">
              {errors.protein.message}
            </span>
          )}
        </div>

        <div>
          <label className="mb-2 block text-sm font-semibold text-gray-300">
            Carbs
          </label>
          <input
            {...register("carbs", {
              required: "Carbs is required",
              valueAsNumber: true,
              min: { value: 0.0001, message: "Must be greater than 0" },
            })}
            type="number"
            step="0.01"
            className="w-full rounded-lg border border-white/20 bg-white/10 px-4 py-3 text-white placeholder-gray-400 transition-all focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/30"
            placeholder="e.g., 12"
            disabled={loading}
          />
          {errors.carbs && (
            <span className="mt-1 block text-xs text-red-400">
              {errors.carbs.message}
            </span>
          )}
        </div>

        <div>
          <label className="mb-2 block text-sm font-semibold text-gray-300">
            Fat
          </label>
          <input
            {...register("fat", {
              required: "Fat is required",
              valueAsNumber: true,
              min: { value: 0.0001, message: "Must be greater than 0" },
            })}
            type="number"
            step="0.01"
            className="w-full rounded-lg border border-white/20 bg-white/10 px-4 py-3 text-white placeholder-gray-400 transition-all focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500/30"
            placeholder="e.g., 0.8"
            disabled={loading}
          />
          {errors.fat && (
            <span className="mt-1 block text-xs text-red-400">
              {errors.fat.message}
            </span>
          )}
        </div>
      </div>

      <div className="flex gap-3 pt-3">
        <button
          type="button"
          onClick={onCancel}
          disabled={loading}
          className="flex-1 rounded-lg border border-white/20 px-4 py-3 font-medium text-gray-200 transition-all hover:border-white/40 hover:text-white disabled:opacity-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={loading}
          className="flex-1 rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-4 py-3 font-medium text-white shadow-lg transition-all hover:from-purple-700 hover:to-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {loading ? "Saving..." : "Save Ingredient"}
        </button>
      </div>
    </form>
  );
};
