/**
 * Categories Service
 * 
 * API service for category-related operations.
 * Supports both mock data and real API calls.
 */

import { apiClient } from '../client';
import type {
  CategoryListItem,
  CategoryDetails,
  PaginatedResponse,
  GetCategoriesParams,
  CreateCategoryRequest,
  UpdateCategoryRequest
} from '../types';

/**
 * Maps frontend params to backend query params per specs2.json.
 */
function buildCategoryQueryParams(params: GetCategoriesParams): Record<string, string | number | boolean | undefined> {
  return {
    SearchTerm: params.search || undefined,
    SortBy: params.sortBy || undefined,
    SortDescending: params.sortOrder === 'desc',
    Page: params.page,
    PageSize: params.pageSize,
    MinProductCount: params.minProductCount,
    MaxProductCount: params.maxProductCount,
  };
}

export const categoriesService = {
  /**
   * Get paginated list of categories with filtering and sorting
   */
  getList: async (params: GetCategoriesParams = {}): Promise<PaginatedResponse<CategoryListItem>> => {
    return apiClient.getPaginated<CategoryListItem>('/v2/categories', {
      params: buildCategoryQueryParams(params),
    });
  },

  /**
   * Get a single category by ID
   */
  getById: async (id: string): Promise<CategoryDetails | undefined> => {
    return apiClient.get<CategoryDetails>(`/v2/categories/${id}`);
  },

  /**
   * Create a new category
   */
  create: async (data: CreateCategoryRequest): Promise<CategoryListItem> => {
    return apiClient.post<CategoryListItem>('/v2/categories', {
      name: data.name,
      description: data.description,
    });
  },

  /**
   * Update an existing category
   */
  update: async (data: UpdateCategoryRequest): Promise<CategoryListItem> => {
    return apiClient.put<CategoryListItem>(`/v2/categories/${data.id}`, {
      id: data.id,
      name: data.name,
      description: data.description,
    });
  },

  /**
   * Delete a category
   */
  delete: async (id: string): Promise<{ success: boolean }> => {
    await apiClient.delete(`/v2/categories/${id}`);
    return { success: true };
  },
};
