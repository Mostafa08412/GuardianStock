import { useState, useEffect, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { X, ArrowUpRight, ArrowDownLeft, Hash, DollarSign, Search, CheckCircle2 } from 'lucide-react';
import { Product, ProductListItem } from '@/types/inventory';
import { productsService } from '@/api/services/products.service';
import { transactionsService } from '@/api/services/transactions.service';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { toast } from 'sonner';

interface NewTransactionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (transaction: {
    productId: string;
    quantity: number;
    type: 'sale' | 'purchase';
  }) => void;
  preselectedProduct?: Product | null;
  preselectedType?: 'sale' | 'purchase';
}

interface TransactionFormValues {
  productId: string;
  quantity: number;
}

export default function NewTransactionModal({
  isOpen,
  onClose,
  onSubmit,
  preselectedProduct = null,
  preselectedType
}: NewTransactionModalProps) {
  const { user } = useAuth();
  const [transactionType, setTransactionType] = useState<'sale' | 'purchase'>(preselectedType || 'sale');
  const [skuInput, setSkuInput] = useState('');
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(preselectedProduct);
  const [searchResults, setSearchResults] = useState<ProductListItem[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [showDropdown, setShowDropdown] = useState(false);
  const [skuError, setSkuError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit: handleFormSubmit,
    setError,
    reset,
    watch,
    formState: { errors }
  } = useForm<TransactionFormValues>({
    defaultValues: {
      productId: preselectedProduct?.id || '',
      quantity: 1
    }
  });

  const quantity = watch('quantity');
  const totalAmount = selectedProduct ? selectedProduct.price * (parseInt(quantity as any) || 0) : 0;

  // Reset when modal opens with preselected product
  useEffect(() => {
    if (isOpen && preselectedProduct) {
      setSelectedProduct(preselectedProduct);
      setSkuInput('');
      setSkuError('');
      setSearchResults([]);
      setShowDropdown(false);
      if (preselectedType) {
        setTransactionType(preselectedType);
      }
    }
  }, [isOpen, preselectedProduct, preselectedType]);

  // Search products by SKU using paginated API
  const searchProducts = useCallback(async (query: string) => {
    if (!query.trim()) {
      setSearchResults([]);
      setShowDropdown(false);
      setSkuError('');
      return;
    }

    setIsSearching(true);
    setSkuError('');

    try {
      const result = await productsService.getList({ search: query, pageSize: 5 });
      setSearchResults(result.items);
      setShowDropdown(result.items.length > 0);
      if (result.items.length === 0) {
        setSkuError('No products found matching this SKU or name');
      }
    } catch (error) {
      setSkuError('Error searching for products');
      setSearchResults([]);
      setShowDropdown(false);
      console.error('Product search error:', error);
    } finally {
      setIsSearching(false);
    }
  }, []);

  // Debounced search
  useEffect(() => {
    if (!preselectedProduct && !selectedProduct) {
      const timeoutId = setTimeout(() => {
        searchProducts(skuInput);
      }, 400);
      return () => clearTimeout(timeoutId);
    }
  }, [skuInput, searchProducts, preselectedProduct, selectedProduct]);

  // Handle selecting a product from search results
  const handleSelectProduct = async (item: ProductListItem) => {
    setShowDropdown(false);
    setIsSearching(true);
    try {
      // Fetch full product details by ID
      const product = await productsService.getById(item.id);
      if (product) {
        setSelectedProduct(product);
        setSkuInput('');
        setSkuError('');
        setSearchResults([]);
      } else {
        setSkuError('Could not load product details');
      }
    } catch (error) {
      setSkuError('Error loading product details');
      console.error(error);
    } finally {
      setIsSearching(false);
    }
  };

  const onSubmitForm = async (data: TransactionFormValues) => {
    if (!selectedProduct) {
      setSkuError('Please select a product');
      return;
    }

    setIsSubmitting(true);

    try {
      const request = {
        productId: selectedProduct.id,
        quantity: parseInt(data.quantity as any),
      };

      transactionType === 'sale'
        ? await transactionsService.recordSale(request)
        : await transactionsService.recordPurchase(request);

      toast.success(`${transactionType === 'sale' ? 'Sale' : 'Purchase'} recorded successfully!`);

      onSubmit({
        productId: selectedProduct.id,
        quantity: data.quantity,
        type: transactionType,
      });

      resetForm();
      onClose();
    } catch (error) {
      toast.error(`Failed to record ${transactionType}`);
      console.error('Transaction error:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const resetForm = () => {
    setSelectedProduct(null);
    setSkuInput('');
    setSkuError('');
    reset();
    setTransactionType('sale');
    setSearchResults([]);
    setShowDropdown(false);
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  const clearSelectedProduct = () => {
    setSelectedProduct(null);
    setSkuInput('');
    setSkuError('');
    setSearchResults([]);
    setShowDropdown(false);
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-background/80 backdrop-blur-sm"
        onClick={handleClose}
      />

      {/* Modal */}
      <div className="relative w-full max-w-lg mx-4 bg-card border border-border rounded-2xl shadow-2xl animate-fade-in max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-border">
          <div>
            <h2 className="text-xl font-semibold text-foreground">New Transaction</h2>
            <p className="text-sm text-muted-foreground mt-1">Record a sale or purchase transaction</p>
          </div>
          <button
            onClick={handleClose}
            className="p-2 rounded-lg hover:bg-secondary transition-colors"
          >
            <X className="w-5 h-5 text-muted-foreground" />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleFormSubmit(onSubmitForm)} className="p-6 space-y-6">
          {/* Transaction Type */}
          <div className="space-y-3">
            <Label className="text-foreground font-medium">Transaction Type</Label>
            <div className="grid grid-cols-2 gap-3">
              <button
                type="button"
                onClick={() => setTransactionType('sale')}
                className={cn(
                  'flex items-center justify-center gap-3 p-4 rounded-xl border-2 transition-all',
                  transactionType === 'sale'
                    ? 'border-emerald-500 bg-emerald-500/10 text-emerald-500'
                    : 'border-border bg-secondary text-muted-foreground hover:border-muted-foreground'
                )}
              >
                <ArrowUpRight className="w-5 h-5" />
                <span className="font-medium">Sale</span>
              </button>
              <button
                type="button"
                onClick={() => setTransactionType('purchase')}
                className={cn(
                  'flex items-center justify-center gap-3 p-4 rounded-xl border-2 transition-all',
                  transactionType === 'purchase'
                    ? 'border-blue-500 bg-blue-500/10 text-blue-500'
                    : 'border-border bg-secondary text-muted-foreground hover:border-muted-foreground'
                )}
              >
                <ArrowDownLeft className="w-5 h-5" />
                <span className="font-medium">Purchase</span>
              </button>
            </div>
          </div>

          {/* Product Selection via SKU */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <Search className="w-4 h-4" />
              {preselectedProduct ? 'Selected Product' : 'Search by Product SKU'}
            </Label>

            {selectedProduct ? (
              <div className="flex items-center gap-2 p-3 bg-emerald-500/10 border border-emerald-500/30 rounded-xl animate-fade-in">
                <CheckCircle2 className="w-5 h-5 text-emerald-500 flex-shrink-0" />
                <div className="flex-1 min-w-0">
                  <p className="text-foreground font-medium truncate">{selectedProduct.name}</p>
                  <p className="text-xs text-muted-foreground">SKU: {selectedProduct.sku || selectedProduct.id}</p>
                </div>
                {!preselectedProduct && (
                  <button
                    type="button"
                    onClick={clearSelectedProduct}
                    className="p-1.5 rounded-lg hover:bg-destructive/20 text-muted-foreground hover:text-destructive transition-colors"
                  >
                    <X className="w-4 h-4" />
                  </button>
                )}
              </div>
            ) : (
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                <Input
                  type="text"
                  value={skuInput}
                  onChange={(e) => setSkuInput(e.target.value)}
                  onFocus={() => searchResults.length > 0 && setShowDropdown(true)}
                  placeholder="Enter product SKU to search..."
                  className="pl-10 bg-secondary border-border"
                />
                {isSearching && (
                  <div className="absolute right-3 top-1/2 -translate-y-1/2">
                    <div className="w-4 h-4 border-2 border-primary border-t-transparent rounded-full animate-spin" />
                  </div>
                )}

                {/* Search Results Dropdown */}
                {showDropdown && searchResults.length > 0 && (
                  <div className="absolute z-10 w-full mt-1 bg-card border border-border rounded-xl shadow-lg overflow-hidden">
                    {searchResults.map((item) => (
                      <button
                        key={item.id}
                        type="button"
                        onClick={() => handleSelectProduct(item)}
                        className="w-full flex items-center gap-3 px-4 py-3 hover:bg-secondary/80 transition-colors text-left"
                      >
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium text-foreground truncate">{item.name}</p>
                          <div className="flex items-center gap-2 text-xs text-muted-foreground">
                            <span className="font-mono">{item.sku || item.id}</span>
                            <span>•</span>
                            <span>${item.price.toLocaleString()}</span>
                            <span>•</span>
                            <span>{item.currentStock} in stock</span>
                          </div>
                        </div>
                      </button>
                    ))}
                  </div>
                )}
              </div>
            )}

            {skuError && <p className="text-sm text-destructive">{skuError}</p>}
          </div>

          {/* Selected Product Info */}
          {selectedProduct && (
            <div className="p-4 bg-secondary rounded-xl space-y-2 animate-fade-in">
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Current Stock</span>
                <span className="text-foreground font-medium">{selectedProduct.stockQuantity ?? selectedProduct.currentStock ?? 0} units</span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Unit Price</span>
                <span className="text-foreground font-medium">${selectedProduct.price.toLocaleString()}</span>
              </div>
              {selectedProduct.supplier && (
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Supplier</span>
                  <span className="text-foreground font-medium">{selectedProduct.supplier}</span>
                </div>
              )}
            </div>
          )}

          {/* Quantity */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium">Quantity</Label>
            <div className="relative">
              <Hash className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
              <Input
                type="number"
                {...register('quantity', {
                  required: 'Quantity is required',
                  min: { value: 1, message: 'Quantity must be at least 1' },
                  validate: (val) => {
                    if (transactionType === 'sale' && selectedProduct) {
                      const stock = selectedProduct.stockQuantity ?? selectedProduct.currentStock ?? 0;
                      if (val > stock) return 'Quantity exceeds available stock';
                    }
                    return true;
                  }
                })}
                placeholder="Enter quantity"
                className={cn(
                  "pl-10 bg-secondary border-border",
                  errors.quantity && "border-destructive focus:ring-destructive/50"
                )}
              />
            </div>
            {errors.quantity && (
              <p className="text-sm text-destructive">{errors.quantity.message}</p>
            )}
          </div>

          {/* Total Amount Preview */}
          {selectedProduct && quantity && (
            <div className="p-4 bg-gradient-to-r from-primary/10 to-primary/5 border border-primary/20 rounded-xl animate-fade-in">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <DollarSign className="w-5 h-5 text-primary" />
                  <span className="text-foreground font-medium">Total Amount</span>
                </div>
                <span className="text-2xl font-bold text-primary">
                  ${totalAmount.toLocaleString()}
                </span>
              </div>
            </div>
          )}

          {/* User Info */}
          <div className="p-4 bg-secondary/50 border border-border rounded-xl">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary font-semibold text-sm border border-primary/20">
                {user?.fullName?.slice(0, 2).toUpperCase() || user?.userName?.slice(0, 2).toUpperCase() || '??'}
              </div>
              <div>
                <p className="text-sm font-semibold text-foreground">{user?.fullName || user?.userName || 'Unknown User'}</p>
                <p className="text-xs text-muted-foreground">Recording this transaction</p>
              </div>
            </div>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-2">
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              className="flex-1"
            >
              Cancel
            </Button>
            <Button
              type="submit"
              disabled={!selectedProduct || !quantity || isSubmitting ||
                (transactionType === 'sale' && selectedProduct && (quantity as number) > (selectedProduct.stockQuantity ?? selectedProduct.currentStock ?? 0))}
              className={cn(
                'flex-1',
                transactionType === 'sale'
                  ? 'bg-emerald-600 hover:bg-emerald-700'
                  : 'bg-blue-600 hover:bg-blue-700'
              )}
            >
              {isSubmitting ? 'Recording...' : `Record ${transactionType === 'sale' ? 'Sale' : 'Purchase'}`}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
