import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, AlertCircle, Search } from 'lucide-react';
import { Navbar } from '../components/Navbar';
import { FoodTable } from '../components/FoodTable';
import { FoodModal } from '../components/FoodModal';
import { foodService } from '../services/foodService';
import type { Food, FoodCreateDto } from '../types';

export const FoodPage: React.FC = () => {
  const navigate = useNavigate();
  const [foods, setFoods] = useState<Food[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedFood, setSelectedFood] = useState<Food | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const loadFoods = useCallback(async (query?: string) => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await foodService.getAllFoods(query);
      setFoods(data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to load foods');
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Load foods on component mount
  useEffect(() => {
    loadFoods();
  }, [loadFoods]);

  const handleSearchChange = (value: string) => {
    setSearchTerm(value);
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(() => {
      void loadFoods(value);
    }, 300);
  };

  const handleCreateClick = () => {
    setSelectedFood(null);
    setModalOpen(true);
  };

  const handleEditClick = (food: Food) => {
    setSelectedFood(food);
    setModalOpen(true);
  };

  const handleModalSubmit = async (data: FoodCreateDto) => {
    try {
      setIsSubmitting(true);
      setError(null);

      if (selectedFood) {
        // Update existing food
        const updatedFood = await foodService.updateFood(selectedFood.id, data);
        setFoods(foods.map(f => f.id === selectedFood.id ? updatedFood : f));
      } else {
        // Create new food
        const newFood = await foodService.createFood(data);
        setFoods([...foods, newFood]);
      }

      setModalOpen(false);
      setSelectedFood(null);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to save food');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await foodService.deleteFood(id);
      setFoods(foods.filter(f => f.id !== id));
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete food');
      throw err;
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {/* Error Banner */}
        {error && (
          <div className="mb-6 p-4 bg-red-500/20 border border-red-500/50 rounded-lg flex items-start space-x-3 backdrop-blur">
            <AlertCircle size={20} className="text-red-400 flex-shrink-0 mt-0.5" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        {/* Page Header */}
        <div className="mb-16">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-10">
            <div>
              <h1 className="text-4xl sm:text-5xl font-bold text-white mb-4">
                Food Management
              </h1>
              <p className="text-gray-400">
                {foods.length} {foods.length === 1 ? 'item' : 'items'} in your collection
              </p>
            </div>
            <div className="flex flex-col sm:flex-row gap-3">
              <button
                onClick={() => navigate('/foods/new')}
                className="flex items-center justify-center space-x-2 border border-white/20 text-white px-6 py-3 rounded-lg transition-all duration-200 hover:bg-white/10 font-semibold"
              >
                <span>Create Food Page</span>
              </button>
              <button
                onClick={handleCreateClick}
                disabled={isLoading || isSubmitting}
                className="flex items-center justify-center space-x-2 bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white px-6 py-3 rounded-lg transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg hover:shadow-purple-500/50 font-semibold"
              >
                <Plus size={20} />
                <span>Add Food</span>
              </button>
            </div>
          </div>
        </div>

        {/* Search Bar */}
        <div className="mb-8">
          <div className="relative max-w-md">
            <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => handleSearchChange(e.target.value)}
              placeholder="Search by name or calories..."
              className="w-full pl-10 pr-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:border-purple-500"
            />
          </div>
        </div>

        {/* Loading State */}
        {isLoading ? (
          <div className="flex flex-col items-center justify-center py-20">
            <div className="relative w-16 h-16 mb-4">
              <div className="absolute inset-0 bg-gradient-to-r from-purple-500 to-blue-500 rounded-full animate-spin"></div>
              <div className="absolute inset-2 bg-slate-900 rounded-full"></div>
            </div>
            <p className="text-gray-300 font-medium">Loading your foods...</p>
          </div>
        ) : (
          /* Content */
          <div>
            <FoodTable
              foods={foods}
              onEdit={handleEditClick}
              onDelete={handleDelete}
              isLoading={isSubmitting}
            />
          </div>
        )}
      </div>

      {/* Modal */}
      <FoodModal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          setSelectedFood(null);
        }}
        onSubmit={handleModalSubmit}
        initialData={selectedFood}
        isLoading={isSubmitting}
      />
    </div>
  );
};
