import React from 'react';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';
import { IngredientForm } from './IngredientForm';
import { X } from 'lucide-react';

interface IngredientModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSubmit: (data: IngredientCreateDto) => void;
    initialData?: Ingredient | null;
    isLoading: boolean;
}

export const IngredientModal: React.FC<IngredientModalProps> = ({
    isOpen,
    onClose,
    onSubmit,
    initialData,
    isLoading
}) => {
    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 bg-black/70 flex items-center justify-center z-50 backdrop-blur-sm">
            <div className="bg-gradient-to-br from-slate-800 to-slate-900 rounded-2xl p-6 w-full max-w-md border border-white/20 shadow-2xl">
                <div className="flex justify-between items-center mb-6">
                    <h2 className="text-2xl font-bold bg-gradient-to-r from-purple-400 to-blue-400 bg-clip-text text-transparent">
                        {initialData ? 'Edit Ingredient' : 'Create New Ingredient'}
                    </h2>
                    <button
                        onClick={onClose}
                        className="text-gray-400 hover:text-white transition-colors p-1 rounded-lg hover:bg-white/10"
                    >
                        <X size={24} />
                    </button>
                </div>
                <IngredientForm
                    initialData={initialData}
                    onSubmit={onSubmit}
                    onCancel={onClose}
                    isLoading={isLoading}
                />
            </div>
        </div>
    );
};