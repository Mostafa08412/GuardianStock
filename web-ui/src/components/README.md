# Components

This directory contains all React components for the application.

## Structure

```
components/
├── common/             # Generic reusable components
│   ├── PageHeader.tsx
│   ├── EmptyState.tsx
│   ├── LoadingOverlay.tsx
│   ├── SearchInput.tsx
│   ├── PaginationFooter.tsx
│   ├── FilterPanel.tsx
│   ├── DataCard.tsx
│   ├── StatusBadge.tsx
│   ├── ActionButtons.tsx
│   └── index.ts
├── layout/             # Layout components
│   ├── Header.tsx
│   ├── Sidebar.tsx
│   └── MobileSidebar.tsx
├── dashboard/          # Dashboard-specific components
│   ├── StatCard.tsx
│   ├── SalesChart.tsx
│   └── ...
├── products/           # Product feature components
├── categories/         # Category feature components
├── transactions/       # Transaction feature components
├── users/              # User feature components
└── ui/                 # shadcn/ui components
```

## Common Components

### PageHeader

Consistent page headers with title, description, and actions.

```tsx
import { PageHeader } from '@/components/common';
import { Plus } from 'lucide-react';

<PageHeader
  title="Products"
  description="Manage your inventory"
  icon={Package}
  actions={
    <Button onClick={handleAdd}>
      <Plus className="w-4 h-4 mr-2" />
      Add Product
    </Button>
  }
/>
```

### EmptyState

Displays a placeholder when no data is available.

```tsx
import { EmptyState } from '@/components/common';
import { Package } from 'lucide-react';

<EmptyState
  icon={Package}
  title="No products found"
  description="Try adjusting your filters"
  action={{
    label: 'Add Product',
    onClick: handleAdd,
  }}
/>
```

### LoadingOverlay

Loading indicator overlay for async operations.

```tsx
import { LoadingOverlay, LoadingSpinner, LoadingState } from '@/components/common';

// Overlay on content
<LoadingOverlay isLoading={loading}>
  <Table>...</Table>
</LoadingOverlay>

// Inline spinner
<LoadingSpinner size="md" />

// Full loading state
<LoadingState text="Loading products..." />
```

### SearchInput

Search input with icon and clear button.

```tsx
import { SearchInput } from '@/components/common';

<SearchInput
  value={searchQuery}
  onChange={setSearchQuery}
  placeholder="Search products..."
/>
```

### PaginationFooter

Complete pagination controls with page size selector.

```tsx
import { PaginationFooter } from '@/components/common';

<PaginationFooter
  currentPage={page}
  pageSize={pageSize}
  totalCount={totalCount}
  totalPages={totalPages}
  onPageChange={setPage}
  onPageSizeChange={setPageSize}
  pageSizeOptions={[5, 10, 20, 50]}
/>
```

### FilterPanel

Container for filter controls with clear functionality.

```tsx
import { FilterPanel, FilterRow } from '@/components/common';

<FilterPanel
  activeFilterCount={3}
  onClearAll={resetFilters}
>
  <SearchInput ... />
  <FilterRow>
    <Select ... />
    <Select ... />
  </FilterRow>
</FilterPanel>
```

### DataCard

Reusable card for mobile/tablet data display.

```tsx
import { DataCard, DataCardHeader, DataCardContent, DataCardFooter } from '@/components/common';

<DataCard onClick={handleClick}>
  <DataCardHeader>
    <h3>{product.name}</h3>
    <StatusBadge variant="success">In Stock</StatusBadge>
  </DataCardHeader>
  <DataCardContent>
    <p>{product.description}</p>
  </DataCardContent>
  <DataCardFooter>
    <span>${product.price}</span>
    <ActionButtons onEdit={handleEdit} onDelete={handleDelete} />
  </DataCardFooter>
</DataCard>
```

### StatusBadge

Status indicator with consistent styling.

```tsx
import { StatusBadge, getStockStatus, getUserStatus } from '@/components/common';

// Direct usage
<StatusBadge variant="success">Active</StatusBadge>
<StatusBadge variant="warning">Low Stock</StatusBadge>
<StatusBadge variant="danger">Critical</StatusBadge>

// With helpers
const status = getStockStatus(quantity, threshold);
<StatusBadge variant={status.variant}>{status.label}</StatusBadge>
```

### ActionButtons

Reusable action button group.

```tsx
import { ActionButtons, ActionButton } from '@/components/common';

// Standard actions
<ActionButtons
  onView={handleView}
  onEdit={handleEdit}
  onDelete={handleDelete}
/>

// Custom action
<ActionButton
  icon={Download}
  onClick={handleDownload}
  label="Download"
/>
```

## Component Patterns

### Single Responsibility
Each component does one thing well. Complex UIs are built by composing simple components.

### Composition over Configuration
Build complex UIs from simple components rather than passing many props.

### Controlled Components
Most components are controlled - state is managed by parent.

### Consistent Styling
All components use Tailwind CSS with design tokens from the theme.

## Creating New Components

1. Create file in appropriate directory
2. Export from index.ts
3. Use TypeScript interfaces for props
4. Follow existing naming conventions
5. Add JSDoc comments for complex components
