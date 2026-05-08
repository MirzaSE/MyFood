import type { Ingredient } from "../types/ingredient";

interface IngredientListProps {
    ingredients: Ingredient[];
}

export default function IngredientList({
                                           ingredients,
                                       }: IngredientListProps) {
    if (ingredients.length === 0) {
        return (
            <div className="bg-slate-800 rounded-xl border border-slate-700 p-10 text-center">
                <p className="text-gray-400">
                    No ingredients found.
                </p>
            </div>
        );
    }

    return (
        <div className="bg-slate-800 rounded-xl border border-slate-700 overflow-hidden">
            <table className="w-full">
                <thead className="bg-slate-700">
                <tr>
                    <th className="text-left px-6 py-4 text-gray-200">
                        ID
                    </th>

                    <th className="text-left px-6 py-4 text-gray-200">
                        Ingredient Name
                    </th>
                </tr>
                </thead>

                <tbody>
                {ingredients.map((ingredient) => (
                    <tr
                        key={ingredient.id}
                        className="border-t border-slate-700 hover:bg-slate-700/50 transition-colors"
                    >
                        <td className="px-6 py-4 text-gray-300">
                            {ingredient.id}
                        </td>

                        <td className="px-6 py-4 text-white">
                            {ingredient.name}
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
}