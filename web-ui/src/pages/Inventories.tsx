import { useState, useEffect, useCallback, memo } from 'react';
import {
    AlertCircle,
    CheckCircle2,
    AlertTriangle,
    RefreshCw
} from 'lucide-react';
import { cn } from '@/lib/utils';
import { InventoriesTable } from '@/components/inventory/InventoriesTable';
import { inventoryService } from '@/api/services/inventory.service';
import { StockSummaryDto } from '@/types/inventory';
import { toast } from 'sonner';

const Inventories = memo(function Inventories() {
    const [summary, setSummary] = useState<StockSummaryDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const fetchSummary = useCallback(async () => {
        try {
            setIsLoading(true);
            const data = await inventoryService.getSummary();
            setSummary(data);
        } catch (error) {
            console.error('Failed to fetch stock summary:', error);
            toast.error('Failed to load stock summary');
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchSummary();
    }, [fetchSummary]);

    return (
        <div className="p-6 space-y-6 animate-fade-in">
            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-border pb-6">
                <div>
                    <h1 className="text-3xl font-bold text-foreground">Inventories</h1>
                    <p className="text-muted-foreground">Manage and track your product stock levels.</p>
                </div>
                <button
                    onClick={fetchSummary}
                    className="flex items-center gap-2 px-4 py-2 bg-secondary text-foreground rounded-lg hover:bg-accent transition-colors font-medium text-sm"
                    disabled={isLoading}
                >
                    <RefreshCw className={cn("w-4 h-4", isLoading && "animate-spin")} />
                    Refresh Stats
                </button>
            </div>

            {/* Summary Cards */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-card border border-border p-6 rounded-xl hover:shadow-lg transition-all duration-300 group">
                    <div className="flex items-center justify-between mb-4">
                        <div className="p-3 bg-destructive/10 text-destructive rounded-lg group-hover:scale-110 transition-transform">
                            <AlertCircle className="w-6 h-6" />
                        </div>
                        <span className="text-xs font-medium text-destructive px-2 py-1 bg-destructive/10 rounded-full">Emergency</span>
                    </div>
                    <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wider">Critical Stock</h3>
                    <div className="flex items-baseline gap-2 mt-1">
                        <span className="text-3xl font-bold text-foreground">{summary?.totalCriticalStock ?? 0}</span>
                        <span className="text-sm text-muted-foreground">items</span>
                    </div>
                </div>

                <div className="bg-card border border-border p-6 rounded-xl hover:shadow-lg transition-all duration-300 group">
                    <div className="flex items-center justify-between mb-4">
                        <div className="p-3 bg-warning/10 text-warning rounded-lg group-hover:scale-110 transition-transform">
                            <AlertTriangle className="w-6 h-6" />
                        </div>
                        <span className="text-xs font-medium text-warning px-2 py-1 bg-warning/10 rounded-full">Action Required</span>
                    </div>
                    <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wider">Low Stock</h3>
                    <div className="flex items-baseline gap-2 mt-1">
                        <span className="text-3xl font-bold text-foreground">{summary?.totalLowStock ?? 0}</span>
                        <span className="text-sm text-muted-foreground">items</span>
                    </div>
                </div>

                <div className="bg-card border border-border p-6 rounded-xl hover:shadow-lg transition-all duration-300 group">
                    <div className="flex items-center justify-between mb-4">
                        <div className="p-3 bg-primary/10 text-primary rounded-lg group-hover:scale-110 transition-transform">
                            <CheckCircle2 className="w-6 h-6" />
                        </div>
                        <span className="text-xs font-medium text-primary px-2 py-1 bg-primary/10 rounded-full">Healthy</span>
                    </div>
                    <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wider">Normal Stock</h3>
                    <div className="flex items-baseline gap-2 mt-1">
                        <span className="text-3xl font-bold text-foreground">{summary?.totalNormalStock ?? 0}</span>
                        <span className="text-sm text-muted-foreground">items</span>
                    </div>
                </div>
            </div>

            <InventoriesTable onRefreshSummary={fetchSummary} />
        </div>
    );
});

export default Inventories;
