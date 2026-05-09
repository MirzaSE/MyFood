import React, { useEffect, useState } from 'react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';
import { Edit, Trash2, Search, ChevronLeft, ChevronRight } from 'lucide-react';

interface IngredientListProps {
    onEdit: (ingredient: Ingredient) => void;
    onDelete: (id: number) => void;
    refreshTrigger: number;
}

export const IngredientList: React.FC<IngredientListProps> = ({ onEdit, onDelete, refreshTrigger }) => {
    const [ingredients, setIngredients] = useState<Ingredient[]>([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 10;

    useEffect(() => {
        loadIngredients();
    }, [refreshTrigger]);

    const loadIngredients = async () => {
        setLoading(true);
        try {
            const data = await ingredientService.getAll();
            setIngredients(data);
        } catch (error) {
            console.error('Failed to load ingredients', error);
        } finally {
            setLoading(false);
        }
    };

    // Filter ingredients by search
    const filteredIngredients = ingredients.filter(i =>
        i.name.toLowerCase().includes(search.toLowerCase())
    );

    // Pagination logic
    const totalPages = Math.ceil(filteredIngredients.length / itemsPerPage);
    const paginatedIngredients = filteredIngredients.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    // Reset to page 1 when search changes
    useEffect(() => {
        setCurrentPage(1);
    }, [search]);

    // Calculate total calories for an ingredient
    const getTotalCalories = (ing: Ingredient) => {
        return (ing.caloriesPerUnit * ing.quantity).toFixed(1);
    };

    if (loading) {
        return (
            <div className="flex justify-center items-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-500"></div>
            </div>
        );
    }

    return (
        <div className="bg-white/5 backdrop-blur-sm rounded-xl overflow-hidden border border-white/10">
            {/* Search Bar */}
            <div className="p-4 border-b border-white/10">
                <div className="relative">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={18} />
                    <input
                        type="text"
                        placeholder="Search ingredients by name..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        className="w-full pl-10 pr-4 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 transition-colors"
                    />
                </div>
            </div>

            {/* Table */}
            <div className="overflow-x-auto">
                <table className="w-full">
                    <thead className="bg-slate-800/50">
                        <tr>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Name</th>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Unit</th>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Cal/Unit</th>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Protein</th>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Carbs</th>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Fat</th>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Quantity</th>
                            <th className="px-4 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Total Cal</th>
                            <th className="px-4 py-3 text-right text-xs font-medium text-gray-300 uppercase tracking-wider">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-white/10">
                        {paginatedIngredients.map((ing) => (
                            <tr key={ing.id} className="hover:bg-white/5 transition-colors">
                                <td className="px-4 py-3 text-sm font-medium text-white">{ing.name}</td>
                                <td className="px-4 py-3 text-sm text-gray-300">{ing.unit}</td>
                                <td className="px-4 py-3 text-sm text-gray-300">{ing.caloriesPerUnit}</td>
                                <td className="px-4 py-3 text-sm text-gray-300">{ing.protein}g</td>
                                <td className="px-4 py-3 text-sm text-gray-300">{ing.carbs}g</td>
                                <td className="px-4 py-3 text-sm text-gray-300">{ing.fat}g</td>
                                <td className="px-4 py-3 text-sm text-gray-300">{ing.quantity} {ing.unit}</td>
                                <td className="px-4 py-3 text-sm text-yellow-400 font-medium">{getTotalCalories(ing)} cal</td>
                                <td className="px-4 py-3 text-right space-x-2">
                                    <button
                                        onClick={() => onEdit(ing)}
                                        className="text-blue-400 hover:text-blue-300 transition-colors inline-flex items-center gap-1"
                                    >
                                        <Edit size={16} />
                                        <span className="text-sm">Edit</span>
                                    </button>
                                    <button
                                        onClick={() => onDelete(ing.id)}
                                        className="text-red-400 hover:text-red-300 transition-colors inline-flex items-center gap-1"
                                    >
                                        <Trash2 size={16} />
                                        <span className="text-sm">Delete</span>
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {/* Empty State */}
            {filteredIngredients.length === 0 && (
                <div className="text-center py-12">
                    <p className="text-gray-400">
                        {search ? 'No ingredients match your search' : 'No ingredients found'}
                    </p>
                </div>
            )}

            {/* Pagination */}
            {totalPages > 1 && (
                <div className="flex justify-center items-center space-x-4 p-4 border-t border-white/10">
                    <button
                        onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                        disabled={currentPage === 1}
                        className="flex items-center gap-1 px-3 py-1 bg-slate-700 hover:bg-slate-600 rounded-lg text-gray-300 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                    >
                        <ChevronLeft size={16} />
                        Previous
                    </button>
                    <span className="text-gray-300">
                        Page {currentPage} of {totalPages}
                    </span>
                    <button
                        onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                        disabled={currentPage === totalPages}
                        className="flex items-center gap-1 px-3 py-1 bg-slate-700 hover:bg-slate-600 rounded-lg text-gray-300 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                    >
                        Next
                        <ChevronRight size={16} />
                    </button>
                </div>
            )}
        </div>
    );
};