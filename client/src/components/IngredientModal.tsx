import React, { useState } from "react";
import type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
} from "../types";
import { IngredientForm } from "./IngredientForm";
import { X } from "lucide-react";

interface IngredientModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: IngredientCreateDto | IngredientUpdateDto) => Promise<void>;
  initialData?: Ingredient;
  title?: string;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  title = initialData ? "Edit Ingredient" : "Create Ingredient",
}) => {
  const [loading, setLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const handleSubmit = async (
    data: IngredientCreateDto | IngredientUpdateDto,
  ) => {
    try {
      setLoading(true);
      setErrorMessage("");
      setSuccessMessage("");

      await onSubmit(data);

      setSuccessMessage(
        initialData
          ? "Ingredient updated successfully!"
          : "Ingredient created successfully!",
      );
      setTimeout(() => {
        onClose();
        setSuccessMessage("");
      }, 1500);
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "An error occurred";
      setErrorMessage(message);
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4 backdrop-blur-sm">
      <div className="w-full max-w-2xl overflow-hidden rounded-2xl border border-white/20 bg-gradient-to-br from-slate-800 to-slate-900 shadow-2xl">
        <div className="flex items-center justify-between bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-5">
          <h2 className="text-xl font-bold text-white">{title}</h2>
          <button
            onClick={onClose}
            className="text-white/80 transition-colors hover:text-white disabled:opacity-50"
            disabled={loading}
          >
            <X size={24} />
          </button>
        </div>

        <div className="px-6 py-6 sm:px-8">
          <p className="mb-5 text-sm text-gray-300">
            Fill in the ingredient details below. Required fields are marked by
            validation.
          </p>

          {successMessage && (
            <div className="mb-4 rounded-lg border border-green-500/40 bg-green-500/10 p-3 text-sm text-green-200">
              {successMessage}
            </div>
          )}

          {errorMessage && (
            <div className="mb-4 rounded-lg border border-red-500/40 bg-red-500/10 p-3 text-sm text-red-200">
              {errorMessage}
            </div>
          )}

          <IngredientForm
            onSubmit={handleSubmit}
            onCancel={onClose}
            initialData={initialData}
            loading={loading}
          />
        </div>
      </div>
    </div>
  );
};
