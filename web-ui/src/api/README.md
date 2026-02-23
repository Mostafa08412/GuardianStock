# API Layer

This directory contains the API client and service layer for the application.

## Structure

```
api/
├── client.ts           # HTTP client wrapper
├── config.ts           # API configuration (mock vs real)
├── services/           # Domain-specific API services
│   ├── index.ts        # Service exports
│   ├── products.service.ts
│   ├── categories.service.ts
│   ├── transactions.service.ts
│   ├── alerts.service.ts
│   ├── users.service.ts
│   └── dashboard.service.ts
└── types/              # API request/response types
    └── index.ts
```

## Configuration

### Mock Data vs Real API

The application supports both mock data and real API calls. This is controlled by the `VITE_USE_MOCK_DATA` environment variable:

```env
# .env
VITE_USE_MOCK_DATA=true  # Use mock data (default)
VITE_USE_MOCK_DATA=false # Use real API
VITE_API_BASE_URL=/api   # API base URL (optional)
```

### How It Works

Each service automatically checks the configuration and either:
1. Returns mock data with simulated network delay
2. Makes real HTTP requests to the API

```typescript
// Example service pattern
export const productsService = {
  getList: async (params) => {
    if (API_CONFIG.useMockData) {
      await delay(API_CONFIG.mockDelay);
      return filterMockProducts(mockProducts, params);
    }
    return apiClient.get('/products', { params });
  },
};
```

## Usage

### Basic Usage

```typescript
import { productsService } from '@/api/services';

// In a component
const products = await productsService.getList({ page: 1, pageSize: 10 });
```

### With React Query (Recommended)

```typescript
import { useQuery } from '@tanstack/react-query';
import { productsService } from '@/api/services';

function ProductsList() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['products', params],
    queryFn: () => productsService.getList(params),
  });
}
```

## Services Reference

### productsService
- `getList(params)` - Get paginated list of products
- `getById(id)` - Get single product
- `create(data)` - Create new product
- `update(data)` - Update existing product
- `delete(id)` - Delete product
- `getSuppliers()` - Get unique suppliers

### categoriesService
- `getList(params)` - Get paginated list of categories
- `getAll()` - Get all categories (no pagination)
- `getById(id)` - Get single category
- `getProducts(categoryId)` - Get products in category
- `create(data)` - Create new category
- `update(data)` - Update existing category
- `delete(id)` - Delete category

### transactionsService
- `getList(params)` - Get paginated transactions
- `getById(id)` - Get single transaction
- `getByProduct(productId)` - Get transactions for product
- `create(data)` - Create new transaction
- `getUsers()` - Get unique users
- `getRecent(limit)` - Get recent transactions

### alertsService
- `getList(params)` - Get paginated alerts
- `getById(id)` - Get single alert
- `dismiss(id)` - Dismiss an alert
- `getStats()` - Get alert statistics

### usersService
- `getList(params)` - Get paginated users
- `getAll()` - Get all users
- `getById(id)` - Get single user
- `create(data)` - Create new user
- `update(data)` - Update existing user
- `delete(id)` - Delete user

### dashboardService
- `getStats()` - Get dashboard statistics
- `getRecentTransactions(limit)` - Get recent transactions
- `getSalesChartData()` - Get sales chart data
- `getCategoryDistribution()` - Get category distribution
- `getTopProducts()` - Get top products

## Types

All API types are defined in `api/types/index.ts`:

- `PaginatedResponse<T>` - Standard paginated response
- `GetProductsParams`, `GetCategoriesParams`, etc. - Request parameters
- `CreateProductRequest`, `UpdateProductRequest`, etc. - Mutation payloads

## Switching to Real API

When the backend is ready:

1. Set `VITE_USE_MOCK_DATA=false` in `.env`
2. Set `VITE_API_BASE_URL` to your API endpoint
3. Optionally remove mock data conditionals from services

The API contracts are already defined - the backend just needs to match the expected request/response formats.
