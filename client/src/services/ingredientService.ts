import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';
import { authService } from './authService';

const API_URL = 'http://localhost:8080/api/v1/ingredients';

export const ingredientService = {
  async getAll(page: number = 1, pageSize: number = 10): Promise<Ingredient[]> {
    const response = await fetch(
      `${API_URL}?page=${page}&pageCount=${pageSize}`,
      {
        headers: {
          Authorization: `Bearer ${authService.getToken()}`,
        },
      }
    );
    if (!response.ok) throw new Error('Failed to fetch ingredients');
    const data = await response.json();
    return data.value || data;
  },

  async getById(id: number): Promise<Ingredient> {
    const response = await fetch(`${API_URL}/${id}`, {
      headers: {
        Authorization: `Bearer ${authService.getToken()}`,
      },
    });
    if (!response.ok) throw new Error('Failed to fetch ingredient');
    return response.json();
  },

  async create(ingredient: IngredientCreateDto): Promise<Ingredient> {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${authService.getToken()}`,
      },
      body: JSON.stringify(ingredient),
    });
    if (!response.ok) throw new Error('Failed to create ingredient');
    return response.json();
  },

  async update(id: number, ingredient: IngredientUpdateDto): Promise<Ingredient> {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${authService.getToken()}`,
      },
      body: JSON.stringify(ingredient),
    });
    if (!response.ok) throw new Error('Failed to update ingredient');
    return response.json();
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${authService.getToken()}`,
      },
    });
    if (!response.ok) throw new Error('Failed to delete ingredient');
  },

  async search(term: string): Promise<Ingredient[]> {
    const response = await fetch(`${API_URL}/search?term=${encodeURIComponent(term)}`, {
      headers: {
        Authorization: `Bearer ${authService.getToken()}`,
      },
    });
    if (!response.ok) throw new Error('Failed to search ingredients');
    return response.json();
  },
};
