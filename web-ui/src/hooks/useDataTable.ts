/**
 * useDataTable Hook
 * 
 * Combines pagination, filtering, and sorting for table data management.
 */

import { useState, useMemo, useCallback, useEffect } from 'react';
import { usePagination } from './usePagination';
import { useFilters, type UseFiltersOptions } from './useFilters';
import { useSorting, type UseSortingOptions, parseSortValue } from './useSorting';
import { useDebounce } from './useDebounce';

export interface UseDataTableOptions<
  TData,
  TFilters extends Record<string, unknown>,
  TSortBy extends string = string
> {
  data?: TData[];
  totalCount?: number;
  initialFilters: TFilters;
  initialSortBy: TSortBy;
  initialSortOrder?: 'asc' | 'desc';
  initialPageSize?: number;
  searchDebounceMs?: number;
  onParamsChange?: (params: DataTableParams<TFilters, TSortBy>) => void;
}

export interface DataTableParams<
  TFilters extends Record<string, unknown>,
  TSortBy extends string = string
> {
  filters: TFilters;
  sortBy: TSortBy;
  sortOrder: 'asc' | 'desc';
  page: number;
  pageSize: number;
}

export function useDataTable<
  TData,
  TFilters extends Record<string, unknown>,
  TSortBy extends string = string
>({
  data,
  totalCount: externalTotalCount,
  initialFilters,
  initialSortBy,
  initialSortOrder = 'asc',
  initialPageSize = 10,
  searchDebounceMs = 300,
  onParamsChange,
}: UseDataTableOptions<TData, TFilters, TSortBy>) {
  const [searchQuery, setSearchQuery] = useState('');
  const debouncedSearch = useDebounce(searchQuery, searchDebounceMs);

  // Filters
  const filtersHook = useFilters<TFilters>({ initialFilters });

  // Sorting
  const sortingHook = useSorting<TSortBy>({
    initialSortBy,
    initialSortOrder,
  });

  // Pagination (will be updated with totalCount)
  const [totalCount, setTotalCount] = useState(externalTotalCount || 0);
  const paginationHook = usePagination({
    initialPageSize,
    totalCount,
  });

  // Update totalCount when external value changes
  useEffect(() => {
    if (externalTotalCount !== undefined) {
      setTotalCount(externalTotalCount);
    }
  }, [externalTotalCount]);

  // Reset pagination when filters or search change
  useEffect(() => {
    paginationHook.setPage(1);
  }, [debouncedSearch, filtersHook.filters]);

  // Combined params for API calls
  const params = useMemo((): DataTableParams<TFilters, TSortBy> => ({
    filters: {
      ...filtersHook.filters,
      search: debouncedSearch,
    } as TFilters,
    sortBy: sortingHook.sortBy,
    sortOrder: sortingHook.sortOrder,
    page: paginationHook.page,
    pageSize: paginationHook.pageSize,
  }), [
    filtersHook.filters,
    debouncedSearch,
    sortingHook.sortBy,
    sortingHook.sortOrder,
    paginationHook.page,
    paginationHook.pageSize,
  ]);

  // Notify parent of param changes
  useEffect(() => {
    onParamsChange?.(params);
  }, [params, onParamsChange]);

  // Handle sort value change from select
  const handleSortChange = useCallback((value: string) => {
    const { sortBy, sortOrder } = parseSortValue<TSortBy>(value);
    sortingHook.setSort(sortBy, sortOrder);
  }, [sortingHook]);

  // Clear all filters, search, and reset pagination
  const resetAll = useCallback(() => {
    setSearchQuery('');
    filtersHook.resetFilters();
    sortingHook.resetSort();
    paginationHook.reset();
  }, [filtersHook, sortingHook, paginationHook]);

  // Check if any filters, search, or sorting differs from default
  const hasActiveFilters = filtersHook.hasActiveFilters || searchQuery !== '';

  return {
    // Search
    searchQuery,
    setSearchQuery,
    debouncedSearch,

    // Filters
    filters: filtersHook.filters,
    setFilter: filtersHook.setFilter,
    setFilters: filtersHook.setFilters,
    resetFilters: filtersHook.resetFilters,
    clearFilter: filtersHook.clearFilter,
    activeFilterCount: filtersHook.activeFilterCount,

    // Sorting
    sortBy: sortingHook.sortBy,
    sortOrder: sortingHook.sortOrder,
    sortValue: sortingHook.sortValue,
    setSortBy: sortingHook.setSortBy,
    setSortOrder: sortingHook.setSortOrder,
    setSort: sortingHook.setSort,
    handleSortChange,

    // Pagination
    page: paginationHook.page,
    pageSize: paginationHook.pageSize,
    totalPages: paginationHook.totalPages,
    setPage: paginationHook.setPage,
    setPageSize: paginationHook.setPageSize,
    nextPage: paginationHook.nextPage,
    prevPage: paginationHook.prevPage,
    hasNextPage: paginationHook.hasNextPage,
    hasPrevPage: paginationHook.hasPrevPage,
    pageNumbers: paginationHook.pageNumbers,
    startIndex: paginationHook.startIndex,
    endIndex: paginationHook.endIndex,

    // Combined
    params,
    hasActiveFilters,
    resetAll,
    setTotalCount,
    totalCount,
  };
}
