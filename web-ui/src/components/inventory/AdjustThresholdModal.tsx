import { useState, useEffect } from 'react';
import { X, Settings2, AlertCircle } from 'lucide-react';
import { inventoryService } from '@/api/services/inventory.service';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { toast } from 'sonner';

interface AdjustThresholdModalProps {
    isOpen: boolean;
    onClose: () => void;
    inventoryId: string;
    productName: string;
    productSku: string;
    currentThreshold: number;
    onSuccess: () => void;
}

export default function AdjustThresholdModal({
    isOpen,
    onClose,
    inventoryId,
    productName,
    productSku,
    currentThreshold,
    onSuccess
}: AdjustThresholdModalProps) {
    const [newThreshold, setNewThreshold] = useState<string>(currentThreshold.toString());
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [apiError, setApiError] = useState<string | null>(null);
    const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

    useEffect(() => {
        if (isOpen) {
            setNewThreshold(currentThreshold.toString());
            setApiError(null);
            setFieldErrors({});
        }
    }, [isOpen, currentThreshold]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const threshold = parseInt(newThreshold);
        setApiError(null);
        setFieldErrors({});

        if (isNaN(threshold) || threshold < 0) {
            setFieldErrors({ newLowStockThreshold: 'Please enter a valid non-negative number' });
            return;
        }

        setIsSubmitting(true);
        try {
            await inventoryService.adjustThreshold({
                inventoryId,
                newLowStockThreshold: threshold
            });
            toast.success('Inventory threshold adjusted successfully');
            onSuccess();
            onClose();
        } catch (error: any) {
            console.error('Failed to adjust threshold:', error);

            if (error.validationErrors) {

                setFieldErrors(error.validationErrors);
            }

            const message = error.message || 'Failed to adjust threshold';
            setApiError(message);
            toast.error(message);
        } finally {
            setIsSubmitting(false);
        }
    };

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
            {/* Backdrop */}
            <div
                className="absolute inset-0 bg-background/80 backdrop-blur-sm animate-in fade-in duration-300"
                onClick={onClose}
            />

            {/* Modal */}
            <div className="relative w-full max-w-md mx-4 bg-card border border-border rounded-2xl shadow-2xl animate-in zoom-in-95 duration-300 overflow-hidden">
                {/* Header */}
                <div className="flex items-center justify-between p-6 border-b border-border bg-secondary/10">
                    <div className="flex items-center gap-3">
                        <div className="p-2 bg-primary/10 rounded-lg">
                            <Settings2 className="w-5 h-5 text-primary" />
                        </div>
                        <div>
                            <h2 className="text-xl font-semibold text-foreground">Adjust Threshold</h2>
                            <p className="text-sm text-muted-foreground">Modify safety stock level</p>
                        </div>
                    </div>
                    <button
                        onClick={onClose}
                        className="p-2 rounded-lg hover:bg-secondary transition-colors"
                    >
                        <X className="w-5 h-5 text-muted-foreground" />
                    </button>
                </div>

                {/* Form */}
                <form onSubmit={handleSubmit} className="p-6 space-y-6">
                    {/* Product Context */}
                    <div className="p-4 bg-secondary/50 border border-border rounded-xl space-y-1">
                        <p className="text-sm font-medium text-foreground truncate">{productName}</p>
                        <p className="text-xs font-mono text-muted-foreground">{productSku}</p>
                    </div>

                    <div className="space-y-4">
                        <div className="space-y-2">
                            <Label htmlFor="threshold" className="text-sm font-medium">New Low Stock Threshold</Label>
                            {(() => {
                                // Find any error key that matches 'NewLowStockThreshold' case-insensitively 
                                // or is just 'threshold'
                                const thresholdErrorKey = Object.keys(fieldErrors).find(key => {
                                    const k = key.toLowerCase();
                                    return k === 'newlowstockthreshold' || k === 'threshold' || k.endsWith('.newlowstockthreshold');
                                });
                                const error = thresholdErrorKey ? fieldErrors[thresholdErrorKey] : null;

                                return (
                                    <>
                                        <Input
                                            id="threshold"
                                            type="number"
                                            min="0"
                                            value={newThreshold}
                                            onChange={(e) => {
                                                setNewThreshold(e.target.value);
                                                if (Object.keys(fieldErrors).length > 0) {
                                                    setFieldErrors({});
                                                }
                                                setApiError(null);
                                            }}
                                            className={cn(
                                                "bg-secondary/30",
                                                error && "border-destructive focus-visible:ring-destructive"
                                            )}
                                            placeholder="Enter units..."
                                            autoFocus
                                        />
                                        {error && (
                                            <p className="text-xs font-medium text-destructive mt-1 animate-in fade-in duration-300">
                                                {Array.isArray(error) ? error[0] : error}
                                            </p>
                                        )}
                                    </>
                                );
                            })()}
                            <p className="text-xs text-muted-foreground">
                                Current safety level: <span className="font-semibold text-foreground">{currentThreshold} units</span>
                            </p>
                        </div>

                        <div className="p-3 bg-blue-500/5 border border-blue-500/20 rounded-lg flex gap-3">
                            <AlertCircle className="w-5 h-5 text-blue-500 flex-shrink-0" />
                            <p className="text-xs text-blue-700/80 leading-relaxed font-medium">
                                Lowering the threshold will reduce low-stock alerts, while raising it provides a larger safety buffer.
                            </p>
                        </div>
                        {apiError && (
                            <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-lg flex gap-3 text-destructive animate-in fade-in slide-in-from-top-2 duration-300">
                                <AlertCircle className="w-5 h-5 flex-shrink-0" />
                                <p className="text-sm font-medium">{apiError}</p>
                            </div>
                        )}
                    </div>

                    {/* Actions */}
                    <div className="flex gap-3 pt-2">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={onClose}
                            className="flex-1"
                        >
                            Cancel
                        </Button>
                        <Button
                            type="submit"
                            disabled={isSubmitting || newThreshold === currentThreshold.toString()}
                            className="flex-1"
                        >
                            {isSubmitting ? 'Saving...' : 'Update Threshold'}
                        </Button>
                    </div>
                </form>
            </div>
        </div>
    );
}
