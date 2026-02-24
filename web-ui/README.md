# GuardianStock - Smart Inventory Management

A professional, real-time inventory management system built with React, TypeScript, and a robust .NET backend integration.

## Tech Stack

- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **shadcn/ui** - UI components
- **React Query** - Server state management
- **React Router** - Routing
- **Recharts** - Data visualization

## Architecture

This project follows Clean Architecture principles with clear separation of concerns:

```
src/
├── api/              # API client and service layer
│   ├── client.ts     # HTTP client configuration
│   ├── config.ts     # API configuration (mock vs real)
│   ├── services/     # Domain-specific API services
│   └── types/        # API request/response types
├── components/       # React components
│   ├── common/       # Reusable UI components
│   ├── layout/       # Layout components
│   ├── features/     # Feature-specific components
│   └── ui/           # shadcn/ui base components
├── contexts/         # React contexts
├── hooks/            # Custom React hooks
├── mocks/            # Mock data (for development)
├── pages/            # Page components
├── types/            # TypeScript type definitions
└── lib/              # Utilities
```

## Getting Started

### Prerequisites

- Node.js 18+
- npm or bun

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

The app runs at `http://localhost:5173`

## Mock Data vs Real API

The app supports both mock data and real API calls:

```env
# .env
VITE_USE_MOCK_DATA=true   # Use mock data (default)
VITE_USE_MOCK_DATA=false  # Use real API
VITE_API_BASE_URL=/api    # API base URL
```

### Mock Data (Default)

By default, the app uses mock data for development. This allows frontend development without a backend.

### Real API

When ready to integrate with a backend:

1. Set `VITE_USE_MOCK_DATA=false`
2. Set `VITE_API_BASE_URL` to your API endpoint
3. Ensure your API matches the expected contracts in `api/types/`

## Key Features

### Data Management
- **Products** - Full CRUD with filtering, sorting, pagination
- **Categories** - Category management with product counts
- **Transactions** - Sales and purchase tracking
- **Alerts** - Low stock alert management
- **Users** - User management with roles

### UI Features
- Responsive design (mobile, tablet, desktop)
- Dark/Light theme support
- Real-time search with debouncing
- Advanced filtering and sorting
- Pagination with page size options

## Documentation

- [API Layer](src/api/README.md) - Services and API configuration
- [Hooks](src/hooks/README.md) - Custom React hooks
- [Components](src/components/README.md) - Reusable components

## Scripts

```bash
npm run dev       # Start development server
npm run build     # Build for production
npm run preview   # Preview production build
npm run lint      # Run ESLint
npm run test      # Run tests
```

## Scripts
