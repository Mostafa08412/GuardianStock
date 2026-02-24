/**
 * Dashboard Service
 * 
 * API service for dashboard-related operations.
 * Supports both mock data and real API calls.
 */

import { API_CONFIG } from '../config';
import { apiClient } from '../client';
import { mockDashboardStats, salesChartData, categoryDistributionData, topProductsData } from '@/mocks';
import { DashboardData } from '@/types';

const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

const formatImageUrl = (url: string | undefined) => {
  if (!url) return undefined;
  if (url.startsWith('http')) return url;
  const serverUrl = API_CONFIG.baseUrl.replace(/\/api$/, '');
  const cleanUrl = url.startsWith('/') ? url.slice(1) : url;
  return `${serverUrl}/${cleanUrl}`;
};

export const dashboardService = {
  /**
   * Get full dashboard data
   */
  getDashboardData: async (): Promise<DashboardData> => {
    // Backend returns the full structure
    const response = await apiClient.get<any>('/v2/dashboard');
    const data = response || {};

    // Safely unwrap and map because backend might be inconsistent with cases
    const stats = data.stats || data.Stats || {};
    const topProductsRaw = data.topProducts || data.TopProducts || [];
    const lowStockRaw = data.lowStockInventories || data.LowStockInventories || [];

    const mappedData: DashboardData = {
      stats: {
        totalProducts: stats.totalProducts || stats.TotalProducts || 0,
        totalStockValue: stats.totalStockValue || stats.TotalStockValue || 0,
        totalSales: stats.totalSales || stats.TotalSales || 0,
        lowStockCount: stats.lowStockCount || stats.LowStockCount || 0,
        recentTransactions: (stats.recentTransactions || stats.RecentTransactions || []).map((tx: any) => ({
          id: tx.id || tx.transactionId || tx.Id || tx.TransactionId,
          productName: tx.productName || tx.ProductName,
          productId: tx.productId || tx.ProductId || tx.productID,
          quantity: tx.quantity || tx.Quantity || 0,
          type: (tx.type || tx.Type || 'sale').toString().toLowerCase(),
          date: tx.date || tx.Date || new Date().toISOString(),
          totalAmount: tx.totalAmount || tx.TotalAmount || 0,
          userName: tx.userName || tx.UserName || 'System'
        }))
      },
      salesChartData: data.salesChartData || data.SalesChartData || [],
      categoryDistribution: data.categoryDistribution || data.CategoryDistribution || [],
      topProducts: topProductsRaw.map((p: any) => ({
        id: p.id || p.productId || p.Id || p.ProductId,
        name: p.name || p.Name,
        imageUrl: formatImageUrl(p.imageUrl || p.ImageUrl),
        sales: p.sales || p.Sales || 0,
        revenue: p.revenue || p.Revenue || 0
      })),
      lowStockInventories: lowStockRaw.map((item: any) => ({
        inventoryId: item.inventoryId || item.inventoryID || item.id || item.Id || item.InventoryId,
        productId: item.productId || item.ProductId,
        productName: item.productName || item.ProductName,
        status: item.status || item.Status,
        stock: item.stock || item.Stock || 0,
        threshold: item.threshold || item.Threshold || 0
      }))
    };

    return mappedData;
  },
};
