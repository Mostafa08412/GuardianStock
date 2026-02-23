export interface DashboardStats {
    totalProducts: number;
    totalStockValue: number;
    totalSales: number;
    lowStockCount: number;
    recentTransactions: DashboardTransaction[];
}

export interface DashboardTransaction {
    id: string;
    productName: string;
    quantity: number;
    type: string;
    date: string;
    totalAmount: number;
    userName: string;
    productId: string;
}

export interface SalesChartData {
    month: number;
    year: number;
    sales: number;
    purchases: number;
}

export interface CategoryDistribution {
    name: string;
    value: number;
}

export interface TopProduct {
    id: string;
    name: string;
    imageUrl?: string;
    sales: number;
    revenue: number;
}

export interface LowStockInventory {
    inventoryId: string;
    productId: string;
    productName: string;
    status: string;
    stock: number;
    threshold: number;
}

export interface DashboardData {
    stats: DashboardStats;
    salesChartData: SalesChartData[];
    categoryDistribution: CategoryDistribution[];
    topProducts: TopProduct[];
    lowStockInventories: LowStockInventory[];
}
