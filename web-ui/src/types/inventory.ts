export type UserRole = 'admin' | 'manager' | 'staff';

export type UserStatus = 'active' | 'inactive';

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  status: UserStatus;
  avatar?: string;
  createdAt: Date;
  lastLogin?: Date;
}

export enum TransactionType {
  Purchase = 0,
  Sale = 1,
  Adjustment = 2,
  Return = 3
}

export interface StockActivity {
  type: TransactionType;
  quantity: number;
  date: string; // ISO string from API
}

export interface StockHistoryDTO {
  quantity: number;
  month: number;
  year: number;
}

export interface Product {
  id: string;
  name: string;
  description?: string;
  supplier?: string;
  categoryName?: string;
  createdAt?: string; // ISO string
  lastUpdatedAt?: string; // ISO string
  totalValue?: number; // ReadOnly
  stockStatus?: string; // ReadOnly
  sku?: string;
  price: number;
  stockQuantity: number; // Renamed from currentStock to match API
  lowStockThreshold: number;
  imageUrl?: string;

  // Analytics
  twoMonthsAgoSales?: number;
  lastMonthSales?: number;
  currentMonthSales?: number;
  avgReStockTime?: number;
  lastRestocked?: string; // ISO string

  // Relations
  recentActivities?: StockActivity[];
  stockHistory?: StockHistoryDTO[];

  // Legacy fields for backward compatibility (optional)
  currentStock?: number;
  categoryId: string; // From getById service mapping or if needed for updates
  updatedAt?: Date; // Mapped from lastUpdatedAt in service
}

export interface ProductListItem {
  id: string;
  sku?: string;
  name: string;
  price: number;
  currentStock: number; // API key is currentStock for list item
  lowStockThreshold: number;
  supplier?: string;
  categoryName?: string;
  categoryId: string;
  description?: string;
  imageUrl?: string;
}

export interface CategoryListItem {
  id: string;
  name: string;
  description: string;
  productCount: number;
}

export interface CategoryDetails {
  id: string;
  name: string;
  description: string;
  productsCount: number;
  totalValue: number;
  averagePrice: number;
  totalStock: number;
}

// Alias for backward compatibility during refactor, prioritizing ListItem as default for lists
export type Category = CategoryListItem;

export interface Transaction {
  id: string;
  productName: string;
  productSku: string;
  quantity: number;
  type: string;
  date: string;
  userName: string;
  amount: number;
}

export interface TransactionDetails {
  id: string;
  productName: string;
  productSku: string;
  unitPrice: number;
  quantity: number;
  totalAmount: number;
  transactionType: string;
  createdDate: string;
  createdByUser: string;
}

export interface LowStockAlert {
  id: string;
  productId: string;
  productName: string;
  currentStock: number;
  threshold: number;
  alertSent: boolean;
  isDismissed: boolean;
  dismissedAt?: Date;
  date: Date;
}

export interface LowStockAlertListItem {
  inventoryId: string;
  productId: string;
  productName: string;
  sku: string;
  currentStock: number;
  threshold: number;
  isNotificationSent: boolean;
  isDismissed: boolean;
  dismissedAt?: Date;
  triggeredOn: Date;
  status: string;
}

// V3 Interfaces
export interface LowStockAlertListItemDto {
  inventoryId: string;
  productId: string;
  productName: string;
  sku: string;
  currentStock: number;
  threshold: number;
  status: string;
  isNotificationSent: boolean;
  isDismissed: boolean;
  triggeredOn: string; // ISO string
}

export interface InventoryListItemDto {
  id: string;
  productId: string;
  productName: string;
  productSku: string;
  productPrice: number;
  stock: number;
  lowStockThreshold: number;
  stockStatus: string;
  imageUrl?: string;
}

export interface InventoryTransactionDto {
  id: string;
  type: TransactionType;
  quantity: number;
  totalAmount: number;
  date: string; // ISO string
}

export interface InventoryDetailsDto {
  id: string;
  productId: string;
  productName: string;
  productSku: string;
  productPrice: number;
  supplier: string;
  stock: number;
  lowStockThreshold: number;
  stockStatus: string;
  stockValue: number;
  imageUrl?: string;
  shortageQuantity: number;
  alertTriggeredAt?: string;
  isNotificationSent: boolean;
  isDismissed: boolean;
  dismissedAt?: string;
  createdAt: string;
  lastUpdatedAt: string;
  recentTransactions: InventoryTransactionDto[];
}

export interface StockSummaryDto {
  totalLowStock: number;
  totalCriticalStock: number;
  totalNormalStock: number;
}

export interface AdjustLowStockThresholdCommand {
  inventoryId: string;
  newLowStockThreshold: number;
}

export interface Inventory {
  id: string;
  productId: string;
  productName: string;
  sku: string;
  currentStock: number;
  lowStockThreshold: number;
  lastUpdated: Date;
  status: string; // 'Normal', 'Low', 'Critical'
  isDismissed?: boolean;
  dismissedAt?: Date;
}






export interface UpdateProductRequest {
  id: string;
  name: string;
  description?: string;
  price: number;
  supplier?: string;
  categoryId: string;
  lowStockAlertThreshold: number;
}
