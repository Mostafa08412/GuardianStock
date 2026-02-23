/**
 * Dashboard Page (Refactored)
 * 
 * Clean, composition-based dashboard using the new architecture.
 */

import { useState, useEffect } from 'react';
import { Package, DollarSign, TrendingUp, AlertTriangle, Loader2 } from 'lucide-react';
import StatCard from '@/components/dashboard/StatCard';
import SalesChart from '@/components/dashboard/SalesChart';
import CategoryChart from '@/components/dashboard/CategoryChart';
import RecentTransactions from '@/components/dashboard/RecentTransactions';
import LowStockPanel from '@/components/dashboard/LowStockPanel';
import TopProducts from '@/components/dashboard/TopProducts';
import { usePermissions } from '@/hooks/usePermissions';
import { dashboardService } from '@/api/services/dashboard.service';
import { DashboardData } from '@/types';
import { cn } from '@/lib/utils';
import { toast } from 'sonner';

import { useNavigate } from 'react-router-dom';

export default function Dashboard() {
  const navigate = useNavigate();
  const { canAccessPage } = usePermissions();
  const [data, setData] = useState<DashboardData | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        const result = await dashboardService.getDashboardData();
        setData(result);
      } catch (error) {
        console.error('Failed to fetch dashboard data:', error);
        toast.error('Failed to load dashboard data');
      } finally {
        setIsLoading(false);
      }
    };
    fetchDashboard();
  }, []);

  if (isLoading) {
    return (
      <div className="flex h-96 items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!data) {
    return null;
  }

  const { stats } = data;
  const canSeeAlerts = canAccessPage('alerts');
  const canSeeCategories = canAccessPage('categories');
  const canSeeReports = canAccessPage('reports');
  const hasRightPanel = canSeeReports || canSeeAlerts;

  return (
    <div className="space-y-6">
      {/* Stats Grid */}
      <div className={cn(
        "grid grid-cols-1 md:grid-cols-2 gap-6",
        canSeeAlerts ? "lg:grid-cols-4" : "lg:grid-cols-3"
      )}>
        <StatCard
          title="Total Products"
          value={stats.totalProducts?.toLocaleString() || '0'}
          icon={Package}
          variant="primary"
          // change={{ value: 12, positive: true }} // TODO: Add change calculation if API supports history
          delay={0}
        />
        <StatCard
          title="Total Stock Value"
          value={`$${(stats.totalStockValue / 1000).toFixed(0)}K`}
          icon={DollarSign}
          variant="success"
          // change={{ value: 8, positive: true }}
          delay={50}
        />
        <StatCard
          title="Total Sales"
          value={`$${(stats.totalSales / 1000).toFixed(0)}K`}
          icon={TrendingUp}
          variant="primary"
          // change={{ value: 23, positive: true }}
          delay={100}
        />
        {canAccessPage('alerts') && (
          <StatCard
            title="Low Stock Items"
            value={stats.lowStockCount || 0}
            icon={AlertTriangle}
            variant="warning"
            delay={150}
          />
        )}
      </div>

      {/* Charts Row */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className={canSeeCategories ? "lg:col-span-2" : "lg:col-span-3"}>
          <SalesChart data={data.salesChartData} />
        </div>
        {canAccessPage('categories') && (
          <CategoryChart data={data.categoryDistribution} />
        )}
      </div>

      {/* Bottom Row */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className={hasRightPanel ? "lg:col-span-2" : "lg:col-span-3"}>
          <RecentTransactions
            data={stats.recentTransactions}
            onNavigate={(path) => navigate(`/${path}`)}
            onItemClick={(type, id) => navigate(type === 'product' ? `/products/${id}` : `/transactions/${id}`)}
          />
        </div>
        <div className="space-y-6">
          {canAccessPage('reports') && (
            <TopProducts
              data={data.topProducts}
              onNavigate={() => navigate('/products')}
              onItemClick={(id) => navigate(`/products/${id}`)}
            />
          )}
          {canAccessPage('alerts') && (
            <LowStockPanel
              data={data.lowStockInventories}
              onNavigate={() => navigate('/alerts')}
              onItemClick={(id) => navigate(`/products/${id}`)}
            />
          )}
        </div>
      </div>
    </div>
  );
}
