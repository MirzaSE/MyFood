import api from './api';
import type { Ingredient, IngredientCreateDto, IngredientUpdateDto } from '../types/ingredient';

const BASE_URL = '/v1/ingredient';

export const ingredientService = {
    getAll: async (): Promise<Ingredient[]> => {
        const response = await api.get(BASE_URL);
        return response.data;
    },

    getById: async (id: number): Promise<Ingredient> => {
        const response = await api.get(`${BASE_URL}/${id}`);
        return response.data;
    },

    create: async (data: IngredientCreateDto): Promise<Ingredient> => {
        const response = await api.post(BASE_URL, data);
        return response.data;
    },

    update: async (id: number, data: IngredientUpdateDto): Promise<Ingredient> => {
        const response = await api.put(`${BASE_URL}/${id}`, data);
        return response.data;
    },

    delete: async (id: number): Promise<void> => {
        await api.delete(`${BASE_URL}/${id}`);
    }
};