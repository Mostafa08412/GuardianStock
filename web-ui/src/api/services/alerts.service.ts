import { apiClient } from '../client';
import type {
  PaginatedResponse,
  LowStockAlertListItemDto,
} from '../types';

/**
 * Mapping existing search params to V2 API query params.
 */
function buildAlertQueryParams(params: any): Record<string, any> {
  return {
    SearchTerm: params.search || undefined,
    Severity: (params.severity && params.severity !== 'all') ? params.severity : undefined,
    IsNotificationSent: params.notificationSent,
    IsDismissed: params.isDismissed,
    FromDate: params.fromDate || undefined,
    ToDate: params.toDate || undefined,
    SortBy: params.sortBy || undefined,
    SortDescending: params.sortOrder === 'desc' ? true : undefined,
    Page: params.page,
    PageSize: params.pageSize,
  };
}

export const alertsService = {
  /**
   * Get paginated list of low stock alerts (V3)
   */
  getList: async (params: any = {}): Promise<PaginatedResponse<LowStockAlertListItemDto>> => {
    return apiClient.getPaginated<LowStockAlertListItemDto>('/v2/alerts', {
      params: buildAlertQueryParams(params),
    });
  },

  /**
   * Dismiss an alert (V2)
   */
  dismiss: async (inventoryId: string): Promise<void> => {
    await apiClient.post(`/v2/alerts/${inventoryId}/dismiss`);
  },
};
