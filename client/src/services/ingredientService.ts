import apiClient from "./api";
import type {
  Ingredient,
  IngredientCreateDto,
  IngredientUpdateDto,
  PaginatedResponse,
  SearchParams,
} from "../types";

const normalizeIngredientResponse = (
  responseData: Ingredient[] | PaginatedResponse<Ingredient>,
  params?: SearchParams,
): PaginatedResponse<Ingredient> => {
  if (Array.isArray(responseData)) {
    return {
      value: responseData,
      pageSize: params?.pageSize || responseData.length || 10,
      pageNumber: params?.pageNumber || 1,
      totalCount: responseData.length,
    };
  }

  return {
    value: responseData.value ?? [],
    pageSize: responseData.pageSize ?? params?.pageSize ?? 10,
    pageNumber: responseData.pageNumber ?? params?.pageNumber ?? 1,
    totalCount: responseData.totalCount ?? responseData.value?.length ?? 0,
  };
};

export const ingredientService = {
  async getAllIngredients(
    params?: SearchParams,
  ): Promise<PaginatedResponse<Ingredient>> {
    const response = await apiClient.get<
      Ingredient[] | PaginatedResponse<Ingredient>
    >("/v1/ingredients", {
      params: {
        pageNumber: params?.pageNumber || 1,
        pageSize: params?.pageSize || 10,
        searchTerm: params?.searchTerm || "",
      },
    });
    return normalizeIngredientResponse(response.data, params);
  },

  async getIngredientById(id: number): Promise<Ingredient> {
    const response = await apiClient.get<Ingredient>(`/v1/ingredients/${id}`);
    return response.data;
  },

  async createIngredient(data: IngredientCreateDto): Promise<Ingredient> {
    const response = await apiClient.post<Ingredient>("/v1/ingredients", data);
    return response.data;
  },

  async updateIngredient(
    id: number,
    data: IngredientUpdateDto,
  ): Promise<Ingredient> {
    const response = await apiClient.put<Ingredient>(
      `/v1/ingredients/${id}`,
      data,
    );
    return response.data;
  },

  async deleteIngredient(id: number): Promise<void> {
    await apiClient.delete(`/v1/ingredients/${id}`);
  },

  async searchIngredients(
    searchTerm: string,
    pageNumber = 1,
    pageSize = 10,
  ): Promise<PaginatedResponse<Ingredient>> {
    const response = await apiClient.get<
      Ingredient[] | PaginatedResponse<Ingredient>
    >("/v1/ingredients", {
      params: {
        pageNumber,
        pageSize,
        searchTerm,
      },
    });
    return normalizeIngredientResponse(response.data, {
      searchTerm,
      pageNumber,
      pageSize,
    });
  },
};
