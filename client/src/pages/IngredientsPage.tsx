import React, { useEffect, useState } from 'react';
import { Navbar } from '../components/Navbar';
import { ingredientService } from '../services/ingredientService';
import { IngredientList } from '../components/IngredientList';
import { IngredientModal } from '../components/IngredientModal';
import type { Ingredient, IngredientCreateDto } from '../types/ingredient';

export const IngredientsPage: React.FC = () => {
  const [items, setItems] = useState<Ingredient[]>([]);
  const [search, setSearch] = useState('');
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<Ingredient | null>(null);
  const [error, setError] = useState<string | null>(null);

  const load = async () => setItems(await ingredientService.getAll(1, 20, search));
  useEffect(() => { load().catch(() => setError('Failed loading ingredients')); }, [search]);

  const submit = async (data: IngredientCreateDto) => {
    if (editing) await ingredientService.update(editing.id, data);
    else await ingredientService.create(data);
    setOpen(false); setEditing(null); await load();
  };

  return <div className="min-h-screen bg-slate-950"><Navbar /><div className="container mx-auto px-6 py-10"><div className="flex justify-between mb-5"><h1 className="text-3xl text-white font-bold">Ingredients</h1><button className="px-4 py-2 bg-blue-600 text-white rounded" onClick={() => setOpen(true)}>New Ingredient</button></div>{error && <p className="text-red-400">{error}</p>}<IngredientList items={items} search={search} onSearch={setSearch} onEdit={(i) => { setEditing(i); setOpen(true); }} onDelete={async (i) => { await ingredientService.delete(i.id, i.foodEntityId); await load(); }} /></div><IngredientModal isOpen={open} initial={editing} onClose={() => { setOpen(false); setEditing(null); }} onSubmit={submit} /></div>;
};
