/**
 * useSorting Hook
 * 
 * Manages sorting state and logic.
 */

import { useState, useCallback } from 'react';

export interface SortingState<T extends string = string> {
  sortBy: T;
  sortOrder: 'asc' | 'desc';
}

export interface UseSortingOptions<T extends string = string> {
  initialSortBy: T;
  initialSortOrder?: 'asc' | 'desc';
  onSortChange?: (sortBy: T, sortOrder: 'asc' | 'desc') => void;
}

export interface UseSortingReturn<T extends string = string> extends SortingState<T> {
  setSortBy: (sortBy: T) => void;
  setSortOrder: (sortOrder: 'asc' | 'desc') => void;
  setSort: (sortBy: T, sortOrder: 'asc' | 'desc') => void;
  toggleSortOrder: () => void;
  resetSort: () => void;
  sortValue: string; // Combined value for select inputs (e.g., "name-asc")
}

export function useSorting<T extends string = string>({
  initialSortBy,
  initialSortOrder = 'asc',
  onSortChange,
}: UseSortingOptions<T>): UseSortingReturn<T> {
  const [sortBy, setSortByState] = useState<T>(initialSortBy);
  const [sortOrder, setSortOrderState] = useState<'asc' | 'desc'>(initialSortOrder);

  const setSortBy = useCallback((newSortBy: T) => {
    setSortByState(newSortBy);
    onSortChange?.(newSortBy, sortOrder);
  }, [sortOrder, onSortChange]);

  const setSortOrder = useCallback((newSortOrder: 'asc' | 'desc') => {
    setSortOrderState(newSortOrder);
    onSortChange?.(sortBy, newSortOrder);
  }, [sortBy, onSortChange]);

  const setSort = useCallback((newSortBy: T, newSortOrder: 'asc' | 'desc') => {
    setSortByState(newSortBy);
    setSortOrderState(newSortOrder);
    onSortChange?.(newSortBy, newSortOrder);
  }, [onSortChange]);

  const toggleSortOrder = useCallback(() => {
    const newOrder = sortOrder === 'asc' ? 'desc' : 'asc';
    setSortOrderState(newOrder);
    onSortChange?.(sortBy, newOrder);
  }, [sortBy, sortOrder, onSortChange]);

  const resetSort = useCallback(() => {
    setSortByState(initialSortBy);
    setSortOrderState(initialSortOrder);
    onSortChange?.(initialSortBy, initialSortOrder);
  }, [initialSortBy, initialSortOrder, onSortChange]);

  const sortValue = `${sortBy}-${sortOrder}`;

  return {
    sortBy,
    sortOrder,
    sortValue,
    setSortBy,
    setSortOrder,
    setSort,
    toggleSortOrder,
    resetSort,
  };
}

/**
 * Helper to parse combined sort value (e.g., "name-asc") into sortBy and sortOrder
 */
export function parseSortValue<T extends string>(value: string): { sortBy: T; sortOrder: 'asc' | 'desc' } {
  const lastDashIndex = value.lastIndexOf('-');
  const sortBy = value.substring(0, lastDashIndex) as T;
  const sortOrder = value.substring(lastDashIndex + 1) as 'asc' | 'desc';
  return { sortBy, sortOrder };
}
