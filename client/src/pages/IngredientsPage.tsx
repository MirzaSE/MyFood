import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Navbar } from "../components/Navbar";
import IngredientList from "../components/IngredientList";
import { ingredientService } from "../services/ingredientService";
import type { Ingredient } from "../types/ingredient";

export default function IngredientsPage() {
    const [ingredients, setIngredients] = useState<Ingredient[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadIngredients();
    }, []);

    const loadIngredients = async () => {
        try {
            setLoading(true);
            setError(null);

            const data = await ingredientService.getAll();

            setIngredients(data);
        } catch (err: any) {
            setError(
                err.response?.data?.message ||
                "Failed to load ingredients"
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
            <Navbar />

            <div className="container mx-auto px-6 py-16">
                {error && (
                    <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
                        <AlertCircle
                            size={20}
                            className="text-red-400 flex-shrink-0 mt-0.5"
                        />

                        <p className="text-red-200">{error}</p>
                    </div>
                )}

                <div className="mb-24">
                    <div className="flex flex-col gap-3">
                        <h1 className="text-5xl font-bold text-white tracking-tight">
                            Ingredients
                        </h1>

                        <p className="text-gray-400 text-lg">
                            Manage your ingredient collection
                        </p>

                        <div className="mt-3">
                            <div className="inline-flex items-center gap-2 px-5 py-2 rounded-full bg-gradient-to-r from-purple-600/20 to-blue-600/20 border border-purple-500/30 backdrop-blur">
                                <div className="w-2 h-2 rounded-full bg-green-400"></div>

                                <span className="text-sm font-medium text-gray-200">
            {ingredients.length}{" "}
                                    {ingredients.length === 1
                                        ? "ingredient"
                                        : "ingredients"}
        </span>
                            </div>
                        </div>
                    </div>
                </div>

                {loading ? (
                    <div className="flex flex-col items-center justify-center py-20">
                        <div className="relative w-16 h-16 mb-4">
                            <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>

                            <div className="absolute inset-2 bg-slate-900 rounded-full"></div>
                        </div>

                        <p className="text-gray-300 font-medium">
                            Loading ingredients...
                        </p>
                    </div>
                ) : (
                    <div className="pt-10">
                        <IngredientList ingredients={ingredients} />
                    </div>
                )}
            </div>
        </div>
    );
}