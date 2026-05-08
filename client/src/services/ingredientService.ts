import apiClient from "./api";

import type {
    Ingredient,
    CreateIngredientDto,
    UpdateIngredientDto,
} from "../types/ingredient";

const BASE_URL = "/v1/Ingredient";

export const ingredientService = {
    async getAll(): Promise<Ingredient[]> {
        const response = await apiClient.get(BASE_URL);
        return response.data;
    },

    async getById(id: number): Promise<Ingredient> {
        const response = await apiClient.get(`${BASE_URL}/${id}`);
        return response.data;
    },

    async create(data: CreateIngredientDto): Promise<Ingredient> {
        const response = await apiClient.post(BASE_URL, data);
        return response.data;
    },

    async update(
        id: number,
        data: UpdateIngredientDto
    ): Promise<Ingredient> {
        const response = await apiClient.put(
            `${BASE_URL}/${id}`,
            data
        );

        return response.data;
    },

    async delete(id: number): Promise<void> {
        await apiClient.delete(`${BASE_URL}/${id}`);
    },
};