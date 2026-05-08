import IngredientForm from "./IngredientForm";

interface Props {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: any) => void;
    initialData?: any;
}

export default function IngredientModal({
                                            isOpen,
                                            onClose,
                                            onSubmit,
                                            initialData
                                        }: Props) {

    if (!isOpen) return null;

    return (
        <div
            style={{
                position: "fixed",
                inset: 0,
                background: "rgba(0,0,0,0.5)",
                display: "flex",
                justifyContent: "center",
                alignItems: "center"
            }}
        >
            <div
                style={{
                    background: "white",
                    padding: 20,
                    borderRadius: 10,
                    width: 400
                }}
            >
                <IngredientForm
                    initialData={initialData}
                    onSubmit={onSubmit}
                    onCancel={onClose}
                />
            </div>
        </div>
    );
}