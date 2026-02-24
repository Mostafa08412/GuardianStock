import { useState, useEffect, useCallback, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Search, ArrowUpRight, ArrowDownLeft, Plus, ChevronLeft, ChevronRight, X, CalendarIcon } from 'lucide-react';
import { Transaction } from '@/types/inventory';
import { transactionsService } from '@/api/services/transactions.service';
import { useRole } from '@/contexts/RoleContext';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { toast } from 'sonner';
import { PermissionGate } from '@/components/auth/PermissionGate';
import NewTransactionModal from './NewTransactionModal';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Calendar } from '@/components/ui/calendar';
import { Button } from '@/components/ui/button';

interface TransactionsTableProps {
  onViewDetails?: (transactionId: string) => void;
}

export default function TransactionsTable({ onViewDetails }: TransactionsTableProps) {
  const [searchParams, setSearchParams] = useSearchParams();
  const isInitialMount = useRef(true);

  // Initialize state from URL params
  const [searchQuery, setSearchQuery] = useState(searchParams.get('search') || '');
  const [typeFilter, setTypeFilter] = useState<'all' | 'sale' | 'purchase'>(
    (searchParams.get('type') as any) || 'all'
  );
  const [dateFrom, setDateFrom] = useState<Date | undefined>(
    searchParams.get('from') ? new Date(searchParams.get('from')!) : undefined
  );
  const [dateTo, setDateTo] = useState<Date | undefined>(
    searchParams.get('to') ? new Date(searchParams.get('to')!) : undefined
  );
  const [sortBy, setSortBy] = useState<'date' | 'amount' | 'quantity'>(
    (searchParams.get('sortBy') as any) || 'date'
  );
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>(
    (searchParams.get('sortOrder') as any) || 'desc'
  );

  const [currentPage, setCurrentPage] = useState(parseInt(searchParams.get('page') || '1'));
  const [itemsPerPage, setItemsPerPage] = useState(parseInt(searchParams.get('pageSize') || '10'));

  // Update URL params whenever state changes
  useEffect(() => {
    if (isInitialMount.current) {
      isInitialMount.current = false;
      return;
    }

    const params: Record<string, string> = {};
    if (searchQuery) params.search = searchQuery;
    if (typeFilter !== 'all') params.type = typeFilter;
    if (dateFrom) params.from = dateFrom.toISOString();
    if (dateTo) params.to = dateTo.toISOString();
    if (sortBy !== 'date') params.sortBy = sortBy;
    if (sortOrder !== 'desc') params.sortOrder = sortOrder;
    if (currentPage !== 1) params.page = currentPage.toString();
    if (itemsPerPage !== 10) params.pageSize = itemsPerPage.toString();

    setSearchParams(params, { replace: true });
  }, [searchQuery, typeFilter, dateFrom, dateTo, sortBy, sortOrder, currentPage, itemsPerPage, setSearchParams]);

  // Handle browser back/forward buttons
  useEffect(() => {
    const search = searchParams.get('search') || '';
    const type = (searchParams.get('type') as any) || 'all';
    const fromStr = searchParams.get('from');
    const toStr = searchParams.get('to');
    const sort = (searchParams.get('sortBy') as any) || 'date';
    const order = (searchParams.get('sortOrder') as any) || 'desc';
    const page = parseInt(searchParams.get('page') || '1');
    const pageSize = parseInt(searchParams.get('pageSize') || '10');

    setSearchQuery(search);
    setTypeFilter(type);
    setDateFrom(fromStr ? new Date(fromStr) : undefined);
    setDateTo(toStr ? new Date(toStr) : undefined);
    setSortBy(sort);
    setSortOrder(order);
    setCurrentPage(page);
    setItemsPerPage(pageSize);
  }, [searchParams]);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const { hasPermission } = useRole();

  const canRecordTransactions = hasPermission(['admin', 'manager', 'staff']);

  const fetchTransactions = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await transactionsService.getList({
        search: searchQuery || undefined,
        type: typeFilter !== 'all' ? typeFilter : undefined,
        fromDate: dateFrom ? dateFrom.toISOString() : undefined,
        toDate: dateTo ? dateTo.toISOString() : undefined,
        sortBy,
        sortOrder,
        page: currentPage,
        pageSize: itemsPerPage,
      });
      setTransactions(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (error) {
      toast.error('Failed to load transactions');
      console.error('Error fetching transactions:', error);
    } finally {
      setIsLoading(false);
    }
  }, [searchQuery, typeFilter, dateFrom, dateTo, sortBy, sortOrder, currentPage, itemsPerPage]);

  useEffect(() => {
    fetchTransactions();
  }, [fetchTransactions]);

  // Reset to page 1 when filters change
  useEffect(() => {
    if (!isInitialMount.current) {
      setCurrentPage(1);
    }
  }, [searchQuery, typeFilter, dateFrom, dateTo]);

  const handleNewTransaction = async (data: { productId: string; quantity: number; type: 'sale' | 'purchase' }) => {

    toast.success(`${data.type === 'sale' ? 'Sale' : 'Purchase'} recorded successfully!`);
    fetchTransactions(); // Refresh the list

    toast.error(`Failed to record ${data.type}`);


  };

  const clearFilters = () => {
    setSearchQuery('');
    setTypeFilter('all');
    setDateFrom(undefined);
    setDateTo(undefined);
    setSortBy('date');
    setSortOrder('desc');
  };

  const hasActiveFilters = searchQuery || typeFilter !== 'all' || dateFrom || dateTo;

  const getPageNumbers = () => {
    const pages: (number | 'ellipsis')[] = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      if (currentPage <= 3) {
        pages.push(1, 2, 3, 4, 'ellipsis', totalPages);
      } else if (currentPage >= totalPages - 2) {
        pages.push(1, 'ellipsis', totalPages - 3, totalPages - 2, totalPages - 1, totalPages);
      } else {
        pages.push(1, 'ellipsis', currentPage - 1, currentPage, currentPage + 1, 'ellipsis', totalPages);
      }
    }
    return pages;
  };

  return (
    <>
      <div className="bg-card border border-border rounded-xl animate-fade-in">
        {/* Header */}
        <div className="p-6 border-b border-border">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <div>
              <h3 className="text-lg font-semibold text-foreground">Transaction History</h3>
              <p className="text-sm text-muted-foreground">{totalCount} transactions found</p>
            </div>

            <div className="flex items-center gap-2">
              <PermissionGate action="transactions.create">
                <button
                  onClick={() => setIsModalOpen(true)}
                  className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium text-sm"
                >
                  <Plus className="w-4 h-4" />
                  New Transaction
                </button>
              </PermissionGate>
            </div>
          </div>

          {/* Filters */}
          <div className="space-y-3 mt-4">
            {/* Search */}
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
              <input
                type="text"
                placeholder="Search by product or user..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full pl-10 pr-4 py-2 bg-secondary border border-border rounded-lg text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
              />
            </div>

            {/* Filter Row */}
            <div className="flex flex-wrap gap-3">
              {/* Type buttons */}
              <div className="flex items-center gap-1 bg-secondary rounded-lg p-1">
                {(['all', 'sale', 'purchase'] as const).map((type) => (
                  <button
                    key={type}
                    onClick={() => setTypeFilter(type)}
                    className={cn(
                      'px-3 py-1.5 rounded-md text-sm font-medium transition-colors',
                      typeFilter === type
                        ? 'bg-primary text-primary-foreground'
                        : 'text-muted-foreground hover:text-foreground'
                    )}
                  >
                    {type === 'all' ? 'All' : type === 'sale' ? 'Sales' : 'Purchases'}
                  </button>
                ))}
              </div>

              <Popover>
                <PopoverTrigger asChild>
                  <Button variant="outline" className={cn(
                    "w-[140px] justify-start text-left font-normal bg-secondary border-border",
                    !dateFrom && "text-muted-foreground"
                  )}>
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {dateFrom ? format(dateFrom, "MMM d, yyyy") : "From"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={dateFrom}
                    onSelect={setDateFrom}
                    initialFocus
                    className="p-3 pointer-events-auto"
                  />
                </PopoverContent>
              </Popover>

              <Popover>
                <PopoverTrigger asChild>
                  <Button variant="outline" className={cn(
                    "w-[140px] justify-start text-left font-normal bg-secondary border-border",
                    !dateTo && "text-muted-foreground"
                  )}>
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {dateTo ? format(dateTo, "MMM d, yyyy") : "To"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={dateTo}
                    onSelect={setDateTo}
                    initialFocus
                    className="p-3 pointer-events-auto"
                  />
                </PopoverContent>
              </Popover>

              <Select value={`${sortBy}-${sortOrder}`} onValueChange={(v) => {
                const [sort, order] = v.split('-') as [typeof sortBy, typeof sortOrder];
                setSortBy(sort);
                setSortOrder(order);
              }}>
                <SelectTrigger className="w-[160px] bg-secondary border-border">
                  <SelectValue placeholder="Sort By" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="date-desc">Date (Newest)</SelectItem>
                  <SelectItem value="date-asc">Date (Oldest)</SelectItem>
                  <SelectItem value="amount-desc">Amount (High-Low)</SelectItem>
                  <SelectItem value="amount-asc">Amount (Low-High)</SelectItem>
                  <SelectItem value="quantity-desc">Quantity (High-Low)</SelectItem>
                  <SelectItem value="quantity-asc">Quantity (Low-High)</SelectItem>
                </SelectContent>
              </Select>

              {hasActiveFilters && (
                <button
                  onClick={clearFilters}
                  className="flex items-center gap-1 px-3 py-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
                >
                  <X className="w-4 h-4" />
                  Clear Filters
                </button>
              )}
            </div>
          </div>
        </div>

        {/* Loading State */}
        {isLoading && (
          <div className="flex items-center justify-center py-12">
            <div className="w-6 h-6 border-2 border-primary border-t-transparent rounded-full animate-spin" />
          </div>
        )}

        {/* Table - Desktop */}
        {!isLoading && (
          <div className="hidden md:block overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-border">
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Type</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Product</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">SKU</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Quantity</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Amount</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">User</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Date</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {transactions.map((transaction) => (
                  <tr
                    key={transaction.id}
                    onClick={() => onViewDetails?.(transaction.id)}
                    className="hover:bg-secondary/50 transition-colors cursor-pointer"
                  >
                    <td className="px-6 py-4">
                      <div className={cn(
                        'inline-flex items-center gap-2 px-3 py-1.5 rounded-full text-xs font-medium',
                        transaction.type.toLowerCase() === 'sale'
                          ? 'bg-emerald-500/20 text-emerald-500'
                          : 'bg-blue-500/20 text-blue-500'
                      )}>
                        {transaction.type.toLowerCase() === 'sale' ? (
                          <ArrowUpRight className="w-3.5 h-3.5" />
                        ) : (
                          <ArrowDownLeft className="w-3.5 h-3.5" />
                        )}
                        {transaction.type.toLowerCase() === 'sale' ? 'Sale' : 'Purchase'}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm font-medium text-foreground">{transaction.productName}</span>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm text-muted-foreground font-mono">{transaction.productSku}</span>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm text-foreground">{transaction.quantity} units</span>
                    </td>
                    <td className="px-6 py-4">
                      <span className={cn(
                        'text-sm font-semibold',
                        transaction.type.toLowerCase() === 'sale' ? 'text-emerald-500' : 'text-blue-500'
                      )}>
                        {transaction.type.toLowerCase() === 'sale' ? '+' : '-'}${transaction.amount.toLocaleString()}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2">
                        <div className="w-6 h-6 rounded-full bg-primary/20 flex items-center justify-center text-primary text-xs font-medium">
                          {transaction.userName.split(' ').map(n => n[0]).join('')}
                        </div>
                        <span className="text-sm text-muted-foreground">{transaction.userName}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm text-muted-foreground">
                        {format(new Date(transaction.date), 'MMM dd, yyyy')}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Mobile Cards */}
        {!isLoading && (
          <div className="md:hidden divide-y divide-border">
            {transactions.map((transaction) => (
              <div
                key={transaction.id}
                onClick={() => onViewDetails?.(transaction.id)}
                className="p-4 hover:bg-secondary/50 transition-colors cursor-pointer"
              >
                <div className="flex items-start justify-between gap-3 mb-3">
                  <div className="flex items-center gap-3 min-w-0">
                    <div className={cn(
                      'inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium flex-shrink-0',
                      transaction.type.toLowerCase() === 'sale'
                        ? 'bg-emerald-500/20 text-emerald-500'
                        : 'bg-blue-500/20 text-blue-500'
                    )}>
                      {transaction.type.toLowerCase() === 'sale' ? (
                        <ArrowUpRight className="w-3 h-3" />
                      ) : (
                        <ArrowDownLeft className="w-3 h-3" />
                      )}
                      {transaction.type.toLowerCase() === 'sale' ? 'Sale' : 'Purchase'}
                    </div>
                    <span className="text-sm font-medium text-foreground truncate">{transaction.productName}</span>
                  </div>
                  <span className={cn(
                    'text-sm font-semibold flex-shrink-0',
                    transaction.type.toLowerCase() === 'sale' ? 'text-emerald-500' : 'text-blue-500'
                  )}>
                    {transaction.type.toLowerCase() === 'sale' ? '+' : '-'}${transaction.amount.toLocaleString()}
                  </span>
                </div>
                <div className="flex items-center justify-between text-xs text-muted-foreground">
                  <div className="flex items-center gap-2">
                    <div className="w-5 h-5 rounded-full bg-primary/20 flex items-center justify-center text-primary text-[10px] font-medium">
                      {transaction.userName.split(' ').map(n => n[0]).join('')}
                    </div>
                    <span>{transaction.userName}</span>
                  </div>
                  <div className="flex items-center gap-3">
                    <span>{transaction.quantity} units</span>
                    <span>{format(new Date(transaction.date), 'MMM d')}</span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {!isLoading && transactions.length === 0 && (
          <div className="p-12 text-center">
            <p className="text-muted-foreground">No transactions found</p>
          </div>
        )}

        {/* Pagination Footer */}
        <div className="px-6 py-4 border-t border-border flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-4">
            <p className="text-sm text-muted-foreground">
              Showing <span className="font-medium text-foreground">{((currentPage - 1) * itemsPerPage) + 1}</span> to{' '}
              <span className="font-medium text-foreground">{Math.min(currentPage * itemsPerPage, totalCount)}</span> of{' '}
              <span className="font-medium text-foreground">{totalCount}</span> results
            </p>
            <Select value={itemsPerPage.toString()} onValueChange={(v) => { setItemsPerPage(parseInt(v)); setCurrentPage(1); }}>
              <SelectTrigger className="w-[80px] bg-secondary border-border">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="5">5</SelectItem>
                <SelectItem value="10">10</SelectItem>
                <SelectItem value="20">20</SelectItem>
                <SelectItem value="50">50</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="flex items-center gap-1">
            <button
              onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
              disabled={currentPage === 1}
              className={cn(
                'flex items-center gap-1 px-3 py-1.5 text-sm rounded-lg transition-colors',
                currentPage === 1
                  ? 'bg-secondary text-muted-foreground cursor-not-allowed'
                  : 'bg-secondary text-foreground hover:bg-accent'
              )}
            >
              <ChevronLeft className="w-4 h-4" />
              Previous
            </button>

            {getPageNumbers().map((page, i) => (
              page === 'ellipsis' ? (
                <span key={`ellipsis-${i}`} className="px-2 text-muted-foreground">...</span>
              ) : (
                <button
                  key={page}
                  onClick={() => setCurrentPage(page)}
                  className={cn(
                    'px-3 py-1.5 text-sm rounded-lg transition-colors',
                    currentPage === page
                      ? 'bg-primary text-primary-foreground'
                      : 'bg-secondary text-muted-foreground hover:text-foreground hover:bg-accent'
                  )}
                >
                  {page}
                </button>
              )
            ))}

            <button
              onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
              disabled={currentPage === totalPages || totalPages === 0}
              className={cn(
                'flex items-center gap-1 px-3 py-1.5 text-sm rounded-lg transition-colors',
                currentPage === totalPages || totalPages === 0
                  ? 'bg-secondary text-muted-foreground cursor-not-allowed'
                  : 'bg-secondary text-foreground hover:bg-accent'
              )}
            >
              Next
              <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      <NewTransactionModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleNewTransaction}
      />
    </>
  );
}
