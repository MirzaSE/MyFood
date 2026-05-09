import React from 'react';
import type { Ingredient } from '../types/ingredient';

interface Props { items: Ingredient[]; onEdit: (i: Ingredient) => void; onDelete: (i: Ingredient) => Promise<void>; search: string; onSearch: (v: string) => void; }

export const IngredientList: React.FC<Props> = ({ items, onEdit, onDelete, search, onSearch }) => {
  return <div><input value={search} onChange={(e) => onSearch(e.target.value)} placeholder="Search" className="mb-3 p-2 rounded bg-slate-800 text-white w-full" />
  <table className="w-full text-white"><thead><tr><th>Name</th><th>Unit</th><th>Calories</th><th>Actions</th></tr></thead><tbody>{items.length === 0 ? <tr><td colSpan={4}>No ingredients.</td></tr> : items.map(i => <tr key={i.id}><td>{i.name}</td><td>{i.unit}</td><td>{i.caloriesPerUnit}</td><td><button onClick={() => onEdit(i)}>Edit</button> <button onClick={() => onDelete(i)}>Delete</button></td></tr>)}</tbody></table></div>;
};
