import { useState, useEffect, useCallback, useRef } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import {
    Search,
    X,
    CalendarIcon,
    AlertTriangle,
    Bell,
    CheckCircle,
    RefreshCw,
    ChevronLeft,
    ChevronRight,
    ShoppingCart
} from 'lucide-react';
import { format } from 'date-fns';
import { cn } from '@/lib/utils';
import { toast } from 'sonner';
import { LowStockAlertListItemDto } from '@/types/inventory';
import { alertsService } from '@/api/services/alerts.service';
import { productsService } from '@/api/services/products.service';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Calendar } from '@/components/ui/calendar';
import NewTransactionModal from '@/components/transactions/NewTransactionModal';
import { Product } from '@/types/inventory';
import { PermissionGate } from '@/components/auth/PermissionGate';

interface AlertsTableProps {
    onRefreshSummary?: () => void;
}

export function AlertsTable({ onRefreshSummary }: AlertsTableProps) {
    const [searchParams, setSearchParams] = useSearchParams();
    const navigate = useNavigate();
    const isInitialMount = useRef(true);

    // State
    const [alerts, setAlerts] = useState<LowStockAlertListItemDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(0);

    // Filters & Pagination State (Initialized from URL)
    const [searchQuery, setSearchQuery] = useState(searchParams.get('search') || '');
    const [severityFilter, setSeverityFilter] = useState<string>(searchParams.get('severity') || 'all');

    // Mapping URL params to state values
    const getNotificationFromUrl = (val: string | null) => {
        if (val === 'true') return 'sent';
        if (val === 'false') return 'pending';
        return 'all';
    };

    const getDismissedFromUrl = (val: string | null) => {
        if (val === 'true') return 'true';
        if (val === 'null') return 'all';
        return 'false';
    };

    const [notificationFilter, setNotificationFilter] = useState<string>(getNotificationFromUrl(searchParams.get('isNotificationSent')));
    const [dismissedFilter, setDismissedFilter] = useState<string>(getDismissedFromUrl(searchParams.get('isDismissed')));
    const [dateFrom, setDateFrom] = useState<Date | undefined>(
        searchParams.get('fromDate') ? new Date(searchParams.get('fromDate')!) : undefined
    );
    const [dateTo, setDateTo] = useState<Date | undefined>(
        searchParams.get('toDate') ? new Date(searchParams.get('toDate')!) : undefined
    );
    const [sortBy, setSortBy] = useState<string>(searchParams.get('sortBy') || 'triggeredOn-desc');
    const [page, setPage] = useState(parseInt(searchParams.get('page') || '1'));
    const [pageSize, setPageSize] = useState(parseInt(searchParams.get('pageSize') || '10'));

    // Modal state
    const [isTransactionModalOpen, setIsTransactionModalOpen] = useState(false);
    const [selectedProductForModal, setSelectedProductForModal] = useState<Product | null>(null);

    // Update URL params whenever state changes
    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
            return;
        }

        const params: Record<string, string> = {};
        if (searchQuery) params.search = searchQuery;
        if (severityFilter !== 'all') params.severity = severityFilter;
        if (notificationFilter !== 'all') {
            params.isNotificationSent = notificationFilter === 'sent' ? 'true' : 'false';
        }
        params.isDismissed = dismissedFilter === 'all' ? 'null' : dismissedFilter;
        if (dateFrom) params.fromDate = dateFrom.toISOString();
        if (dateTo) params.toDate = dateTo.toISOString();
        if (sortBy !== 'triggeredOn-desc') params.sortBy = sortBy;
        if (page !== 1) params.page = page.toString();
        if (pageSize !== 10) params.pageSize = pageSize.toString();

        setSearchParams(params, { replace: true });
    }, [searchQuery, severityFilter, notificationFilter, dismissedFilter, dateFrom, dateTo, sortBy, page, pageSize, setSearchParams]);

    // Handle browser back/forward buttons
    useEffect(() => {
        const search = searchParams.get('search') || '';
        const severity = searchParams.get('severity') || 'all';
        const notified = getNotificationFromUrl(searchParams.get('isNotificationSent'));
        const dismissed = getDismissedFromUrl(searchParams.get('isDismissed'));

        const fromStr = searchParams.get('fromDate');
        const toStr = searchParams.get('toDate');
        const sort = searchParams.get('sortBy') || 'triggeredOn-desc';
        const p = parseInt(searchParams.get('page') || '1');
        const ps = parseInt(searchParams.get('pageSize') || '10');

        setSearchQuery(search);
        setSeverityFilter(severity);
        setNotificationFilter(notified);
        setDismissedFilter(dismissed);
        setDateFrom(fromStr ? new Date(fromStr) : undefined);
        setDateTo(toStr ? new Date(toStr) : undefined);
        setSortBy(sort);
        setPage(p);
        setPageSize(ps);
    }, [searchParams]);

    // Fetch Data
    const fetchAlerts = useCallback(async () => {
        try {
            setIsLoading(true);
            const [sortField, sortOrder] = sortBy.split('-');

            const response = await alertsService.getList({
                page,
                pageSize,
                search: searchQuery || undefined,
                severity: severityFilter !== 'all' ? severityFilter : undefined,
                notificationSent: notificationFilter === 'all' ? undefined : notificationFilter === 'sent',
                isDismissed: dismissedFilter === 'all' ? null : dismissedFilter === 'true',
                fromDate: dateFrom?.toISOString(),
                toDate: dateTo?.toISOString(),
                sortBy: sortField,
                sortOrder: sortOrder as 'asc' | 'desc',
            });

            setAlerts(response.items);
            setTotalCount(response.totalCount || response.items.length); // Handle if totalCount not in response.data directly
            setTotalPages(response.totalPages || 1);
        } catch (error) {
            console.error('Failed to fetch alerts:', error);
            toast.error('Failed to load alerts');
        } finally {
            setIsLoading(false);
        }
    }, [page, pageSize, searchQuery, severityFilter, notificationFilter, dismissedFilter, dateFrom, dateTo, sortBy]);

    // Initial Load & Refetch
    useEffect(() => {
        fetchAlerts();
    }, [fetchAlerts]);

    // Reset page when filters change
    useEffect(() => {
        if (!isInitialMount.current) {
            setPage(1);
        }
    }, [searchQuery, severityFilter, notificationFilter, dismissedFilter, dateFrom, dateTo, sortBy]);

    const handleDismissAlert = async (id: string, e?: React.MouseEvent) => {
        if (e) e.stopPropagation();
        try {
            await alertsService.dismiss(id);
            toast.success('Alert dismissed');
            fetchAlerts();
            if (onRefreshSummary) onRefreshSummary();
        } catch (error) {
            console.error('Failed to dismiss alert:', error);
            toast.error('Failed to dismiss alert');
        }
    };

    const handleOrderStock = async (alert: LowStockAlertListItemDto, e: React.MouseEvent) => {
        if (e) e.stopPropagation();
        try {
            const product = await productsService.getById(alert.productId);
            if (product) {
                setSelectedProductForModal(product);
                setIsTransactionModalOpen(true);
            } else {
                toast.error('Could not find product details');
            }
        } catch (error) {
            console.error('Failed to fetch product for order:', error);
            toast.error('Failed to prepare stock order');
        }
    };

    const handleViewDetails = (inventoryId: string) => {
        navigate(`/inventories/${inventoryId}`);
    };

    const clearAllFilters = () => {
        setSearchQuery('');
        setSeverityFilter('all');
        setNotificationFilter('all');
        setDismissedFilter('false');
        setDateFrom(undefined);
        setDateTo(undefined);
        setSortBy('triggeredOn-desc');
        setPage(1);
    };

    const activeFiltersCount = [
        searchQuery,
        severityFilter !== 'all',
        notificationFilter !== 'all',
        dismissedFilter !== 'false',
        dateFrom,
        dateTo,
    ].filter(Boolean).length;

    const getPageNumbers = () => {
        const pages: (number | 'ellipsis')[] = [];
        if (totalPages <= 7) {
            for (let i = 1; i <= totalPages; i++) pages.push(i);
        } else {
            if (page <= 3) {
                pages.push(1, 2, 3, 4, 'ellipsis', totalPages);
            } else if (page >= totalPages - 2) {
                pages.push(1, 'ellipsis', totalPages - 3, totalPages - 2, totalPages - 1, totalPages);
            } else {
                pages.push(1, 'ellipsis', page - 1, page, page + 1, 'ellipsis', totalPages);
            }
        }
        return pages;
    };

    const startIndex = (page - 1) * pageSize;
    const endIndex = Math.min(startIndex + pageSize, totalCount);

    return (
        <div className="bg-card border border-border rounded-xl animate-fade-in">
            {/* Header */}
            <div className="p-6 border-b border-border">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h3 className="text-lg font-semibold text-foreground">Stock Alerts</h3>
                        <p className="text-sm text-muted-foreground">{totalCount} alerts found</p>
                    </div>

                    <div className="flex items-center gap-2">
                        <Button
                            onClick={() => { fetchAlerts(); if (onRefreshSummary) onRefreshSummary(); }}
                            size="sm"
                            className="gap-2"
                            disabled={isLoading}
                        >
                            <RefreshCw className={cn("w-4 h-4", isLoading && "animate-spin")} />
                            Refresh
                        </Button>
                    </div>
                </div>

                {/* Filters */}
                <div className="space-y-3 mt-4">
                    <div className="relative flex-1">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                        <Input
                            placeholder="Search by product name..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="pl-10 bg-secondary/50 border-border"
                        />
                    </div>

                    <div className="flex flex-wrap gap-3">
                        <Select value={severityFilter} onValueChange={setSeverityFilter}>
                            <SelectTrigger className="w-[160px] bg-secondary/50 border-border">
                                <SelectValue placeholder="Severity" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All Severities</SelectItem>
                                <SelectItem value="Critical">Critical</SelectItem>
                                <SelectItem value="Low">Low Stock</SelectItem>
                            </SelectContent>
                        </Select>

                        <Select value={notificationFilter} onValueChange={setNotificationFilter}>
                            <SelectTrigger className="w-[180px] bg-secondary/50 border-border">
                                <SelectValue placeholder="Notification Status" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All Notifications</SelectItem>
                                <SelectItem value="sent">Notification Sent</SelectItem>
                                <SelectItem value="pending">Pending</SelectItem>
                            </SelectContent>
                        </Select>

                        <Select value={dismissedFilter} onValueChange={setDismissedFilter}>
                            <SelectTrigger className="w-[180px] bg-secondary/50 border-border">
                                <SelectValue placeholder="Alert Status" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All Alerts</SelectItem>
                                <SelectItem value="false">Active Only</SelectItem>
                                <SelectItem value="true">Dismissed</SelectItem>
                            </SelectContent>
                        </Select>

                        <Popover>
                            <PopoverTrigger asChild>
                                <Button variant="outline" size="sm" className={cn(
                                    "w-[140px] justify-start text-left font-normal bg-secondary/50 border-border",
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
                                />
                            </PopoverContent>
                        </Popover>

                        <Popover>
                            <PopoverTrigger asChild>
                                <Button variant="outline" size="sm" className={cn(
                                    "w-[140px] justify-start text-left font-normal bg-secondary/50 border-border",
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
                                />
                            </PopoverContent>
                        </Popover>

                        <Select value={sortBy} onValueChange={setSortBy}>
                            <SelectTrigger className="w-[180px] bg-secondary/50 border-border">
                                <SelectValue placeholder="Sort By" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="triggeredOn-desc">Date (Newest)</SelectItem>
                                <SelectItem value="triggeredOn-asc">Date (Oldest)</SelectItem>
                                <SelectItem value="currentStock-asc">Stock (Low-High)</SelectItem>
                                <SelectItem value="currentStock-desc">Stock (High-Low)</SelectItem>
                                <SelectItem value="productName-asc">Name (A-Z)</SelectItem>
                                <SelectItem value="productName-desc">Name (Z-A)</SelectItem>
                            </SelectContent>
                        </Select>

                        {activeFiltersCount > 0 && (
                            <Button variant="ghost" size="sm" onClick={clearAllFilters} className="text-muted-foreground">
                                <X className="w-4 h-4 mr-1" />
                                Clear
                            </Button>
                        )}
                    </div>
                </div>
            </div>

            {isLoading ? (
                <div className="p-12 text-center">
                    <RefreshCw className="w-8 h-8 animate-spin text-primary mx-auto mb-4" />
                    <p className="text-muted-foreground">Loading alerts...</p>
                </div>
            ) : alerts.length === 0 ? (
                <div className="p-12 text-center">
                    <AlertTriangle className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
                    <h4 className="text-lg font-semibold text-foreground mb-2">No alerts found</h4>
                    <p className="text-muted-foreground">Try adjusting your filters.</p>
                </div>
            ) : (
                <>
                    {/* Desktop Table */}
                    <div className="hidden md:block overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b border-border bg-secondary/10">
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Product</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Stock Status</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Levels</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Date</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Notification</th>
                                    <th className="px-6 py-4 text-right text-xs font-semibold text-muted-foreground uppercase tracking-wider">Actions</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-border">
                                {alerts.map((alert) => (
                                    <tr
                                        key={alert.inventoryId}
                                        onClick={() => handleViewDetails(alert.inventoryId)}
                                        className="hover:bg-secondary/50 transition-colors cursor-pointer group"
                                    >
                                        <td className="px-6 py-4">
                                            <span className="text-sm font-medium text-foreground group-hover:text-primary transition-colors">{alert.productName}</span>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className={cn(
                                                'inline-flex items-center gap-2 px-3 py-1.5 rounded-full text-xs font-medium',
                                                alert.status === 'Critical' ? 'bg-red-500/20 text-red-500' : 'bg-warning/20 text-warning'
                                            )}>
                                                <AlertTriangle className="w-3.5 h-3.5" />
                                                {alert.status}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="text-sm">
                                                <span className="font-semibold text-foreground">{alert.currentStock}</span>
                                                <span className="text-muted-foreground"> / {alert.threshold}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className="text-sm text-muted-foreground">
                                                {format(new Date(alert.triggeredOn), 'MMM dd, yyyy')}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className={cn(
                                                'flex items-center gap-2 text-sm',
                                                alert.isNotificationSent ? 'text-success' : 'text-warning'
                                            )}>
                                                {alert.isNotificationSent ? <CheckCircle className="w-4 h-4" /> : <Bell className="w-4 h-4" />}
                                                {alert.isNotificationSent ? 'Sent' : 'Pending'}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 text-right">
                                            <div className="flex items-center justify-end gap-2">
                                                <PermissionGate action="transactions.create">
                                                    <Button
                                                        size="sm"
                                                        onClick={(e) => handleOrderStock(alert, e)}
                                                    >
                                                        <ShoppingCart className="w-3.5 h-3.5 mr-1" />
                                                        Order
                                                    </Button>
                                                </PermissionGate>
                                                {!alert.isDismissed && (
                                                    <PermissionGate action="alerts.acknowledge">
                                                        <Button
                                                            variant="outline"
                                                            size="sm"
                                                            onClick={(e) => handleDismissAlert(alert.inventoryId, e)}
                                                        >
                                                            Dismiss
                                                        </Button>
                                                    </PermissionGate>
                                                )}
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    {/* Mobile View */}
                    <div className="md:hidden divide-y divide-border">
                        {alerts.map((alert) => (
                            <div
                                key={alert.inventoryId}
                                onClick={() => handleViewDetails(alert.inventoryId)}
                                className="p-4 hover:bg-secondary/50 transition-colors cursor-pointer"
                            >
                                <div className="flex items-start justify-between mb-2">
                                    <span className="text-sm font-medium text-foreground">{alert.productName}</span>
                                    <div className={cn(
                                        'inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[10px] font-medium',
                                        alert.status === 'Critical' ? 'bg-destructive/20 text-destructive' : 'bg-warning/20 text-warning'
                                    )}>
                                        {alert.status}
                                    </div>
                                </div>
                                <div className="flex justify-between text-xs text-muted-foreground mb-3">
                                    <span>Stock: {alert.currentStock}/{alert.threshold}</span>
                                    <span>{format(new Date(alert.triggeredOn), 'MMM d')}</span>
                                </div>
                                <div className="flex items-center gap-2">
                                    <PermissionGate action="transactions.create">
                                        <Button
                                            size="sm"
                                            className="flex-1 h-8"
                                            onClick={(e) => handleOrderStock(alert, e)}
                                        >
                                            Order
                                        </Button>
                                    </PermissionGate>
                                    {!alert.isDismissed && (
                                        <PermissionGate action="alerts.acknowledge">
                                            <Button
                                                variant="outline"
                                                size="sm"
                                                className="flex-1 h-8"
                                                onClick={(e) => handleDismissAlert(alert.inventoryId, e)}
                                            >
                                                Dismiss
                                            </Button>
                                        </PermissionGate>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>

                    {/* Pagination */}
                    {totalPages > 1 && (
                        <div className="px-6 py-4 border-t border-border flex flex-col sm:flex-row items-center justify-between gap-4">
                            <p className="text-sm text-muted-foreground">
                                Showing <span className="font-medium text-foreground">{startIndex + 1}</span> to{' '}
                                <span className="font-medium text-foreground">{endIndex}</span> of{' '}
                                <span className="font-medium text-foreground">{totalCount}</span> results
                            </p>

                            <div className="flex items-center gap-1">
                                <Button
                                    variant="outline"
                                    size="sm"
                                    onClick={() => setPage(p => Math.max(1, p - 1))}
                                    disabled={page === 1}
                                >
                                    <ChevronLeft className="w-4 h-4" />
                                </Button>
                                {getPageNumbers().map((p, i) => (
                                    p === 'ellipsis' ? (
                                        <span key={`el-${i}`} className="px-2">...</span>
                                    ) : (
                                        <Button
                                            key={p}
                                            variant={page === p ? "default" : "outline"}
                                            size="sm"
                                            onClick={() => setPage(p)}
                                            className="w-9"
                                        >
                                            {p}
                                        </Button>
                                    )
                                ))}
                                <Button
                                    variant="outline"
                                    size="sm"
                                    onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                                    disabled={page === totalPages}
                                >
                                    <ChevronRight className="w-4 h-4" />
                                </Button>
                            </div>
                        </div>
                    )}
                </>
            )}

            <NewTransactionModal
                isOpen={isTransactionModalOpen}
                onClose={() => {
                    setIsTransactionModalOpen(false);
                    setSelectedProductForModal(null);
                }}
                preselectedProduct={selectedProductForModal}
                preselectedType="purchase"
                onSubmit={() => {
                    fetchAlerts();
                    if (onRefreshSummary) onRefreshSummary();
                }}
            />
        </div>
    );
}
