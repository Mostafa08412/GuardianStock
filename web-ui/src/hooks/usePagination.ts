/**
 * usePagination Hook
 * 
 * Manages pagination state and logic.
 */

import { useState, useMemo, useCallback } from 'react';

export interface PaginationState {
  page: number;
  pageSize: number;
}

export interface PaginationActions {
  setPage: (page: number) => void;
  setPageSize: (pageSize: number) => void;
  nextPage: () => void;
  prevPage: () => void;
  reset: () => void;
}

export interface UsePaginationOptions {
  initialPage?: number;
  initialPageSize?: number;
  totalCount?: number;
}

export interface UsePaginationReturn extends PaginationState, PaginationActions {
  totalPages: number;
  startIndex: number;
  endIndex: number;
  hasNextPage: boolean;
  hasPrevPage: boolean;
  pageNumbers: (number | 'ellipsis')[];
}

export function usePagination({
  initialPage = 1,
  initialPageSize = 10,
  totalCount = 0,
}: UsePaginationOptions = {}): UsePaginationReturn {
  const [page, setPageState] = useState(initialPage);
  const [pageSize, setPageSizeState] = useState(initialPageSize);

  const totalPages = useMemo(() => 
    Math.max(1, Math.ceil(totalCount / pageSize)), 
    [totalCount, pageSize]
  );

  const setPage = useCallback((newPage: number) => {
    setPageState(Math.max(1, Math.min(newPage, totalPages)));
  }, [totalPages]);

  const setPageSize = useCallback((newPageSize: number) => {
    setPageSizeState(newPageSize);
    setPageState(1); // Reset to first page when page size changes
  }, []);

  const nextPage = useCallback(() => {
    setPage(page + 1);
  }, [page, setPage]);

  const prevPage = useCallback(() => {
    setPage(page - 1);
  }, [page, setPage]);

  const reset = useCallback(() => {
    setPageState(initialPage);
    setPageSizeState(initialPageSize);
  }, [initialPage, initialPageSize]);

  const startIndex = (page - 1) * pageSize;
  const endIndex = Math.min(startIndex + pageSize, totalCount);

  const hasNextPage = page < totalPages;
  const hasPrevPage = page > 1;

  // Generate page numbers with ellipsis
  const pageNumbers = useMemo((): (number | 'ellipsis')[] => {
    const pages: (number | 'ellipsis')[] = [];
    
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      if (page <= 3) {
        pages.push(1, 2, 3, 4, 'ellipsis', totalPages);
      } else if (page >= totalPages - 2) {
        pages.push(1, 'ellipsis', totalPages - 3, totalPages - 2, totalPages - 1, totalPages);
      } else {
        pages.push(1, 'ellipsis', page - 1, page, page + 1, 'ellipsis', totalPages);
      }
    }
    
    return pages;
  }, [page, totalPages]);

  return {
    page,
    pageSize,
    totalPages,
    startIndex,
    endIndex,
    hasNextPage,
    hasPrevPage,
    pageNumbers,
    setPage,
    setPageSize,
    nextPage,
    prevPage,
    reset,
  };
}
