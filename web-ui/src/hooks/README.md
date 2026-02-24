# Hooks

This directory contains custom React hooks for the application.

## Structure

```
hooks/
├── index.ts           # Hook exports
├── usePagination.ts   # Pagination state management
├── useFilters.ts      # Filter state management
├── useSorting.ts      # Sorting state management
├── useDebounce.ts     # Debounce utilities
├── useDataTable.ts    # Combined data table hook
├── use-mobile.tsx     # Mobile detection
└── use-toast.ts       # Toast notifications
```

## Core Hooks

### usePagination

Manages pagination state including page, page size, and navigation.

```typescript
import { usePagination } from '@/hooks';

const {
  page,
  pageSize,
  totalPages,
  setPage,
  setPageSize,
  nextPage,
  prevPage,
  hasNextPage,
  hasPrevPage,
  pageNumbers,  // For rendering pagination UI
} = usePagination({
  initialPage: 1,
  initialPageSize: 10,
  totalCount: 100,
});
```

### useFilters

Generic filter state management with type safety.

```typescript
import { useFilters } from '@/hooks';

interface MyFilters {
  category: string;
  status: 'all' | 'active' | 'inactive';
  search: string;
}

const {
  filters,
  setFilter,
  setFilters,
  resetFilters,
  clearFilter,
  hasActiveFilters,
  activeFilterCount,
} = useFilters<MyFilters>({
  initialFilters: {
    category: 'all',
    status: 'all',
    search: '',
  },
});

// Update single filter
setFilter('category', 'electronics');

// Update multiple filters
setFilters({ category: 'electronics', status: 'active' });
```

### useSorting

Manages sorting state with sortBy and sortOrder.

```typescript
import { useSorting, parseSortValue } from '@/hooks';

const {
  sortBy,
  sortOrder,
  sortValue,       // Combined: "name-asc"
  setSortBy,
  setSortOrder,
  setSort,
  toggleSortOrder,
  resetSort,
} = useSorting({
  initialSortBy: 'name',
  initialSortOrder: 'asc',
});

// For select inputs
<Select value={sortValue} onValueChange={(v) => {
  const { sortBy, sortOrder } = parseSortValue(v);
  setSort(sortBy, sortOrder);
}}>
```

### useDebounce

Debounce values to prevent excessive updates.

```typescript
import { useDebounce, useDebouncedCallback, useDebouncedSearch } from '@/hooks';

// Debounce a value
const debouncedSearch = useDebounce(searchQuery, 300);

// Debounce a callback
const debouncedFn = useDebouncedCallback((value) => {
  console.log(value);
}, 300);

// Debounced search with loading state
const { value, debouncedValue, setValue, isSearching } = useDebouncedSearch('', 300);
```

### useDataTable

Combines pagination, filtering, sorting, and search for table data management.

```typescript
import { useDataTable } from '@/hooks';

interface ProductFilters {
  category: string;
  stockFilter: 'all' | 'in-stock' | 'low' | 'critical';
}

const table = useDataTable<Product, ProductFilters, 'name' | 'price' | 'stock'>({
  totalCount: 100,
  initialFilters: {
    category: 'all',
    stockFilter: 'all',
  },
  initialSortBy: 'name',
  initialSortOrder: 'asc',
  initialPageSize: 10,
});

// Use in component
<input value={table.searchQuery} onChange={(e) => table.setSearchQuery(e.target.value)} />
<select value={table.filters.category} onChange={(e) => table.setFilter('category', e.target.value)} />
<Pagination 
  page={table.page}
  pageSize={table.pageSize}
  totalPages={table.totalPages}
  onPageChange={table.setPage}
/>
```

## Utility Hooks

### use-mobile

Detects mobile viewport using media query.

```typescript
import { useIsMobile } from '@/hooks/use-mobile';

const isMobile = useIsMobile();
```

### use-toast

Toast notification system.

```typescript
import { toast, useToast } from '@/hooks/use-toast';

// Imperative
toast({ title: 'Success', description: 'Action completed' });

// Hook
const { toast: showToast, dismiss } = useToast();
```

## Best Practices

1. **Compose hooks** - Use `useDataTable` for complete table functionality
2. **Reset pagination** - Always reset to page 1 when filters change
3. **Debounce search** - Use `useDebounce` for search inputs
4. **Type your filters** - Define filter interfaces for type safety
