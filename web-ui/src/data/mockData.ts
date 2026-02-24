import { Product, Category, Transaction, LowStockAlert, User } from '@/types/inventory';
import { DashboardStats } from '@/types/dashboard';

export const mockUsers: User[] = [
  { id: '1', name: 'Sarah Chen', email: 'sarah@company.com', role: 'admin', avatar: 'SC', status: 'active', createdAt: new Date('2023-06-15'), lastLogin: new Date('2024-01-21') },
  { id: '2', name: 'Michael Torres', email: 'michael@company.com', role: 'manager', avatar: 'MT', status: 'active', createdAt: new Date('2023-08-20'), lastLogin: new Date('2024-01-20') },
  { id: '3', name: 'Emily Johnson', email: 'emily@company.com', role: 'staff', avatar: 'EJ', status: 'active', createdAt: new Date('2023-10-01'), lastLogin: new Date('2024-01-19') },
  { id: '4', name: 'David Kim', email: 'david@company.com', role: 'staff', avatar: 'DK', status: 'active', createdAt: new Date('2023-11-10'), lastLogin: new Date('2024-01-18') },
  { id: '5', name: 'Jessica Wang', email: 'jessica@company.com', role: 'manager', avatar: 'JW', status: 'inactive', createdAt: new Date('2023-07-05') },
];

export const mockCategories: Category[] = [
  { id: '1', name: 'Electronics', description: 'Electronic devices and accessories', productCount: 45 },
  { id: '2', name: 'Office Supplies', description: 'Office equipment and supplies', productCount: 128 },
  { id: '3', name: 'Furniture', description: 'Office and home furniture', productCount: 34 },
  { id: '4', name: 'Software', description: 'Software licenses and subscriptions', productCount: 22 },
  { id: '5', name: 'Hardware', description: 'Computer hardware components', productCount: 67 },
];

export const mockProducts: Product[] = [
  { id: '1', sku: 'MBP-16-M3P', name: 'MacBook Pro 16"', description: 'Apple M3 Pro, 18GB RAM', price: 2499, stockQuantity: 15, currentStock: 15, categoryId: '1', supplier: 'Apple Inc.', createdAt: '2024-01-15T10:00:00Z', lastUpdatedAt: '2024-01-20T10:00:00Z', lowStockThreshold: 5 },
  { id: '2', sku: 'DELL-US27-4K', name: 'Dell UltraSharp 27"', description: '4K USB-C Hub Monitor', price: 699, stockQuantity: 3, currentStock: 3, categoryId: '1', supplier: 'Dell Technologies', createdAt: '2024-01-10T10:00:00Z', lastUpdatedAt: '2024-01-18T10:00:00Z', lowStockThreshold: 10 },
  { id: '3', sku: 'HM-AERON-BLK', name: 'Herman Miller Aeron', description: 'Ergonomic Office Chair', price: 1395, stockQuantity: 8, currentStock: 8, categoryId: '3', supplier: 'Herman Miller', createdAt: '2024-01-05T10:00:00Z', lastUpdatedAt: '2024-01-12T10:00:00Z', lowStockThreshold: 5 },
  { id: '4', sku: 'LOG-MX3S-BLK', name: 'Logitech MX Master 3S', description: 'Wireless Performance Mouse', price: 99, stockQuantity: 42, currentStock: 42, categoryId: '1', supplier: 'Logitech', createdAt: '2024-01-08T10:00:00Z', lastUpdatedAt: '2024-01-15T10:00:00Z', lowStockThreshold: 20 },
  { id: '5', sku: 'KEY-MECH-PRO', name: 'Mechanical Keyboard Pro', description: 'Cherry MX Brown Switches', price: 179, stockQuantity: 2, currentStock: 2, categoryId: '1', supplier: 'Keychron', createdAt: '2024-01-12T10:00:00Z', lastUpdatedAt: '2024-01-19T10:00:00Z', lowStockThreshold: 10 },
  { id: '6', sku: 'UPL-DSK-FRM', name: 'Standing Desk Frame', description: 'Electric Height Adjustable', price: 549, stockQuantity: 18, currentStock: 18, categoryId: '3', supplier: 'Uplift Desk', createdAt: '2024-01-03T10:00:00Z', lastUpdatedAt: '2024-01-10T10:00:00Z', lowStockThreshold: 8 },
  { id: '7', sku: 'ANK-HUB-10P', name: 'USB-C Hub 10-in-1', description: 'Multiport Adapter', price: 79, stockQuantity: 56, currentStock: 56, categoryId: '5', supplier: 'Anker', createdAt: '2024-01-14T10:00:00Z', lastUpdatedAt: '2024-01-21T10:00:00Z', lowStockThreshold: 25 },
  { id: '8', sku: 'MOL-NB-100P', name: 'Notebook Pack (100)', description: 'A5 Lined Notebooks', price: 45, stockQuantity: 234, currentStock: 234, categoryId: '2', supplier: 'Moleskine', createdAt: '2024-01-06T10:00:00Z', lastUpdatedAt: '2024-01-13T10:00:00Z', lowStockThreshold: 50 },
  { id: '9', sku: 'LOG-WBC-4KP', name: 'Webcam 4K Pro', description: 'Ultra HD Streaming Camera', price: 199, stockQuantity: 4, currentStock: 4, categoryId: '1', supplier: 'Logitech', createdAt: '2024-01-11T10:00:00Z', lastUpdatedAt: '2024-01-18T10:00:00Z', lowStockThreshold: 10 },
  { id: '10', sku: 'MS-O365-BUS', name: 'Office 365 License', description: 'Annual Business Subscription', price: 299, stockQuantity: 150, currentStock: 150, categoryId: '4', supplier: 'Microsoft', createdAt: '2024-01-01T10:00:00Z', lastUpdatedAt: '2024-01-08T10:00:00Z', lowStockThreshold: 20 },
];

export const mockTransactions: Transaction[] = [
  { id: '1', productSku: 'MBP-16-M3P', productName: 'MacBook Pro 16"', quantity: 2, type: 'sale', date: '2024-01-21T00:00:00Z', userName: 'Emily Johnson', amount: 4998 },
  { id: '2', productSku: 'LOG-MX3S-BLK', productName: 'Logitech MX Master 3S', quantity: 10, type: 'purchase', date: '2024-01-20T00:00:00Z', userName: 'Michael Torres', amount: 990 },
  { id: '3', productSku: 'DELL-US27-4K', productName: 'Dell UltraSharp 27"', quantity: 1, type: 'sale', date: '2024-01-20T00:00:00Z', userName: 'Emily Johnson', amount: 699 },
  { id: '4', productSku: 'MOL-NB-100P', productName: 'Notebook Pack (100)', quantity: 50, type: 'purchase', date: '2024-01-19T00:00:00Z', userName: 'Michael Torres', amount: 2250 },
  { id: '5', productSku: 'HM-AERON-BLK', productName: 'Herman Miller Aeron', quantity: 3, type: 'sale', date: '2024-01-19T00:00:00Z', userName: 'Emily Johnson', amount: 4185 },
  { id: '6', productSku: 'KEY-MECH-PRO', productName: 'Mechanical Keyboard Pro', quantity: 5, type: 'sale', date: '2024-01-18T00:00:00Z', userName: 'Emily Johnson', amount: 895 },
  { id: '7', productSku: 'ANK-HUB-10P', productName: 'USB-C Hub 10-in-1', quantity: 20, type: 'purchase', date: '2024-01-17T00:00:00Z', userName: 'Michael Torres', amount: 1580 },
  { id: '8', productSku: 'LOG-WBC-4KP', productName: 'Webcam 4K Pro', quantity: 2, type: 'sale', date: '2024-01-17T00:00:00Z', userName: 'Emily Johnson', amount: 398 },
];

export const mockAlerts: LowStockAlert[] = [
  { id: '1', productId: '2', productName: 'Dell UltraSharp 27"', currentStock: 3, threshold: 10, alertSent: true, date: new Date('2024-01-20'), isDismissed: false },
  { id: '2', productId: '5', productName: 'Mechanical Keyboard Pro', currentStock: 2, threshold: 10, alertSent: true, date: new Date('2024-01-19'), isDismissed: false },
  { id: '3', productId: '9', productName: 'Webcam 4K Pro', currentStock: 4, threshold: 10, alertSent: false, date: new Date('2024-01-18'), isDismissed: false },
];

export const mockDashboardStats: DashboardStats = {
  totalProducts: 296,
  totalStockValue: 847500,
  totalSales: 156780,
  lowStockCount: 3,
  recentTransactions: mockTransactions.slice(0, 5).map(t => ({
    ...t,
    totalAmount: t.amount,
    productId: 'dummy-id' // Transaction interface doesn't have productId, but DashboardTransaction requires it
  })),
};

export const salesChartData = [
  { name: 'Jan', sales: 45000, purchases: 32000 },
  { name: 'Feb', sales: 52000, purchases: 28000 },
  { name: 'Mar', sales: 48000, purchases: 35000 },
  { name: 'Apr', sales: 61000, purchases: 42000 },
  { name: 'May', sales: 55000, purchases: 38000 },
  { name: 'Jun', sales: 67000, purchases: 45000 },
  { name: 'Jul', sales: 72000, purchases: 48000 },
];

export const categoryDistributionData = [
  { name: 'Electronics', value: 45, fill: 'hsl(173 80% 40%)' },
  { name: 'Office Supplies', value: 128, fill: 'hsl(142 76% 36%)' },
  { name: 'Furniture', value: 34, fill: 'hsl(38 92% 50%)' },
  { name: 'Software', value: 22, fill: 'hsl(199 89% 48%)' },
  { name: 'Hardware', value: 67, fill: 'hsl(280 65% 60%)' },
];

export const topProductsData = [
  { name: 'MacBook Pro 16"', sales: 45, revenue: 112455 },
  { name: 'Herman Miller Aeron', sales: 38, revenue: 53010 },
  { name: 'Dell UltraSharp 27"', sales: 32, revenue: 22368 },
  { name: 'Logitech MX Master 3S', sales: 89, revenue: 8811 },
  { name: 'USB-C Hub 10-in-1', sales: 124, revenue: 9796 },
];
