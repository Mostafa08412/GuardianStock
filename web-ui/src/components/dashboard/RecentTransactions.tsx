import { ArrowUpRight, ArrowDownLeft, ExternalLink } from 'lucide-react';
import { DashboardTransaction } from '@/types';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';

interface RecentTransactionsProps {
  data: DashboardTransaction[];
  onNavigate?: (page: string) => void;
  onItemClick?: (type: 'transaction' | 'product', id: string) => void;
}

export default function RecentTransactions({ data, onNavigate, onItemClick }: RecentTransactionsProps) {
  return (
    <div className="bg-card border border-border rounded-xl animate-fade-in" style={{ animationDelay: '400ms' }}>
      <div className="p-6 border-b border-border">
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-semibold text-foreground">Recent Transactions</h3>
            <p className="text-sm text-muted-foreground">Latest inventory movements</p>
          </div>
          <button
            onClick={() => onNavigate?.('transactions')}
            className="flex items-center gap-1 text-sm text-primary hover:underline"
          >
            View all
            <ExternalLink className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      <div className="divide-y divide-border">
        {data.map((transaction) => (
          <div
            key={transaction.id}
            className="px-6 py-4 table-row-hover cursor-pointer group"
            onClick={() => onItemClick?.('transaction', transaction.id)}
          >
            <div className="flex items-center gap-4">
              <div className={cn(
                'w-10 h-10 rounded-lg flex items-center justify-center',
                transaction.type.toLowerCase() === 'sale'
                  ? 'bg-success/20 text-success'
                  : 'bg-info/20 text-info'
              )}>
                {transaction.type.toLowerCase() === 'sale'
                  ? <ArrowUpRight className="w-5 h-5" />
                  : <ArrowDownLeft className="w-5 h-5" />
                }
              </div>

              <div className="flex-1 min-w-0">
                <p
                  className="text-sm font-medium text-foreground truncate hover:text-primary transition-colors"
                  onClick={(e) => {
                    e.stopPropagation();
                    onItemClick?.('product', transaction.productId);
                  }}
                >
                  {transaction.productName}
                </p>
                <p className="text-xs text-muted-foreground">
                  {transaction.userName} • {format(new Date(transaction.date), 'MMM d, yyyy')}
                </p>
              </div>

              <div className="text-right">
                <p className={cn(
                  'text-sm font-semibold',
                  transaction.type.toLowerCase() === 'sale' ? 'text-success' : 'text-info'
                )}>
                  {transaction.type.toLowerCase() === 'sale' ? '+' : '-'}${transaction.totalAmount?.toLocaleString() || '0'}
                </p>
                <p className="text-xs text-muted-foreground">
                  {transaction.quantity} units
                </p>
              </div>
            </div>
          </div>
        ))}
        {data.length === 0 && (
          <div className="px-6 py-8 text-center">
            <p className="text-sm text-muted-foreground">No recent transactions</p>
          </div>
        )}
      </div>
    </div>
  );
}
