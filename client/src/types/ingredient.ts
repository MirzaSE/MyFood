export interface Ingredient {
    id: number;
    name: string;
    quantity: number;
    foodId: number;
}

export interface IngredientCreateDto {
    name: string;
    quantity: number;
    foodId: number;
}

export interface IngredientUpdateDto {
    name?: string;
    quantity?: number;
    foodId?: number;
}