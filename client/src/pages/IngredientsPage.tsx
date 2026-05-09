import React, { useState } from "react";
import type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
} from "../types";
import { ingredientService } from "../services/ingredientService";
import { IngredientList } from "../components/IngredientList";
import { IngredientModal } from "../components/IngredientModal";
import { Navbar } from "../components/Navbar";

export const IngredientsPage: React.FC = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedIngredient, setSelectedIngredient] = useState<
    Ingredient | undefined
  >();
  const [ingredientDrafts, setIngredientDrafts] = useState<
    Record<number, Ingredient>
  >({});
  const [refreshList, setRefreshList] = useState(false);
  const [error, setError] = useState("");

  const handleOpenModal = () => {
    setSelectedIngredient(undefined);
    setIsModalOpen(true);
  };

  const handleEditIngredient = (ingredient: Ingredient) => {
    setSelectedIngredient({
      ...ingredient,
      ...(ingredientDrafts[ingredient.id] ?? {}),
    });
    setIsModalOpen(true);
  };

  const handleSubmitForm = async (
    data: IngredientCreateDto | IngredientUpdateDto,
  ) => {
    try {
      if (selectedIngredient) {
        const updatedIngredient = await ingredientService.updateIngredient(
          selectedIngredient.id,
          data as IngredientUpdateDto,
        );
        setIngredientDrafts((previous) => ({
          ...previous,
          [selectedIngredient.id]: {
            ...selectedIngredient,
            ...data,
            ...updatedIngredient,
          },
        }));
      } else {
        const createdIngredient = await ingredientService.createIngredient(
          data as IngredientCreateDto,
        );
        if (createdIngredient?.id) {
          setIngredientDrafts((previous) => ({
            ...previous,
            [createdIngredient.id]: {
              ...createdIngredient,
              ...data,
            },
          }));
        }
      }
      setIsModalOpen(false);
      setSelectedIngredient(undefined);
      setRefreshList(true);
    } catch (err) {
      const message = err instanceof Error ? err.message : "An error occurred";
      setError(message);
      throw new Error(message);
    }
  };

  const handleDeleteIngredient = async (id: number) => {
    if (confirm("Are you sure you want to delete this ingredient?")) {
      try {
        await ingredientService.deleteIngredient(id);
        setRefreshList(true);
      } catch (err) {
        const message =
          err instanceof Error ? err.message : "Failed to delete ingredient";
        setError(message);
      }
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-950">
      <Navbar />

      <div className="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-4xl font-bold text-white">Ingredients</h1>
            <p className="mt-2 text-gray-400">
              Manage ingredients with the same workflow as foods.
            </p>
          </div>
          <button
            onClick={handleOpenModal}
            className="rounded-lg bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-3 font-semibold text-white shadow-lg transition-all hover:from-purple-700 hover:to-blue-700 hover:shadow-purple-500/30"
          >
            New Ingredient
          </button>
        </div>

        {error && (
          <div className="mb-4 rounded-lg border border-red-500/40 bg-red-500/10 p-4 text-red-200 shadow-lg shadow-red-950/20">
            {error}
            <button
              onClick={() => setError("")}
              className="ml-2 text-sm underline underline-offset-2"
            >
              Dismiss
            </button>
          </div>
        )}

        <div className="rounded-2xl border border-white/10 bg-white/95 p-6 shadow-2xl shadow-black/20 backdrop-blur">
          <IngredientList
            onEdit={handleEditIngredient}
            onDelete={handleDeleteIngredient}
            refresh={refreshList}
            onRefreshComplete={() => setRefreshList(false)}
          />
        </div>
      </div>

      <IngredientModal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setSelectedIngredient(undefined);
        }}
        onSubmit={handleSubmitForm}
        initialData={selectedIngredient}
        title={selectedIngredient ? "Edit Ingredient" : "Create Ingredient"}
      />
    </div>
  );
};
