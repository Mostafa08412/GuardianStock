import { useState, useEffect, useCallback, useRef } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import {
    Search,
    X,
    RefreshCw,
    ChevronLeft,
    ChevronRight,
    Box,
    DollarSign,
    Layers,
    AlertCircle
} from 'lucide-react';
import { cn } from '@/lib/utils';
import { toast } from 'sonner';
import { InventoryListItemDto } from '@/types/inventory';
import { inventoryService } from '@/api/services/inventory.service';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';

interface InventoriesTableProps {
    onRefreshSummary?: () => void;
}

export function InventoriesTable({ onRefreshSummary }: InventoriesTableProps) {
    const [searchParams, setSearchParams] = useSearchParams();
    const navigate = useNavigate();
    const isInitialMount = useRef(true);

    // State
    const [inventories, setInventories] = useState<InventoryListItemDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(0);

    // Filters & Pagination State
    const [searchQuery, setSearchQuery] = useState(searchParams.get('search') || '');
    const [stockStatus, setStockStatus] = useState<string>(searchParams.get('status') || 'all');
    const [sortBy, setSortBy] = useState<string>(searchParams.get('sortBy') || 'ProductName-asc');
    const [page, setPage] = useState(parseInt(searchParams.get('page') || '1'));
    const [pageSize, setPageSize] = useState(parseInt(searchParams.get('pageSize') || '10'));

    // Update URL params
    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
            return;
        }

        const params: Record<string, string> = {};
        if (searchQuery) params.search = searchQuery;
        if (stockStatus !== 'all') params.status = stockStatus;
        if (sortBy !== 'ProductName-asc') params.sortBy = sortBy;
        if (page !== 1) params.page = page.toString();
        if (pageSize !== 10) params.pageSize = pageSize.toString();

        setSearchParams(params, { replace: true });
    }, [searchQuery, stockStatus, sortBy, page, pageSize, setSearchParams]);

    // Fetch Data
    const fetchInventories = useCallback(async () => {
        try {
            setIsLoading(true);
            const [sortField, sortOrder] = sortBy.split('-');

            const response = await inventoryService.getList({
                page,
                pageSize,
                search: searchQuery || undefined,
                stockStatus: stockStatus !== 'all' ? stockStatus : undefined,
                sortBy: sortField,
                sortOrder: sortOrder.toLowerCase() as 'asc' | 'desc',
            });

            setInventories(response.items);
            setTotalCount(response.totalCount);
            setTotalPages(response.totalPages);
        } catch (error) {
            console.error('Failed to fetch inventories:', error);
            toast.error('Failed to load inventories');
        } finally {
            setIsLoading(false);
        }
    }, [page, pageSize, searchQuery, stockStatus, sortBy]);

    useEffect(() => {
        fetchInventories();
    }, [fetchInventories]);

    const handleRowClick = (id: string) => {
        navigate(`/inventories/${id}`);
    };

    const clearAllFilters = () => {
        setSearchQuery('');
        setStockStatus('all');
        setSortBy('ProductName-asc');
        setPage(1);
    };

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
        <div className="bg-card border border-border rounded-xl">
            {/* Header */}
            <div className="p-6 border-b border-border">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h3 className="text-lg font-semibold text-foreground">Inventories</h3>
                        <p className="text-sm text-muted-foreground">{totalCount} items in stock</p>
                    </div>

                    <div className="flex items-center gap-2">
                        <Button
                            onClick={() => { fetchInventories(); onRefreshSummary?.(); }}
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
                <div className="flex flex-wrap gap-3 mt-4">
                    <div className="relative flex-1 min-w-[200px]">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                        <Input
                            placeholder="Search by product name or SKU..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="pl-10 bg-secondary/50"
                        />
                    </div>

                    <Select value={stockStatus} onValueChange={setStockStatus}>
                        <SelectTrigger className="w-[150px] bg-secondary/50">
                            <SelectValue placeholder="Stock Status" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="all">All Statuses</SelectItem>
                            <SelectItem value="Healthy">Healthy</SelectItem>
                            <SelectItem value="Low">Low Stock</SelectItem>
                            <SelectItem value="Critical">Critical</SelectItem>
                        </SelectContent>
                    </Select>

                    <Select value={sortBy} onValueChange={setSortBy}>
                        <SelectTrigger className="w-[180px] bg-secondary/50">
                            <SelectValue placeholder="Sort By" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value="ProductName-asc">Name (A-Z)</SelectItem>
                            <SelectItem value="ProductName-desc">Name (Z-A)</SelectItem>
                            <SelectItem value="Stock-asc">Stock (Low-High)</SelectItem>
                            <SelectItem value="Stock-desc">Stock (High-Low)</SelectItem>
                            <SelectItem value="ProductPrice-desc">Price (High-Low)</SelectItem>
                            <SelectItem value="ProductPrice-asc">Price (Low-High)</SelectItem>
                        </SelectContent>
                    </Select>

                    {(searchQuery || stockStatus !== 'all' || sortBy !== 'ProductName-asc') && (
                        <Button variant="ghost" size="sm" onClick={clearAllFilters} className="text-muted-foreground">
                            <X className="w-4 h-4 mr-1" />
                            Clear
                        </Button>
                    )}
                </div>
            </div>

            {isLoading ? (
                <div className="p-12 text-center">
                    <RefreshCw className="w-8 h-8 animate-spin text-primary mx-auto mb-4" />
                    <p className="text-muted-foreground">Loading inventories...</p>
                </div>
            ) : inventories.length === 0 ? (
                <div className="p-12 text-center">
                    <Box className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
                    <h4 className="text-lg font-semibold text-foreground mb-2">No inventory items found</h4>
                    <p className="text-muted-foreground">Try adjusting your filters.</p>
                </div>
            ) : (
                <>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b border-border bg-secondary/20">
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Product</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">SKU</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Price</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Stock Status</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Quantity</th>
                                    <th className="px-6 py-4 text-right text-xs font-semibold text-muted-foreground uppercase tracking-wider">Actions</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-border">
                                {inventories.map((item) => (
                                    <tr
                                        key={item.id}
                                        onClick={() => handleRowClick(item.id)}
                                        className="hover:bg-secondary/50 transition-colors cursor-pointer group"
                                    >
                                        <td className="px-6 py-4">
                                            <div className="flex flex-col">
                                                <span className="text-sm font-medium text-foreground group-hover:text-primary transition-colors">
                                                    {item.productName}
                                                </span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className="text-sm font-mono text-muted-foreground">{item.productSku}</span>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className="text-sm text-foreground">${item.productPrice.toLocaleString()}</span>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className={cn(
                                                'inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium',
                                                item.stockStatus === 'Healthy' ? 'bg-success/20 text-success' :
                                                    item.stockStatus === 'Low' ? 'bg-warning/20 text-warning' :
                                                        'bg-destructive/20 text-destructive'
                                            )}>
                                                {item.stockStatus === 'Healthy' ? <Layers className="w-3 h-3" /> : <AlertCircle className="w-3 h-3" />}
                                                {item.stockStatus}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex flex-col">
                                                <span className="text-sm font-semibold text-foreground">{item.stock} units</span>
                                                <span className="text-xs text-muted-foreground">Threshold: {item.lowStockThreshold}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 text-right">
                                            <Button variant="ghost" size="sm" onClick={(e) => {
                                                e.stopPropagation();
                                                handleRowClick(item.id);
                                            }}>
                                                View Details
                                            </Button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
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
                                    <ChevronLeft className="w-4 h-4 mr-1" />
                                    Prev
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
                                    Next
                                    <ChevronRight className="w-4 h-4 ml-1" />
                                </Button>
                            </div>
                        </div>
                    )}
                </>
            )}
        </div>
    );
}
