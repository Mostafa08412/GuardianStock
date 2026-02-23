import { apiClient } from '../client';
import { API_CONFIG } from '../config';
import type {
  Product,
  ProductListItem,
  Category,
  PaginatedResponse,
  GetProductsParams,
  CreateProductRequest,
  UpdateProductRequest,
  UploadPreviewResponse,
  ApiResponse
} from '../types';

const buildProductQueryParams = (params: GetProductsParams) => {
  const queryParams: Record<string, string | number | boolean | undefined> = {
    Page: params.page,
    PageSize: params.pageSize,
    SearchTerm: params.search,
    SortBy: params.sortBy,
    SortDescending: params.sortOrder === 'desc',
    CategoryId: params.categoryId,
    MinPrice: params.minPrice,
    MaxPrice: params.maxPrice,
    StockLevel: params.stockFilter === 'all' ? undefined :
      params.stockFilter === 'in-stock' ? 'Normal' :
        params.stockFilter === 'low' ? 'Low' :
          params.stockFilter === 'critical' ? 'Critical' : undefined
  };
  return queryParams;
};

const formatImageUrl = (url: string | undefined) => {
  if (!url) return undefined;
  if (url.startsWith('http')) return url;

  // Deriving server root from API base URL (e.g. http://localhost:5089/api -> http://localhost:5089)
  const serverUrl = API_CONFIG.baseUrl.replace(/\/api$/, '');
  const cleanUrl = url.startsWith('/') ? url.slice(1) : url;

  return `${serverUrl}/${cleanUrl}`;
};

export const productsService = {
  getList: async (params: GetProductsParams = {}): Promise<PaginatedResponse<ProductListItem>> => {
    const queryParams = buildProductQueryParams(params);
    const response = await apiClient.getPaginated<ProductListItem>('/v2/products', {
      params: queryParams
    });

    return {
      ...response,
      items: response.items.map(item => ({
        ...item,
        imageUrl: formatImageUrl(item.imageUrl)
      }))
    };
  },

  getById: async (id: string): Promise<Product | undefined> => {
    const data = await apiClient.get<any>(`/v2/products/${id}`);
    if (!data) return undefined;

    return {
      ...data,
      // Map API fields to frontend interface
      currentStock: data.stockQuantity, // Map stockQuantity to currentStock for compatibility
      updatedAt: data.updatedAt ? new Date(data.updatedAt) : undefined,

      // Handle categoryId: API might not return it in ProductDto, so we fallback to empty string if missing
      categoryId: data.categoryId || '',
      categoryName: data.categoryName,

      imageUrl: formatImageUrl(data.imageUrl),

      // Ensure arrays are initialized
      recentActivities: data.recentActivities || [],
      stockHistory: data.stockHistory || []
    };
  },

  create: async (data: CreateProductRequest): Promise<ProductListItem> => {
    const formData = new FormData();
    formData.append('Name', data.name);
    if (data.description) formData.append('Description', data.description);
    formData.append('Price', data.price.toString());
    if (data.supplier) formData.append('Supplier', data.supplier);
    formData.append('CategoryId', data.categoryId);
    formData.append('InitialQuantity', data.initialQuantity.toString());
    formData.append('LowStockThreshold', data.lowStockThreshold.toString());
    if (data.image) formData.append('Image', data.image);

    return apiClient.post<any>('/v2/products', formData);
  },

  update: async (id: string, data: UpdateProductRequest): Promise<void> => {
    const formData = new FormData();
    formData.append('ProductId', id);
    if (data.name) formData.append('Name', data.name);
    if (data.description) formData.append('Description', data.description);
    if (data.categoryId) formData.append('CategoryId', data.categoryId);
    if (data.price !== undefined) formData.append('Price', data.price.toString());
    if (data.supplier) formData.append('Supplier', data.supplier);
    if (data.lowStockAlertThreshold !== undefined) {
      formData.append('LowStockAlertThreshold', data.lowStockAlertThreshold.toString());
    }
    if (data.image) formData.append('Image', data.image);

    return apiClient.put<void>(`/v2/products/${id}`, formData);
  },

  delete: async (id: string): Promise<{ success: boolean }> => {
    await apiClient.delete(`/v2/products/${id}`);
    return { success: true };
  },

  getCategoryName: async (categoryId: string): Promise<string> => {
    try {
      const category = await apiClient.get<Category>(`/v2/categories/${categoryId}`);
      return category.name;
    } catch {
      return 'Unknown';
    }
  },

  uploadPreview: async (file: File, jobId: string): Promise<UploadPreviewResponse> => {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('jobId', jobId);

    const token = localStorage.getItem('access_token');
    const headers: HeadersInit = token ? { 'Authorization': `Bearer ${token}` } : {};

    const response = await fetch(`${API_CONFIG.baseUrl}/v2/products/import-preview`, {
      method: 'POST',
      headers: headers,
      body: formData,
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || 'Import preview failed');
    }

    const json = await response.json();
    if (json && !json.isSuccess) {
      throw new Error(json.message || 'Import preview failed');
    }
    return json.data;
  },

  confirmImport: async (previewId: string, jobId: string): Promise<void> => {
    const formData = new FormData();
    formData.append('PreviewId', previewId);
    formData.append('JobId', jobId);

    const token = localStorage.getItem('access_token');
    const headers: HeadersInit = token ? { 'Authorization': `Bearer ${token}` } : {};

    const response = await fetch(`${API_CONFIG.baseUrl}/v2/products/confirm-import`, {
      method: 'POST',
      headers: headers,
      body: formData,
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || 'Confirm import failed');
    }

    const json = await response.json();
    if (json && !json.isSuccess) {
      throw new Error(json.message || 'Confirm import failed');
    }
  }
};
