import React, { useState, useEffect } from 'react';
import { Plus, AlertCircle } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { Navbar } from '../components/Navbar';
import { FoodTable } from '../components/FoodTable';
import { FoodModal } from '../components/FoodModal';
import { extractApiErrorMessage } from '../services/api';
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

  useEffect(() => {
    void loadFoods();
  }, []);

  const loadFoods = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await foodService.getAllFoods();
      setFoods(data);
    } catch (err: any) {
      setError(extractApiErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
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
        const updatedFood = await foodService.updateFood(selectedFood.id, data);
        setFoods((currentFoods) => currentFoods.map((food) => food.id === selectedFood.id ? updatedFood : food));
      } else {
        const newFood = await foodService.createFood(data);
        setFoods((currentFoods) => [...currentFoods, newFood]);
      }

      setModalOpen(false);
      setSelectedFood(null);
    } catch (err: any) {
      setError(extractApiErrorMessage(err));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      setError(null);
      await foodService.deleteFood(id);
      setFoods((currentFoods) => currentFoods.filter((food) => food.id !== id));
    } catch (err: any) {
      setError(extractApiErrorMessage(err));
      throw err;
    }
  };

  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top_left,_rgba(245,158,11,0.16),_transparent_35%),linear-gradient(135deg,#0f172a_0%,#1e293b_45%,#111827_100%)]">
      <Navbar />

      <div className="container mx-auto px-6 py-16">
        {error && (
          <div className="mb-6 flex items-start space-x-3 rounded-lg border border-red-500/50 bg-red-500/20 p-4 backdrop-blur">
            <AlertCircle size={20} className="mt-0.5 flex-shrink-0 text-red-400" />
            <p className="text-red-200">{error}</p>
          </div>
        )}

        <div className="mb-16">
          <div className="flex flex-col gap-10 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <p className="mb-3 text-sm uppercase tracking-[0.35em] text-amber-300">Food Overview</p>
              <h1 className="text-4xl font-black text-white sm:text-5xl">
                Food Management
              </h1>
              <p className="mt-4 text-gray-400">
                {foods.length} {foods.length === 1 ? 'item' : 'items'} in your collection
              </p>
            </div>
            <button
              onClick={() => navigate('/foods/new')}
              disabled={isLoading || isSubmitting}
              className="flex items-center justify-center gap-2 rounded-xl bg-amber-500 px-6 py-3 font-semibold text-slate-900 transition disabled:opacity-50"
            >
              <Plus size={20} />
              <span>Add Food</span>
            </button>
          </div>
        </div>

        {isLoading ? (
          <div className="flex flex-col items-center justify-center py-20">
            <div className="relative mb-4 h-16 w-16">
              <div className="absolute inset-0 animate-spin rounded-full bg-gradient-to-r from-amber-400 to-orange-500"></div>
              <div className="absolute inset-2 rounded-full bg-slate-900"></div>
            </div>
            <p className="font-medium text-gray-300">Loading your foods...</p>
          </div>
        ) : (
          <FoodTable
            foods={foods}
            onEdit={handleEditClick}
            onDelete={handleDelete}
            isLoading={isSubmitting}
          />
        )}
      </div>

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
