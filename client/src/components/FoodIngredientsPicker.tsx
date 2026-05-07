import React from 'react';
import { Check, ChevronDown, Plus, Search, Trash2 } from 'lucide-react';
import type { Ingredient, SelectedIngredient } from '../types';

interface FoodIngredientsPickerProps {
  availableIngredients: Ingredient[];
  selectedIngredients: SelectedIngredient[];
  onChange: (items: SelectedIngredient[]) => void;
}

export const FoodIngredientsPicker: React.FC<FoodIngredientsPickerProps> = ({
  availableIngredients,
  selectedIngredients,
  onChange,
}) => {
  const containerRef = React.useRef<HTMLDivElement | null>(null);
  const [selectedId, setSelectedId] = React.useState<number>(0);
  const [quantityInput, setQuantityInput] = React.useState('1');
  const [search, setSearch] = React.useState('');
  const [isOpen, setIsOpen] = React.useState(false);
  const [quantityDrafts, setQuantityDrafts] = React.useState<Record<number, string>>({});

  React.useEffect(() => {
    const onDocumentClick = (event: MouseEvent) => {
      if (!containerRef.current) return;
      if (!containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', onDocumentClick);
    return () => document.removeEventListener('mousedown', onDocumentClick);
  }, []);

  React.useEffect(() => {
    setQuantityDrafts((prev) => {
      const next: Record<number, string> = {};
      for (const item of selectedIngredients) {
        next[item.ingredient.id] = prev[item.ingredient.id] ?? String(item.quantity);
      }
      return next;
    });
  }, [selectedIngredients]);

  const normalizeIngredientName = (name: string) => name.trim().toLowerCase();
  const selectedNameSet = new Set(
    selectedIngredients.map((item) => normalizeIngredientName(item.ingredient.name))
  );

  const uniqueIngredientsByName = React.useMemo(() => {
    const seen = new Set<string>();
    const unique: Ingredient[] = [];

    for (const ingredient of availableIngredients) {
      const key = normalizeIngredientName(ingredient.name);
      if (seen.has(key)) continue;
      seen.add(key);
      unique.push(ingredient);
    }

    return unique;
  }, [availableIngredients]);

  const addableIngredients = React.useMemo(
    () => uniqueIngredientsByName.filter((ingredient) => !selectedNameSet.has(normalizeIngredientName(ingredient.name))),
    [uniqueIngredientsByName, selectedNameSet]
  );

  const selectedIngredient = addableIngredients.find((i) => i.id === selectedId) ?? null;

  React.useEffect(() => {
    if (selectedId === 0) return;
    if (!addableIngredients.some((item) => item.id === selectedId)) {
      setSelectedId(0);
    }
  }, [addableIngredients, selectedId]);

  const filteredIngredients = addableIngredients.filter((ingredient) => {
    const searchValue = search.trim().toLowerCase();
    if (!searchValue) return true;
    return ingredient.name.toLowerCase().includes(searchValue) || ingredient.unit.toLowerCase().includes(searchValue);
  });

  const parseAmount = (value: string): number => {
    const parsed = Number.parseInt(value.replace(/\D/g, ''), 10);
    return Number.isFinite(parsed) && parsed > 0 ? parsed : 1;
  };

  const addIngredient = () => {
    const ingredient = selectedIngredient;
    if (!ingredient) return;
    if (selectedNameSet.has(normalizeIngredientName(ingredient.name))) return;

    const parsedQuantity = parseAmount(quantityInput);
    onChange([...selectedIngredients, { ingredient, quantity: parsedQuantity }]);
    setSelectedId(0);
    setQuantityInput('1');
    setSearch('');
  };

  const updateQuantity = (ingredientId: number, newQuantity: number) => {
    onChange(
      selectedIngredients.map((item) =>
        item.ingredient.id === ingredientId ? { ...item, quantity: Math.max(1, Math.floor(newQuantity)) } : item
      )
    );
  };

  const removeIngredient = (ingredientId: number) => {
    onChange(selectedIngredients.filter((item) => item.ingredient.id !== ingredientId));
  };

  const totalCalories = selectedIngredients.reduce(
    (sum, item) => sum + item.ingredient.caloriesPerUnit * item.quantity,
    0
  );

  return (
    <div className="space-y-4">
      <div className="bg-slate-800/60 border border-white/10 rounded-xl p-5">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold text-white">Ingredients</h3>
          <span className="text-sm text-gray-400">
            {selectedIngredients.length} selected
          </span>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-3 mb-5">
          <div className="md:col-span-2 relative" ref={containerRef}>
            <button
              type="button"
              onClick={() => setIsOpen((prev) => !prev)}
              className="w-full px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-left text-white flex items-center justify-between hover:border-purple-400/60 transition-colors"
            >
              <span className={selectedIngredient ? 'text-white' : 'text-gray-400'}>
                {selectedIngredient ? `${selectedIngredient.name} (${selectedIngredient.unit})` : 'Select ingredient...'}
              </span>
              <ChevronDown size={16} className={`text-gray-300 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
            </button>

            {isOpen && (
              <div className="absolute z-30 mt-2 w-full rounded-lg border border-white/20 bg-slate-900 shadow-xl overflow-hidden">
                <div className="p-2 border-b border-white/10">
                  <div className="flex items-center gap-2 px-2 py-2 rounded-md bg-white/5 border border-white/10">
                    <Search size={14} className="text-gray-400" />
                    <input
                      value={search}
                      onChange={(e) => setSearch(e.target.value)}
                      placeholder="Search ingredient..."
                      className="w-full bg-transparent text-sm text-white placeholder-gray-400 focus:outline-none"
                    />
                  </div>
                </div>
                <div className="max-h-60 overflow-y-auto">
                  {filteredIngredients.length === 0 && (
                    <p className="px-3 py-3 text-sm text-gray-400">No ingredients found.</p>
                  )}
                  {filteredIngredients.map((ingredient) => {
                    return (
                      <button
                        key={ingredient.id}
                        type="button"
                        onClick={() => {
                          setSelectedId(ingredient.id);
                          setIsOpen(false);
                        }}
                        className="w-full px-3 py-2 text-left hover:bg-white/10 transition-colors"
                      >
                        <div className="flex items-center justify-between">
                          <div>
                            <p className="text-white text-sm font-medium">{ingredient.name}</p>
                            <p className="text-xs text-gray-400">
                              {ingredient.caloriesPerUnit.toFixed(1)} kcal / {ingredient.unit}
                            </p>
                          </div>
                          {selectedId === ingredient.id && <Check size={16} className="text-green-400" />}
                        </div>
                      </button>
                    );
                  })}
                </div>
              </div>
            )}
          </div>

          <input
            type="text"
            inputMode="numeric"
            value={quantityInput}
            onChange={(e) => setQuantityInput(e.target.value)}
            onBlur={() => setQuantityInput(String(parseAmount(quantityInput)))}
            className="px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
          />
          <button
            type="button"
            onClick={addIngredient}
            disabled={!selectedIngredient}
            className="flex items-center justify-center gap-2 px-3 py-2 bg-purple-600 hover:bg-purple-700 rounded-lg text-white disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Plus size={16} />
            Add
          </button>
        </div>

        <div className="space-y-2">
          {selectedIngredients.map((item) => (
            <div
              key={item.ingredient.id}
              className="grid grid-cols-1 md:grid-cols-5 gap-3 items-center p-3 rounded-lg bg-slate-900/60 border border-white/10"
            >
              <div className="md:col-span-2">
                <p className="text-white font-medium">{item.ingredient.name}</p>
                <p className="text-xs text-gray-400">
                  {item.ingredient.caloriesPerUnit.toFixed(1)} cal / {item.ingredient.unit}
                </p>
              </div>
              <input
                type="text"
                inputMode="numeric"
                value={quantityDrafts[item.ingredient.id] ?? String(item.quantity)}
                onChange={(e) =>
                  setQuantityDrafts((prev) => ({
                    ...prev,
                    [item.ingredient.id]: e.target.value,
                  }))
                }
                onBlur={() => {
                  const rawValue = quantityDrafts[item.ingredient.id] ?? String(item.quantity);
                  const parsed = parseAmount(rawValue);
                  updateQuantity(item.ingredient.id, parsed);
                  setQuantityDrafts((prev) => ({
                    ...prev,
                    [item.ingredient.id]: String(parsed),
                  }));
                }}
                className="px-3 py-2 bg-white/10 border border-white/20 rounded-lg text-white"
              />
              <p className="text-gray-300">{(item.ingredient.caloriesPerUnit * item.quantity).toFixed(1)} kcal</p>
              <button
                type="button"
                onClick={() => removeIngredient(item.ingredient.id)}
                className="justify-self-end p-2 rounded-lg bg-red-500/20 hover:bg-red-500/30 text-red-300 border border-red-500/40"
              >
                <Trash2 size={16} />
              </button>
            </div>
          ))}
          {selectedIngredients.length === 0 && (
            <p className="text-sm text-gray-400">No ingredients selected yet.</p>
          )}
        </div>

        <div className="mt-4 pt-4 border-t border-white/10 flex items-center justify-end">
          <p className="text-sm text-gray-300">
            Total: <span className="font-semibold text-white">{totalCalories.toFixed(1)} kcal</span>
          </p>
        </div>
      </div>
    </div>
  );
};
