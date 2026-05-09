import React from 'react';
import { IngredientForm } from './IngredientForm';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

interface Props { isOpen: boolean; initial?: Ingredient | null; onClose: () => void; onSubmit: (v: IngredientCreateDto) => Promise<void>; loading?: boolean; error?: string | null; success?: string | null; }

export const IngredientModal: React.FC<Props> = ({ isOpen, initial, onClose, onSubmit, loading, error, success }) => {
  if (!isOpen) return null;
  return <div className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50"><div className="bg-slate-900 p-4 rounded-xl w-full max-w-md"><h2 className="text-white text-xl mb-3">{initial ? 'Edit Ingredient' : 'New Ingredient'}</h2>{error && <p className="text-red-400 text-sm">{error}</p>}{success && <p className="text-green-400 text-sm">{success}</p>}<IngredientForm initial={initial ?? undefined} onCancel={onClose} onSubmit={onSubmit} loading={loading} /></div></div>;
};
