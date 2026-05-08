import { useState } from "react";

interface Props {
    initialData?: any;
    onSubmit: (data: any) => void;
    onCancel: () => void;
}

export default function IngredientForm({
                                           initialData,
                                           onSubmit,
                                           onCancel
                                       }: Props) {

    const [form, setForm] = useState({
        name: initialData?.name || "",
        unit: initialData?.unit || "",
        caloriesPerUnit: initialData?.caloriesPerUnit || 0,
        protein: initialData?.protein || 0,
        carbs: initialData?.carbs || 0,
        fat: initialData?.fat || 0
    });

    const [error, setError] = useState("");

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        if (!form.name || !form.unit) {
            setError("Name and unit are required");
            return;
        }

        if (
            form.caloriesPerUnit < 0 ||
            form.protein < 0 ||
            form.carbs < 0 ||
            form.fat < 0
        ) {
            setError("Values must be positive");
            return;
        }

        onSubmit(form);
    };

    return (
        <form onSubmit={handleSubmit}>

            <h2>Ingredient Form</h2>

            {error && (
                <p style={{ color: "red" }}>
                    {error}
                </p>
            )}

            <input
                placeholder="Name"
                value={form.name}
                onChange={(e) =>
                    setForm({
                        ...form,
                        name: e.target.value
                    })
                }
            />

            <input
                placeholder="Unit"
                value={form.unit}
                onChange={(e) =>
                    setForm({
                        ...form,
                        unit: e.target.value
                    })
                }
            />

            <input
                type="number"
                placeholder="Calories"
                value={form.caloriesPerUnit}
                onChange={(e) =>
                    setForm({
                        ...form,
                        caloriesPerUnit: Number(e.target.value)
                    })
                }
            />

            <input
                type="number"
                placeholder="Protein"
                value={form.protein}
                onChange={(e) =>
                    setForm({
                        ...form,
                        protein: Number(e.target.value)
                    })
                }
            />

            <input
                type="number"
                placeholder="Carbs"
                value={form.carbs}
                onChange={(e) =>
                    setForm({
                        ...form,
                        carbs: Number(e.target.value)
                    })
                }
            />

            <input
                type="number"
                placeholder="Fat"
                value={form.fat}
                onChange={(e) =>
                    setForm({
                        ...form,
                        fat: Number(e.target.value)
                    })
                }
            />

            <div style={{ marginTop: 10 }}>
                <button type="submit">
                    Submit
                </button>

                <button
                    type="button"
                    onClick={onCancel}
                    style={{ marginLeft: 10 }}
                >
                    Cancel
                </button>
            </div>

        </form>
    );
}