/**
 * Transactions Service
 * 
 * API service for transaction-related operations.
 * Calls the real backend API as defined in specs2.json.
 */

import { apiClient } from '../client';
import {
  GetTransactionsParams,
  CreateTransactionRequest,
  PaginatedResponse
} from '../types';
import { Transaction, TransactionDetails, TransactionType } from '@/types/inventory';

/**
 * Maps frontend params to backend query params per specs2.json.
 * Transactions endpoint uses IsDescending and PageNumber.
 */
// Helper to build query params
const buildTransactionQueryParams = (params: GetTransactionsParams) => {
  const queryParams = {
    SearchTerm: params.search,
    Type: params.type === 'sale' ? TransactionType.Sale :
      params.type === 'purchase' ? TransactionType.Purchase : undefined,
    Sku: params.sku,
    FromDate: params.fromDate, // String ISO expected by API client/backend binding
    ToDate: params.toDate,
    MinAmount: params.minAmount,
    MaxAmount: params.maxAmount,
    SortBy: params.sortBy || undefined,
    IsDescending: params.sortOrder === 'desc' ? true : false,
    PageNumber: params.page,
    PageSize: params.pageSize,
  };
  return queryParams;
};

export const transactionsService = {
  /**
   * Get paginated list of transactions with filtering and sorting
   */
  getList: async (params: GetTransactionsParams = {}): Promise<PaginatedResponse<Transaction>> => {
    return apiClient.getPaginated<Transaction>('/v2/transactions', {
      params: buildTransactionQueryParams(params),
    });
  },

  /**
   * Get a single transaction by ID — returns TransactionDetailsDto
   */
  getById: async (id: string): Promise<TransactionDetails> => {
    return apiClient.get<TransactionDetails>(`/v2/transactions/${id}`);
  },

  /**
   * Record a sale transaction
   */
  recordSale: async (data: CreateTransactionRequest): Promise<void> => {
    await apiClient.post('/v2/transactions/sale', {
      productId: data.productId,
      quantity: data.quantity,
    });
  },

  /**
   * Record a purchase transaction
   */
  recordPurchase: async (data: CreateTransactionRequest): Promise<void> => {
    await apiClient.post('/v2/transactions/purchase', {
      productId: data.productId,
      quantity: data.quantity,
    });
  },

  /**
   * Get transactions for a specific product (filters via SKU or search)
   */
  getByProductId: async (productId: string, params: GetTransactionsParams = {}): Promise<PaginatedResponse<Transaction>> => {
    return apiClient.getPaginated<Transaction>('/v2/transactions', {
      params: {
        ...buildTransactionQueryParams(params),
        SearchTerm: productId, // Use productId as search since the endpoint doesn't have a productId filter
      },
    });
  },
};
