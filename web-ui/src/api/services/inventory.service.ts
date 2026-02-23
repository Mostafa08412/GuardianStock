import { apiClient } from '../client';
import type {
    PaginatedResponse,
    InventoryListItemDto,
    InventoryDetailsDto,
    StockSummaryDto,
    AdjustLowStockThresholdCommand
} from '../types';

/**
 * Mapping existing search params to V3 API query params.
 */
function buildInventoryQueryParams(params: any): Record<string, any> {
    return {
        SearchTerm: params.search || undefined,
        StockStatus: params.stockStatus !== 'all' ? params.stockStatus : undefined,
        MinPrice: params.minPrice,
        MaxPrice: params.maxPrice,
        SortBy: params.sortBy || undefined,
        SortDescending: params.sortOrder === 'desc' ? true : undefined,
        Page: params.page,
        PageSize: params.pageSize,
    };
}

export const inventoryService = {
    /**
     * Get paginated list of inventories (V2)
     */
    getList: async (params: any = {}): Promise<PaginatedResponse<InventoryListItemDto>> => {
        return apiClient.getPaginated<InventoryListItemDto>('/v2/inventories', {
            params: buildInventoryQueryParams(params),
        });
    },

    /**
     * Get inventory details by ID (V2)
     */
    getById: async (inventoryId: string): Promise<InventoryDetailsDto> => {
        return apiClient.get<InventoryDetailsDto>(`/v2/inventories/${inventoryId}`);
    },

    /**
     * Get stock summary (V2)
     */
    getSummary: async (): Promise<StockSummaryDto> => {
        return apiClient.get<StockSummaryDto>('/v2/inventories/summary');
    },

    /**
     * Adjust low stock threshold (V2)
     */
    adjustThreshold: async (command: AdjustLowStockThresholdCommand): Promise<void> => {
        await apiClient.post(`/v2/inventories/${command.inventoryId}/adjust-threshold`, command);
    }
};
