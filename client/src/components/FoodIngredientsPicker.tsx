import React, { useEffect, useState } from 'react';
import { ingredientService } from '../services/ingredientService';
import type { Ingredient } from '../types/ingredient';
import { Plus, Trash2 } from 'lucide-react';

interface SelectedIngredient {
    ingredientId: number;
    name: string;
    unit: string;
    caloriesPerUnit: number;
    protein: number;
    carbs: number;
    fat: number;
    quantity: number;
    totalCalories: number;
    totalProtein: number;
    totalCarbs: number;
    totalFat: number;
}

interface FoodIngredientsPickerProps {
    onIngredientsChange: (ingredients: SelectedIngredient[], totalNutrition: {
        calories: number;
        protein: number;
        carbs: number;
        fat: number;
    }) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
    onIngredientsChange
}) => {
    const [allIngredients, setAllIngredients] = useState<Ingredient[]>([]);
    const [selectedIngredients, setSelectedIngredients] = useState<SelectedIngredient[]>([]);
    const [selectedIngredientId, setSelectedIngredientId] = useState<number>(0);
    const [quantity, setQuantity] = useState<number>(100);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadIngredients();
    }, []);

    useEffect(() => {
        // Calculate total nutrition
        const totals = selectedIngredients.reduce(
            (acc, ing) => ({
                calories: acc.calories + ing.totalCalories,
                protein: acc.protein + ing.totalProtein,
                carbs: acc.carbs + ing.totalCarbs,
                fat: acc.fat + ing.totalFat
            }),
            { calories: 0, protein: 0, carbs: 0, fat: 0 }
        );
        onIngredientsChange(selectedIngredients, totals);
    }, [selectedIngredients, onIngredientsChange]);

    const loadIngredients = async () => {
        setLoading(true);
        try {
            const data = await ingredientService.getAll();
            setAllIngredients(data);
        } catch (error) {
            console.error('Failed to load ingredients', error);
        } finally {
            setLoading(false);
        }
    };

    const addIngredient = () => {
        if (selectedIngredientId === 0) return;

        const ingredient = allIngredients.find(i => i.id === selectedIngredientId);
        if (!ingredient) return;

        const existing = selectedIngredients.find(i => i.ingredientId === ingredient.id);
        if (existing) {
            // Update quantity if already exists
            updateQuantity(ingredient.id, existing.quantity + quantity);
        } else {
            // Add new ingredient
            const newIngredient: SelectedIngredient = {
                ingredientId: ingredient.id,
                name: ingredient.name,
                unit: ingredient.unit,
                caloriesPerUnit: ingredient.caloriesPerUnit,
                protein: ingredient.protein,
                carbs: ingredient.carbs,
                fat: ingredient.fat,
                quantity: quantity,
                totalCalories: (ingredient.caloriesPerUnit * quantity) / 100,
                totalProtein: (ingredient.protein * quantity) / 100,
                totalCarbs: (ingredient.carbs * quantity) / 100,
                totalFat: (ingredient.fat * quantity) / 100
            };
            setSelectedIngredients([...selectedIngredients, newIngredient]);
        }
        setSelectedIngredientId(0);
        setQuantity(100);
    };

    const updateQuantity = (ingredientId: number, newQuantity: number) => {
        setSelectedIngredients(prev =>
            prev.map(ing => {
                if (ing.ingredientId !== ingredientId) return ing;
                return {
                    ...ing,
                    quantity: newQuantity,
                    totalCalories: (ing.caloriesPerUnit * newQuantity) / 100,
                    totalProtein: (ing.protein * newQuantity) / 100,
                    totalCarbs: (ing.carbs * newQuantity) / 100,
                    totalFat: (ing.fat * newQuantity) / 100
                };
            })
        );
    };

    const removeIngredient = (ingredientId: number) => {
        setSelectedIngredients(prev => prev.filter(i => i.ingredientId !== ingredientId));
    };

    if (loading) {
        return <div className="text-center py-4 text-gray-400">Loading ingredients...</div>;
    }

    return (
        <div className="bg-white/5 backdrop-blur-sm rounded-xl p-4 border border-white/10">
            <h3 className="text-lg font-semibold text-white mb-4">Ingredients</h3>

            {/* Add Ingredient Section */}
            <div className="flex flex-wrap gap-3 mb-6">
                <select
                    value={selectedIngredientId}
                    onChange={(e) => setSelectedIngredientId(Number(e.target.value))}
                    className="flex-1 px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                >
                    <option value={0}>Select ingredient...</option>
                    {allIngredients.map(ing => (
                        <option key={ing.id} value={ing.id}>
                            {ing.name} ({ing.caloriesPerUnit} cal/{ing.unit})
                        </option>
                    ))}
                </select>

                <input
                    type="number"
                    value={quantity}
                    onChange={(e) => setQuantity(Number(e.target.value))}
                    className="w-32 px-3 py-2 bg-slate-800/50 border border-white/10 rounded-lg text-white focus:outline-none focus:border-purple-500"
                    placeholder="g/ml"
                />

                <button
                    type="button"
                    onClick={addIngredient}
                    disabled={selectedIngredientId === 0}
                    className="px-4 py-2 bg-purple-500 hover:bg-purple-600 disabled:opacity-50 text-white rounded-lg transition flex items-center gap-2"
                >
                    <Plus size={16} />
                    Add
                </button>
            </div>

            {/* Selected Ingredients List */}
            {selectedIngredients.length === 0 ? (
                <p className="text-gray-400 text-center py-4">No ingredients added yet</p>
            ) : (
                <div className="space-y-2">
                    {selectedIngredients.map(ing => (
                        <div key={ing.ingredientId} className="flex items-center justify-between p-3 bg-slate-800/30 rounded-lg border border-white/5">
                            <div className="flex-1">
                                <div className="flex items-center gap-3">
                                    <span className="font-medium text-white">{ing.name}</span>
                                    <span className="text-sm text-gray-400">{ing.unit}</span>
                                </div>
                                <div className="text-xs text-gray-400 mt-1">
                                    {ing.caloriesPerUnit} cal/{ing.unit} | P: {ing.protein}g | C: {ing.carbs}g | F: {ing.fat}g
                                </div>
                            </div>

                            <div className="flex items-center gap-3">
                                <input
                                    type="number"
                                    value={ing.quantity}
                                    onChange={(e) => updateQuantity(ing.ingredientId, Number(e.target.value))}
                                    className="w-20 px-2 py-1 bg-slate-800 border border-white/10 rounded text-white text-sm"
                                />
                                <div className="text-right min-w-[100px]">
                                    <div className="text-sm text-yellow-400">{Math.round(ing.totalCalories)} cal</div>
                                    <div className="text-xs text-gray-400">
                                        P:{Math.round(ing.totalProtein)} C:{Math.round(ing.totalCarbs)} F:{Math.round(ing.totalFat)}
                                    </div>
                                </div>
                                <button
                                    type="button"
                                    onClick={() => removeIngredient(ing.ingredientId)}
                                    className="p-1 text-red-400 hover:text-red-300 transition"
                                >
                                    <Trash2 size={18} />
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};