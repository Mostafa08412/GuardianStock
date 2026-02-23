/**
 * useFilters Hook
 * 
 * Generic filter state management hook.
 */

import { useState, useCallback, useMemo } from 'react';

export interface UseFiltersOptions<T extends Record<string, unknown>> {
  initialFilters: T;
  onFilterChange?: (filters: T) => void;
}

export interface UseFiltersReturn<T extends Record<string, unknown>> {
  filters: T;
  setFilter: <K extends keyof T>(key: K, value: T[K]) => void;
  setFilters: (filters: Partial<T>) => void;
  resetFilters: () => void;
  clearFilter: <K extends keyof T>(key: K) => void;
  hasActiveFilters: boolean;
  activeFilterCount: number;
}

export function useFilters<T extends Record<string, unknown>>({
  initialFilters,
  onFilterChange,
}: UseFiltersOptions<T>): UseFiltersReturn<T> {
  const [filters, setFiltersState] = useState<T>(initialFilters);

  const setFilter = useCallback(<K extends keyof T>(key: K, value: T[K]) => {
    setFiltersState(prev => {
      const next = { ...prev, [key]: value };
      onFilterChange?.(next);
      return next;
    });
  }, [onFilterChange]);

  const setFilters = useCallback((newFilters: Partial<T>) => {
    setFiltersState(prev => {
      const next = { ...prev, ...newFilters };
      onFilterChange?.(next);
      return next;
    });
  }, [onFilterChange]);

  const resetFilters = useCallback(() => {
    setFiltersState(initialFilters);
    onFilterChange?.(initialFilters);
  }, [initialFilters, onFilterChange]);

  const clearFilter = useCallback(<K extends keyof T>(key: K) => {
    setFiltersState(prev => {
      const next = { ...prev, [key]: initialFilters[key] };
      onFilterChange?.(next);
      return next;
    });
  }, [initialFilters, onFilterChange]);

  const { hasActiveFilters, activeFilterCount } = useMemo(() => {
    let count = 0;
    
    for (const key in filters) {
      const currentValue = filters[key];
      const initialValue = initialFilters[key];
      
      // Check if value differs from initial
      if (currentValue !== initialValue) {
        // Skip 'all' values as they typically mean "no filter"
        if (currentValue !== 'all' && currentValue !== '' && currentValue !== undefined && currentValue !== null) {
          count++;
        }
      }
    }
    
    return {
      hasActiveFilters: count > 0,
      activeFilterCount: count,
    };
  }, [filters, initialFilters]);

  return {
    filters,
    setFilter,
    setFilters,
    resetFilters,
    clearFilter,
    hasActiveFilters,
    activeFilterCount,
  };
}
