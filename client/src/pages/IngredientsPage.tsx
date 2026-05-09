import React, { useState } from 'react';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingIngredient, setEditingIngredient] = useState<Ingredient | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    const handleCreate = () => {
        setEditingIngredient(null);
        setIsModalOpen(true);
    };

    const handleEdit = (ingredient: Ingredient) => {
        setEditingIngredient(ingredient);
        setIsModalOpen(true);
    };

    const handleDelete = async (id: number) => {
        if (confirm('Are you sure you want to delete this ingredient?')) {
            try {
                await ingredientService.delete(id);
                setRefreshTrigger((prev: number) => prev + 1);
            } catch (error) {
                console.error('Failed to delete ingredient', error);
            }
        }
    };

   const handleSubmit = async (data: IngredientCreateDto) => {
    console.log('Updating with data:', data);  // ← ADD
    setIsLoading(true);
    try {
        if (editingIngredient) {
            console.log('Updating ingredient ID:', editingIngredient.id);  // ← ADD
            await ingredientService.update(editingIngredient.id, data);
            console.log('Update successful');  // ← ADD
        } else {
            await ingredientService.create(data);
        }
        setIsModalOpen(false);
        setRefreshTrigger((prev: number) => prev + 1);
    } catch (error) {
        console.error('Failed to save ingredient:', error);
    } finally {
        setIsLoading(false);
    }
};

    return (
        <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900">
            <div className="container mx-auto px-6 py-8">
                <div className="flex justify-between items-center mb-6">
                    <h1 className="text-3xl font-bold text-white">Ingredients</h1>
                    <button
                        onClick={handleCreate}
                        className="px-4 py-2 bg-gradient-to-r from-purple-500 to-blue-500 text-white rounded-lg hover:from-purple-600 hover:to-blue-600 transition-all duration-200 shadow-lg"
                    >
                        + New Ingredient
                    </button>
                </div>

                <IngredientList
                    onEdit={handleEdit}
                    onDelete={handleDelete}
                    refreshTrigger={refreshTrigger}
                />

                <IngredientModal
                    isOpen={isModalOpen}
                    onClose={() => setIsModalOpen(false)}
                    onSubmit={handleSubmit}
                    initialData={editingIngredient}
                    isLoading={isLoading}
                />
            </div>
        </div>
    );
};