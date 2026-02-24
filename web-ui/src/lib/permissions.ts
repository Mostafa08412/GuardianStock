import { UserRole } from '@/types/inventory';

// Define page access by role
export const PAGE_PERMISSIONS: Record<string, UserRole[]> = {
  dashboard: ['admin', 'manager', 'staff'],
  products: ['admin', 'manager', 'staff'],
  transactions: ['admin', 'manager', 'staff'],
  alerts: ['admin', 'manager'],
  inventories: ['admin', 'manager', 'staff'],
  reports: ['admin', 'manager'],
  categories: ['admin', 'manager'],
  users: ['admin'],
  settings: ['admin', 'manager'],
};

// Define action permissions by role
export const ACTION_PERMISSIONS = {
  // Product actions
  'products.create': ['admin', 'manager'] as UserRole[],
  'products.edit': ['admin', 'manager'] as UserRole[],
  'products.delete': ['admin'] as UserRole[],
  'products.import': ['admin', 'manager'] as UserRole[],

  // Transaction actions
  'transactions.create': ['admin', 'manager', 'staff'] as UserRole[],
  'transactions.edit': ['admin', 'manager'] as UserRole[],
  'transactions.delete': ['admin'] as UserRole[],

  // Category actions
  'categories.create': ['admin'] as UserRole[],
  'categories.edit': ['admin'] as UserRole[],
  'categories.delete': ['admin'] as UserRole[],

  // User management actions
  'users.view': ['admin'] as UserRole[],
  'users.create': ['admin'] as UserRole[],
  'users.edit': ['admin'] as UserRole[],
  'users.delete': ['admin'] as UserRole[],
  'users.changeRole': ['admin'] as UserRole[],

  // Alert actions
  'alerts.acknowledge': ['admin', 'manager'] as UserRole[],
  'alerts.resolve': ['admin', 'manager'] as UserRole[],

  // Inventory actions
  'inventories.adjustThreshold': ['admin', 'manager'] as UserRole[],

  // Settings
  'settings.view': ['admin', 'manager'] as UserRole[],
  'settings.edit': ['admin'] as UserRole[],
};

export type ActionPermission = keyof typeof ACTION_PERMISSIONS;

export function hasPageAccess(page: string, role: UserRole): boolean {
  const allowedRoles = PAGE_PERMISSIONS[page];
  if (!allowedRoles) return true; // Allow access if page not defined
  return allowedRoles.includes(role);
}

export function hasActionPermission(action: ActionPermission, role: UserRole): boolean {
  const allowedRoles = ACTION_PERMISSIONS[action];
  if (!allowedRoles) return false;
  return allowedRoles.includes(role);
}

export function getAccessiblePages(role: UserRole): string[] {
  return Object.entries(PAGE_PERMISSIONS)
    .filter(([_, roles]) => roles.includes(role))
    .map(([page]) => page);
}

// Get role display info
export const ROLE_INFO: Record<UserRole, { label: string; description: string; color: string }> = {
  admin: {
    label: 'Administrator',
    description: 'Full access to all features and settings',
    color: 'destructive',
  },
  manager: {
    label: 'Manager',
    description: 'Manage products, transactions, and view reports',
    color: 'warning',
  },
  staff: {
    label: 'Staff',
    description: 'View products and create transactions',
    color: 'info',
  },
};
