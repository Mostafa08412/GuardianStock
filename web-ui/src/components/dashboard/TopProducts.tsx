import { TrendingUp, ExternalLink, Package } from 'lucide-react';
import { TopProduct } from '@/types';

interface TopProductsProps {
  data: TopProduct[];
  onNavigate?: (page: string) => void;
  onItemClick?: (id: string) => void;
}

export default function TopProducts({ data, onNavigate, onItemClick }: TopProductsProps) {
  const maxSales = data.length > 0 ? Math.max(...data.map(p => p.sales)) : 1;

  return (
    <div className="bg-card border border-border rounded-xl animate-fade-in" style={{ animationDelay: '350ms' }}>
      <div className="p-6 border-b border-border">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-primary/20 flex items-center justify-center">
              <TrendingUp className="w-5 h-5 text-primary" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-foreground">Top Products</h3>
              <p className="text-sm text-muted-foreground">Best selling items this month</p>
            </div>
          </div>
          <button
            onClick={() => onNavigate?.('products')}
            className="flex items-center gap-1 text-sm text-primary hover:underline"
          >
            View all
            <ExternalLink className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      <div className="p-6 space-y-4">
        {data.map((product, index) => {
          const percentage = (product.sales / maxSales) * 100;

          return (
            <div
              key={product.id || index}
              className="group cursor-pointer p-2 -mx-2 rounded-lg hover:bg-secondary/50 transition-colors"
              onClick={() => { console.log(product.id); onItemClick?.(product.id) }}
            >
              <div className="flex items-center justify-between mb-2">
                <div className="flex items-center gap-3">
                  <span className="w-6 h-6 rounded-full bg-secondary flex items-center justify-center text-xs font-medium text-muted-foreground group-hover:bg-primary group-hover:text-primary-foreground transition-colors">
                    {index + 1}
                  </span>
                  <div className="w-8 h-8 rounded-lg bg-secondary flex items-center justify-center overflow-hidden flex-shrink-0">
                    {product.imageUrl ? (
                      <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
                    ) : (
                      <Package className="w-4 h-4 text-muted-foreground" />
                    )}
                  </div>
                  <span className="text-sm font-medium text-foreground truncate group-hover:text-primary transition-colors">{product.name}</span>
                </div>
                <div className="text-right">
                  <p className="text-sm font-semibold text-foreground">${product.revenue.toLocaleString()}</p>
                  <p className="text-xs text-muted-foreground">{product.sales} sales</p>
                </div>
              </div>

              <div className="h-1.5 bg-secondary rounded-full overflow-hidden">
                <div
                  className="h-full bg-primary rounded-full transition-all duration-700"
                  style={{ width: `${percentage}%` }}
                />
              </div>
            </div>
          );
        })}
        {data.length === 0 && (
          <div className="text-center text-sm text-muted-foreground">No top products data</div>
        )}
      </div>
    </div>
  );
}
