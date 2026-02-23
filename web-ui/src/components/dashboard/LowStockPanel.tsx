import { AlertTriangle, ExternalLink } from 'lucide-react';
import { LowStockInventory } from '@/types';

interface LowStockPanelProps {
  data: LowStockInventory[];
  onNavigate?: (page: string) => void;
  onItemClick?: (id: string) => void;
}

export default function LowStockPanel({ data, onNavigate, onItemClick }: LowStockPanelProps) {
  return (
    <div className="bg-card border border-border rounded-xl animate-fade-in" style={{ animationDelay: '500ms' }}>
      <div className="p-6 border-b border-border">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-lg bg-warning/20 flex items-center justify-center">
            <AlertTriangle className="w-5 h-5 text-warning" />
          </div>
          <div>
            <h3 className="text-lg font-semibold text-foreground">Low Stock Alerts</h3>
            <p className="text-sm text-muted-foreground">{data.length} items need attention</p>
          </div>
        </div>
      </div>

      <div className="divide-y divide-border">
        {data.map((alert) => {
          const stockPercentage = alert.threshold > 0 ? (alert.stock / alert.threshold) * 100 : 0;

          return (
            <div
              key={alert.inventoryId}
              className="px-6 py-4 table-row-hover cursor-pointer group"
              onClick={() => onItemClick?.(alert.productId)}
            >
              <div className="flex items-center justify-between mb-3">
                <p className="text-sm font-medium text-foreground group-hover:text-primary transition-colors">{alert.productName}</p>
                <span className={`badge-${stockPercentage < 30 ? 'danger' : 'warning'}`}>
                  {stockPercentage < 30 ? 'Critical' : 'Low'}
                </span>
              </div>

              <div className="space-y-2">
                <div className="flex justify-between text-xs">
                  <span className="text-muted-foreground">Current: {alert.stock} units</span>
                  <span className="text-muted-foreground">Threshold: {alert.threshold}</span>
                </div>

                <div className="h-2 bg-secondary rounded-full overflow-hidden">
                  <div
                    className={`h-full rounded-full transition-all duration-500 ${stockPercentage < 30 ? 'bg-destructive' : 'bg-warning'
                      }`}
                    style={{ width: `${Math.min(stockPercentage, 100)}%` }}
                  />
                </div>
              </div>
            </div>
          );
        })}
        {data.length === 0 && (
          <div className="px-6 py-8 text-center">
            <p className="text-sm text-muted-foreground">No low stock items</p>
          </div>
        )}
      </div>

      <div className="p-4 border-t border-border">
        <button
          onClick={() => onNavigate?.('alerts')}
          className="w-full flex items-center justify-center gap-2 py-2 text-sm text-primary hover:underline"
        >
          <span>Manage all alerts</span>
          <ExternalLink className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
}
