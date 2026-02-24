export * from './auth.types';

/**
 * API Types
 * 
 * Centralized type definitions for API requests and responses.
 */

// Re-export domain types
export * from '@/types/inventory';

// ============================================
// Common API Types
// ============================================

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PaginationParams {
  page?: number;
  pageSize?: number;
}

export interface SortParams {
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface DateRangeParams {
  dateFrom?: Date;
  dateTo?: Date;
}

// ============================================
// Product API Types
// ============================================

export interface GetProductsParams extends PaginationParams, SortParams {
  search?: string;
  categoryId?: string;
  stockFilter?: 'all' | 'in-stock' | 'low' | 'critical';
  supplier?: string;
  priceRange?: 'all' | '0-100' | '100-500' | '500-1000' | '1000+';
  minPrice?: number;
  maxPrice?: number;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  initialQuantity: number;
  categoryId: string;
  supplier?: string;
  lowStockThreshold: number;
  image?: File;
}

export interface UpdateProductRequest {
  id: string; // Mapped to productId in service
  productId?: string; // Optional if we want to pass it directly
  name?: string;
  description?: string;
  categoryId?: string;
  price?: number;
  supplier?: string;
  lowStockAlertThreshold?: number;
  image?: File;
}

// ============================================
// Category API Types
// ============================================

export interface GetCategoriesParams extends PaginationParams, SortParams {
  search?: string;
  productCountFilter?: 'all' | '0-20' | '20-50' | '50-100' | '100+';
  minProductCount?: number;
  maxProductCount?: number;
}

export interface CreateCategoryRequest {
  name: string;
  description: string;
}

export interface UpdateCategoryRequest extends Partial<CreateCategoryRequest> {
  id: string;
}

// ============================================
// Transaction API Types
// ============================================

export interface GetTransactionsParams extends PaginationParams, SortParams {
  search?: string;
  type?: 'all' | 'sale' | 'purchase';
  sku?: string;
  fromDate?: string;
  toDate?: string;
  minAmount?: number;
  maxAmount?: number;
}

export interface CreateTransactionRequest {
  productId: string;
  quantity: number;
}

// ============================================
// Alert API Types
// ============================================

export interface GetAlertsParams extends PaginationParams, SortParams, DateRangeParams {
  search?: string;
  severity?: 'all' | 'critical' | 'low';
  notificationStatus?: 'all' | 'sent' | 'pending';
  notificationSent?: boolean;
  stockRange?: 'all' | '0-10' | '10-30' | '30-50' | '50+';
  minStock?: number;
  maxStock?: number;
  isDismissed?: boolean | null;
  fromDate?: string;
  toDate?: string;
}

export interface DismissAlertRequest {
  alertId: string;
}

// ============================================
// User API Types
// ============================================

export interface GetUsersParams extends PaginationParams {
  SearchTerm?: string;
  Role?: string;
  IsActive?: boolean;
  SortBy?: string;
  SortDescending?: boolean;
  PageNumber?: number;
  PageSize?: number;
}

export interface UserListItemDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string | null;
  isActive: boolean;
  lastLoginDate: string | null;
}

export interface UserDetailsDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  role: string | null;
  isActive: boolean;
  emailConfirmed: boolean;
  lastLoginDate: string | null;
  lockoutEnd: string | null;
}

export interface CreateUserCommand {
  firstName: string;
  lastName: string;
  email: string;
  role: string;
}

export interface UpdateUserCommand {
  userId: string;
  firstName: string;
  lastName: string;
  role: string;
}

// Backward compatibility (old request types which we will eventually remove)
export interface CreateUserRequest {
  name: string;
  email: string;
  role: 'admin' | 'manager' | 'staff';
}

export interface UpdateUserRequest extends Partial<CreateUserRequest> {
  id: string;
  status?: 'active' | 'inactive';
}

// ============================================
// Dashboard API Types
// ============================================

export interface DashboardStatsResponse {
  totalProducts: number;
  totalStockValue: number;
  totalSales: number;
  totalPurchases: number;
  lowStockCount: number;
}

// ============================================
// Reports API Types
// ============================================

export interface ReportParams extends DateRangeParams {
  type: 'revenue' | 'expenses' | 'profit' | 'inventory';
  groupBy?: 'day' | 'week' | 'month' | 'year';
}

export interface ReportDataPoint {
  label: string;
  value: number;
}

export interface ReportResponse {
  data: ReportDataPoint[];
  summary: {
    total: number;
    average: number;
    min: number;
    max: number;
  };
}

// ============================================
// Import API Types
// ============================================

export interface UploadPreviewResponse {
  jobId: string;
}

export interface ImportStatusResponse {
  jobId: string;
  status: 'pending' | 'processing' | 'preview_ready' | 'importing' | 'completed' | 'failed';
  succeededCount?: number;
  failedCount?: number;
  errorMessage?: string;
}
