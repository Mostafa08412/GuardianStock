import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
    ArrowLeft,
    AlertTriangle,
    AlertCircle,
    Package,
    ShoppingCart,
    Clock,
    Bell,
    CheckCircle,
    TrendingDown,
    Loader2,
    DollarSign,
    Layers,
    Settings2,
    TrendingUp,
    History
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Progress } from '@/components/ui/progress';
import { inventoryService } from '@/api/services/inventory.service';
import { alertsService } from '@/api/services/alerts.service';
import { InventoryDetailsDto, TransactionType } from '@/types/inventory';
import { format } from 'date-fns';
import NewTransactionModal from '@/components/transactions/NewTransactionModal';
import AdjustThresholdModal from '@/components/inventory/AdjustThresholdModal';
import { PermissionGate } from '@/components/auth/PermissionGate';
import { cn } from '@/lib/utils';
import { toast } from 'sonner';

export default function InventoryDetails() {
    const { id: inventoryId } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const onBack = () => navigate(-1);

    const [inventory, setInventory] = useState<InventoryDetailsDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isTransactionModalOpen, setIsTransactionModalOpen] = useState(false);
    const [isAdjustModalOpen, setIsAdjustModalOpen] = useState(false);

    const fetchData = useCallback(async () => {
        if (!inventoryId) return;
        try {
            setIsLoading(true);
            const data = await inventoryService.getById(inventoryId);
            setInventory(data);
        } catch (error) {
            console.error('Failed to load inventory details:', error);
            toast.error('Failed to load inventory details');
        } finally {
            setIsLoading(false);
        }
    }, [inventoryId]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    const handleOrderStock = () => {
        setIsTransactionModalOpen(true);
    };

    const handleDismissAlert = async () => {
        if (!inventoryId) return;
        try {
            await alertsService.dismiss(inventoryId);
            toast.success(`Alert dismissed for ${inventory?.productName || 'Product'}.`);
            fetchData();
        } catch (error) {
            console.error('Failed to dismiss alert:', error);
            toast.error('Failed to dismiss alert');
        }
    };

    const handleAdjustThreshold = () => {
        setIsAdjustModalOpen(true);
    };

    if (isLoading) {
        return (
            <div className="flex h-[400px] items-center justify-center">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
        );
    }

    if (!inventory) {
        return (
            <div className="flex flex-col items-center justify-center h-[400px] space-y-4">
                <AlertTriangle className="h-12 w-12 text-muted-foreground" />
                <p className="text-lg font-medium text-foreground">Inventory item not found</p>
                <Button onClick={onBack}>Go Back</Button>
            </div>
        );
    }

    const stockPercentage = inventory.lowStockThreshold > 0
        ? (inventory.stock / inventory.lowStockThreshold) * 100
        : 100;

    const isAlertActive = inventory.stock <= inventory.lowStockThreshold;

    return (
        <div className="p-6 space-y-6 animate-fade-in">
            {/* Header */}
            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-border pb-6">
                <div className="flex items-center gap-4">
                    <Button variant="ghost" size="icon" onClick={onBack} className="rounded-full">
                        <ArrowLeft className="w-5 h-5" />
                    </Button>
                    <div>
                        <div className="flex items-center gap-2">
                            <h1 className="text-2xl font-bold text-foreground">{inventory.productName}</h1>
                            <div className={cn(
                                'inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium',
                                inventory.stockStatus === 'Healthy' ? 'bg-success/20 text-success' :
                                    inventory.stockStatus === 'Low' ? 'bg-warning/20 text-warning' :
                                        'bg-destructive/20 text-destructive'
                            )}>
                                {inventory.stockStatus === 'Healthy' ? <CheckCircle className="w-3.5 h-3.5" /> : <AlertTriangle className="w-3.5 h-3.5" />}
                                {inventory.stockStatus}
                            </div>
                        </div>
                        <p className="text-muted-foreground font-mono text-sm">{inventory.productSku}</p>
                    </div>
                </div>
                <div className="flex flex-wrap items-center gap-2">
                    {isAlertActive && !inventory.isDismissed && (
                        <PermissionGate action="alerts.acknowledge">
                            <Button variant="outline" onClick={handleDismissAlert} className="gap-2 border-warning text-warning hover:bg-warning/10">
                                <Bell className="w-4 h-4" />
                                Dismiss Alert
                            </Button>
                        </PermissionGate>
                    )}
                    <PermissionGate action="inventories.adjustThreshold">
                        <Button variant="outline" onClick={handleAdjustThreshold} className="gap-2">
                            <Settings2 className="w-4 h-4" />
                            Adjust Threshold
                        </Button>
                    </PermissionGate>
                    <Button onClick={handleOrderStock} className="gap-2">
                        <ShoppingCart className="w-4 h-4" />
                        Restock Item
                    </Button>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Main Info */}
                <div className="lg:col-span-2 space-y-6">
                    {/* Stock Status Card */}
                    <div className="bg-card border border-border rounded-xl p-6 shadow-sm overflow-hidden relative">
                        <div className="flex items-start gap-4 mb-6">
                            <div className={cn(
                                'w-14 h-14 rounded-2xl flex items-center justify-center shadow-inner',
                                inventory.stockStatus === 'Healthy' ? 'bg-success/10 text-success' :
                                    inventory.stockStatus === 'Low' ? 'bg-warning/10 text-warning' :
                                        'bg-destructive/10 text-destructive'
                            )}>
                                <Layers className="w-7 h-7" />
                            </div>
                            <div className="flex-1">
                                <h3 className="text-lg font-semibold text-foreground mb-1">Stock Availability</h3>
                                <p className="text-sm text-muted-foreground">
                                    {inventory.stockStatus === 'Healthy'
                                        ? 'Inventory levels are currently within safe operating range.'
                                        : `Current stock(${inventory.stock} units) is ${inventory.stockStatus.toLowerCase()} level.`}
                                </p>
                            </div>
                        </div>

                        <div className="space-y-4">
                            <div className="flex justify-between text-sm font-medium">
                                <span className="text-muted-foreground">Threshold Progress</span>
                                <span className={cn(
                                    inventory.stockStatus === 'Healthy' ? 'text-success' :
                                        inventory.stockStatus === 'Low' ? 'text-warning' : 'text-destructive'
                                )}>
                                    {inventory.stock} / {inventory.lowStockThreshold} units
                                </span>
                            </div>
                            <Progress
                                value={Math.min(stockPercentage, 100)}
                                className={cn('h-3',
                                    inventory.stockStatus === 'Healthy' ? '[&>div]:bg-success' :
                                        inventory.stockStatus === 'Low' ? '[&>div]:bg-warning' : '[&>div]:bg-destructive'
                                )}
                            />
                            <div className="flex justify-between text-xs text-muted-foreground px-1">
                                <span>0 units</span>
                                <span className="font-medium">Limit: {inventory.lowStockThreshold} units</span>
                            </div>
                        </div>

                        {inventory.shortageQuantity > 0 && (
                            <div className="mt-6 p-4 bg-destructive/5 rounded-lg border border-destructive/20 flex items-center gap-3">
                                <AlertCircle className="w-5 h-5 text-destructive" />
                                <p className="text-sm text-destructive font-medium">
                                    Currently <span className="underline decoration-2">{inventory.shortageQuantity} units</span> short of the safety threshold.
                                </p>
                            </div>
                        )}
                    </div>

                    {/* Product and Supplier Details */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="bg-card border border-border rounded-xl p-6 shadow-sm">
                            <div className="flex items-center gap-2 mb-4">
                                <Package className="w-5 h-5 text-primary" />
                                <h3 className="font-semibold text-foreground">Product Info</h3>
                            </div>
                            <div className="space-y-4">
                                <div className="flex justify-between py-2 border-b border-border/50">
                                    <span className="text-sm text-muted-foreground">Unit Price</span>
                                    <span className="text-sm font-semibold text-foreground">${inventory.productPrice.toLocaleString()}</span>
                                </div>
                                <div className="flex justify-between py-2 border-b border-border/50">
                                    <span className="text-sm text-muted-foreground">Stock Value</span>
                                    <span className="text-sm font-semibold text-foreground">${inventory.stockValue.toLocaleString()}</span>
                                </div>
                                <div className="flex justify-between py-2 border-b border-border/50">
                                    <span className="text-sm text-muted-foreground">SKU Number</span>
                                    <span className="text-sm font-mono text-foreground">{inventory.productSku}</span>
                                </div>
                            </div>
                        </div>

                        <div className="bg-card border border-border rounded-xl p-6 shadow-sm">
                            <div className="flex items-center gap-2 mb-4">
                                <DollarSign className="w-5 h-5 text-primary" />
                                <h3 className="font-semibold text-foreground">Supply Chain</h3>
                            </div>
                            <div className="space-y-4">
                                <div className="flex justify-between py-2 border-b border-border/50">
                                    <span className="text-sm text-muted-foreground">Main Supplier</span>
                                    <span className="text-sm font-semibold text-foreground">{inventory.supplier}</span>
                                </div>
                                <div className="flex justify-between py-2 border-b border-border/50">
                                    <span className="text-sm text-muted-foreground">Last Updated</span>
                                    <span className="text-sm text-foreground">{format(new Date(inventory.lastUpdatedAt), 'MMM d, h:mm a')}</span>
                                </div>
                                <div className="flex justify-between py-2 border-b border-border/50">
                                    <span className="text-sm text-muted-foreground">Created On</span>
                                    <span className="text-sm text-foreground">{format(new Date(inventory.createdAt), 'MMM d, yyyy')}</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Recent Transactions */}
                    <div className="bg-card border border-border rounded-xl shadow-sm overflow-hidden">
                        <div className="p-6 border-b border-border bg-secondary/10 flex items-center justify-between">
                            <div>
                                <h3 className="text-lg font-semibold text-foreground">Transaction History</h3>
                                <p className="text-sm text-muted-foreground italic">Last {inventory.recentTransactions.length} activities</p>
                            </div>
                            <History className="w-5 h-5 text-muted-foreground" />
                        </div>
                        {inventory.recentTransactions.length === 0 ? (
                            <div className="p-12 text-center">
                                <Clock className="w-10 h-10 text-muted-foreground/30 mx-auto mb-3" />
                                <p className="text-muted-foreground">No recent transaction data available.</p>
                            </div>
                        ) : (
                            <div className="divide-y divide-border">
                                {inventory.recentTransactions.map((transaction) => (
                                    <div key={transaction.id} className="p-4 flex items-center justify-between hover:bg-secondary/20 transition-colors">
                                        <div className="flex items-center gap-4">
                                            <div className={cn(
                                                'w-10 h-10 rounded-full flex items-center justify-center border-2',
                                                transaction.type === TransactionType.Sale ? 'bg-success/5 border-success/20 text-success' : 'bg-info/5 border-info/20 text-info'
                                            )}>
                                                {transaction.type === TransactionType.Sale ? <TrendingDown className="w-5 h-5" /> : <TrendingUp className="w-5 h-5" />}
                                            </div>
                                            <div>
                                                <p className="font-semibold text-foreground capitalize text-sm">
                                                    {TransactionType[transaction.type]}
                                                </p>
                                                <p className="text-xs text-muted-foreground">{format(new Date(transaction.date), 'MMM d, yyyy h:mm a')}</p>
                                            </div>
                                        </div>
                                        <div className="text-right">
                                            <p className={cn(
                                                'font-bold text-sm',
                                                transaction.type === TransactionType.Sale ? 'text-destructive' : 'text-success'
                                            )}>
                                                {transaction.type === TransactionType.Sale ? '-' : '+'}{transaction.quantity} units
                                            </p>
                                            <p className="text-xs text-muted-foreground font-medium">${transaction.totalAmount.toLocaleString()}</p>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                        {inventory.recentTransactions.length > 0 && (
                            <div className="p-4 bg-secondary/5 text-center border-t border-border">
                                <Button variant="ghost" size="sm" onClick={() => navigate('/transactions', { state: { search: inventory.productSku } })}>
                                    View All Transactions
                                </Button>
                            </div>
                        )}
                    </div>
                </div>

                {/* Sidebar */}
                <div className="space-y-6">
                    {/* Quick Status Summary */}
                    <div className="bg-card border border-border rounded-xl p-6 shadow-sm">
                        <h3 className="font-semibold text-foreground mb-4 border-b border-border pb-2">Inventory Metrics</h3>
                        <div className="space-y-5">
                            <div className="flex items-center justify-between">
                                <div className="flex flex-col">
                                    <span className="text-xs text-muted-foreground uppercase font-bold tracking-tight">On Hand</span>
                                    <span className="text-2xl font-bold text-foreground">{inventory.stock}</span>
                                </div>
                                <div className="flex flex-col text-right">
                                    <span className="text-xs text-muted-foreground uppercase font-bold tracking-tight">Threshold</span>
                                    <span className="text-lg font-semibold text-muted-foreground">{inventory.lowStockThreshold}</span>
                                </div>
                            </div>

                            <div className="p-4 bg-secondary/30 rounded-lg space-y-3">
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">Market Value</span>
                                    <span className="font-bold text-foreground">${inventory.stockValue.toLocaleString()}</span>
                                </div>
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">Alert Triggered</span>
                                    <span className="font-medium text-foreground">
                                        {inventory.alertTriggeredAt ? format(new Date(inventory.alertTriggeredAt), 'MMM d, yyyy') : 'Never'}
                                    </span>
                                </div>
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">Dismissed</span>
                                    <span className="font-medium text-foreground">
                                        {inventory.isDismissed ? 'Yes' : 'No'}
                                    </span>
                                </div>
                            </div>

                            {inventory.stock <= inventory.lowStockThreshold && (
                                <div className="space-y-3">
                                    <p className="text-xs font-semibold text-muted-foreground uppercase tracking-widest px-1">Recommendations</p>
                                    <div className="p-3 bg-warning/10 border border-warning/20 rounded-lg">
                                        <p className="text-xs text-warning-foreground leading-relaxed">
                                            Stock is below safety levels. Recommend ordering at least
                                            <span className="font-bold mx-1">{(inventory.lowStockThreshold * 2) - inventory.stock} units</span>
                                            to maintain optimal inventory buffer.
                                        </p>
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Notification Info */}
                    <div className="bg-card border border-border rounded-xl p-6 shadow-sm">
                        <div className="flex items-center gap-2 mb-4">
                            <Bell className="w-5 h-5 text-primary" />
                            <h3 className="font-semibold text-foreground">Alert History</h3>
                        </div>
                        <div className="space-y-4">
                            <div className="flex gap-3">
                                <div className={cn(
                                    "w-1 h-auto rounded-full",
                                    inventory.isNotificationSent ? "bg-success" : "bg-muted"
                                )} />
                                <div>
                                    <p className="text-sm font-medium">Notification Sent</p>
                                    <p className="text-xs text-muted-foreground">
                                        {inventory.isNotificationSent ? 'System successfully dispatched alert notify.' : 'Notification queue pending.'}
                                    </p>
                                </div>
                            </div>
                            {inventory.isDismissed && (
                                <div className="flex gap-3">
                                    <div className="w-1 h-auto rounded-full bg-info" />
                                    <div>
                                        <p className="text-sm font-medium">Alert Dismissed</p>
                                        <p className="text-xs text-muted-foreground">
                                            User acknowledged this alert on {inventory.dismissedAt ? format(new Date(inventory.dismissedAt), 'MMM d') : 'unknown date'}.
                                        </p>
                                    </div>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>

            <NewTransactionModal
                isOpen={isTransactionModalOpen}
                onClose={() => setIsTransactionModalOpen(false)}
                preselectedProduct={{
                    id: inventory.productId,
                    name: inventory.productName,
                    sku: inventory.productSku,
                    price: inventory.productPrice,
                    stockQuantity: inventory.stock,
                    lowStockThreshold: inventory.lowStockThreshold,
                    supplier: inventory.supplier,
                    categoryId: '', // Not in DTO
                    description: '' // Not in DTO
                }}
                preselectedType="purchase"
                onSubmit={() => {
                    fetchData();
                }}
            />
            <AdjustThresholdModal
                isOpen={isAdjustModalOpen}
                onClose={() => setIsAdjustModalOpen(false)}
                inventoryId={inventoryId!}
                productName={inventory.productName}
                productSku={inventory.productSku}
                currentThreshold={inventory.lowStockThreshold}
                onSuccess={fetchData}
            />
        </div>
    );
}
