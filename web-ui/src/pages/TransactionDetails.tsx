import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowUpRight, ArrowDownLeft, ArrowLeft, Package, User, Calendar, Hash, DollarSign } from 'lucide-react';
import { TransactionDetails as TransactionDetailsType } from '@/types/inventory';
import { transactionsService } from '@/api/services/transactions.service';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';

export default function TransactionDetails() {
  const { id: transactionId } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const onBack = () => navigate(-1);
  const [transaction, setTransaction] = useState<TransactionDetailsType | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchTransaction = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const data = await transactionsService.getById(transactionId);
        setTransaction(data);
      } catch (err) {
        setError('Failed to load transaction details');
        console.error('Error fetching transaction:', err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchTransaction();
  }, [transactionId]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-16">
        <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (error || !transaction) {
    return (
      <div className="flex flex-col items-center justify-center py-16">
        <p className="text-muted-foreground">{error || 'Transaction not found'}</p>
        <button
          onClick={onBack}
          className="mt-4 text-primary hover:underline"
        >
          Go back
        </button>
      </div>
    );
  }

  const isSale = transaction.transactionType.toLowerCase() === 'sale';

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex items-center gap-4">
        <button
          onClick={onBack}
          className="p-2 rounded-lg bg-secondary hover:bg-accent transition-colors"
        >
          <ArrowLeft className="w-5 h-5 text-muted-foreground" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-foreground">Transaction Details</h1>
          <p className="text-muted-foreground">Transaction #{transaction.id}</p>
        </div>
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Transaction Info Card */}
        <div className="lg:col-span-2 bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-4 mb-6">
            <div className={cn(
              'w-14 h-14 rounded-xl flex items-center justify-center',
              isSale
                ? 'bg-success/20 text-success'
                : 'bg-info/20 text-info'
            )}>
              {isSale
                ? <ArrowUpRight className="w-7 h-7" />
                : <ArrowDownLeft className="w-7 h-7" />
              }
            </div>
            <div>
              <div className={cn(
                'inline-flex items-center gap-2 px-3 py-1 rounded-full text-sm font-medium mb-1',
                isSale
                  ? 'bg-success/20 text-success'
                  : 'bg-info/20 text-info'
              )}>
                {isSale ? 'Sale' : 'Purchase'}
              </div>
              <h2 className="text-xl font-semibold text-foreground">{transaction.productName}</h2>
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
            <div className="space-y-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-lg bg-secondary flex items-center justify-center">
                  <Hash className="w-5 h-5 text-muted-foreground" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Transaction ID</p>
                  <p className="text-sm font-medium text-foreground">#{transaction.id}</p>
                </div>
              </div>

              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-lg bg-secondary flex items-center justify-center">
                  <Package className="w-5 h-5 text-muted-foreground" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Quantity</p>
                  <p className="text-sm font-medium text-foreground">{transaction.quantity} units</p>
                </div>
              </div>

              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-lg bg-secondary flex items-center justify-center">
                  <DollarSign className="w-5 h-5 text-muted-foreground" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Unit Price</p>
                  <p className="text-sm font-medium text-foreground">${transaction.unitPrice.toLocaleString()}</p>
                </div>
              </div>
            </div>

            <div className="space-y-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-lg bg-secondary flex items-center justify-center">
                  <User className="w-5 h-5 text-muted-foreground" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Processed By</p>
                  <p className="text-sm font-medium text-foreground">{transaction.createdByUser}</p>
                </div>
              </div>

              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-lg bg-secondary flex items-center justify-center">
                  <Calendar className="w-5 h-5 text-muted-foreground" />
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Date & Time</p>
                  <p className="text-sm font-medium text-foreground">
                    {format(new Date(transaction.createdDate), 'MMM dd, yyyy • h:mm a')}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Summary Card */}
        <div className="bg-card border border-border rounded-xl p-6">
          <h3 className="text-lg font-semibold text-foreground mb-4">Summary</h3>

          <div className="space-y-3">
            <div className="flex justify-between items-center py-2 border-b border-border">
              <span className="text-sm text-muted-foreground">Subtotal</span>
              <span className="text-sm text-foreground">${transaction.totalAmount.toLocaleString()}</span>
            </div>
            <div className="flex justify-between items-center py-2 border-b border-border">
              <span className="text-sm text-muted-foreground">Tax</span>
              <span className="text-sm text-foreground">$0.00</span>
            </div>
            <div className="flex justify-between items-center py-2">
              <span className="text-sm font-semibold text-foreground">Total</span>
              <span className={cn(
                'text-lg font-bold',
                isSale ? 'text-success' : 'text-info'
              )}>
                {isSale ? '+' : '-'}${transaction.totalAmount.toLocaleString()}
              </span>
            </div>
          </div>

          <div className="mt-6 pt-4 border-t border-border">
            <p className="text-xs text-muted-foreground mb-2">Product SKU</p>
            <p className="text-sm font-mono text-foreground bg-secondary px-3 py-2 rounded-lg">
              {transaction.productSku}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
