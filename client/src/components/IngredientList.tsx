import React, { useEffect, useState } from 'react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';
import { Edit, Trash2, Search } from 'lucide-react';

interface IngredientListProps {
    onEdit: (ingredient: Ingredient) => void;
    onDelete: (id: number) => void;
    refreshTrigger: number;
}

export const IngredientList: React.FC<IngredientListProps> = ({ onEdit, onDelete, refreshTrigger }) => {
    const [ingredients, setIngredients] = useState<Ingredient[]>([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState('');

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

    const filteredIngredients = ingredients.filter(i =>
        i.name.toLowerCase().includes(search.toLowerCase())
    );

    if (loading) {
        return (
            <div className="flex justify-center items-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-500"></div>
            </div>
        );
    }

    return (
        <div className="bg-white/5 backdrop-blur-sm rounded-xl overflow-hidden border border-white/10">
            <div className="p-4 border-b border-white/10">
                <div className="relative">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={18} />
                    <input
                        type="text"
                        placeholder="Search ingredients..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        className="w-full pl-10 pr-4 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500 transition-colors"
                    />
                </div>
            </div>

            <div className="overflow-x-auto">
                <table className="w-full">
                    <thead className="bg-slate-800/50">
                        <tr>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">ID</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Name</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Quantity</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Food ID</th>
                            <th className="px-6 py-3 text-right text-xs font-medium text-gray-300 uppercase tracking-wider">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-white/10">
                        {filteredIngredients.map((ing) => (
                            <tr key={ing.id} className="hover:bg-white/5 transition-colors">
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{ing.id}</td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-white">{ing.name}</td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{ing.quantity}</td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{ing.foodId}</td>
                                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-3">
                                    <button
                                        onClick={() => onEdit(ing)}
                                        className="text-blue-400 hover:text-blue-300 transition-colors inline-flex items-center space-x-1"
                                    >
                                        <Edit size={16} />
                                        <span>Edit</span>
                                    </button>
                                    <button
                                        onClick={() => onDelete(ing.id)}
                                        className="text-red-400 hover:text-red-300 transition-colors inline-flex items-center space-x-1"
                                    >
                                        <Trash2 size={16} />
                                        <span>Delete</span>
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {filteredIngredients.length === 0 && (
                <div className="text-center py-12">
                    <p className="text-gray-400">No ingredients found</p>
                </div>
            )}
        </div>
    );
};