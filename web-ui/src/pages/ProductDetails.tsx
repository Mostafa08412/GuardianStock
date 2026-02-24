import { useState, useEffect, useCallback, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Edit2, Trash2, Package, TrendingUp, TrendingDown, Calendar, Building, BarChart3, History, ShoppingCart, PackagePlus, Barcode } from 'lucide-react';
import { Product, Transaction, Category } from '@/types/inventory';
import { productsService } from '@/api/services/products.service';
import { transactionsService } from '@/api/services/transactions.service';
import { categoriesService } from '@/api/services/categories.service';
import { useRole } from '@/contexts/RoleContext';
import { toast } from 'sonner';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import NewTransactionModal from '@/components/transactions/NewTransactionModal';
import EditProductModal from '@/components/products/EditProductModal';

export default function ProductDetails() {
  const { id: productId } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const onBack = () => navigate('/products');
  const { hasPermission } = useRole();
  const canManageProducts = hasPermission(['admin', 'manager']);

  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const [categoryName, setCategoryName] = useState('Unknown');
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);

  const [isTransactionModalOpen, setIsTransactionModalOpen] = useState(false);
  const [transactionType, setTransactionType] = useState<'sale' | 'purchase'>('sale');
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  // Fetch product data
  const fetchProductData = useCallback(async () => {
    setLoading(true);
    setError(false);
    try {
      const productData = await productsService.getById(productId);
      if (!productData) {
        setError(true);
        return;
      }
      setProduct(productData);
      console.log(productData.categoryId);
      // Fetch transactions (best effort)
      try {
        // We'll search by product name since we don't have a direct product ID filter in the service yet
        // Ideally backend should support filtering by productId
        const txRes = await transactionsService.getList({ search: productData.name, pageSize: 5 });
        setTransactions(txRes.items);
      } catch {
        setTransactions([]);
      }
    } catch (err) {
      console.error(err);
      setError(true);
    } finally {
      setLoading(false);
    }
  }, [productId]);

  // Fetch all categories once on mount
  useEffect(() => {
    categoriesService.getList({ pageSize: 150 }).then(res => {
      setCategories(res.items);
    }).catch(err => {
      console.error("Failed to load categories", err);
    });
  }, []);

  // Update category name whenever product or categories change
  useEffect(() => {
    if (product && categories.length > 0) {
      const cat = categories.find(c => c.id === product.categoryId);
      if (cat) {
        setCategoryName(cat.name);
      }
    }
  }, [product, categories]);

  useEffect(() => {
    fetchProductData();
  }, [fetchProductData]);

  // Format month names
  const monthNames = useMemo(() => ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'], []);

  // Prepare chart data from stock history or fallback to mock
  const chartData = useMemo<{ date: string; stock: number; }[]>(() => {
    if (product?.stockHistory && product.stockHistory.length > 0) {
      return [...product.stockHistory]
        .sort((a, b) => {
          if (a.year !== b.year) return a.year - b.year;
          return a.month - b.month;
        })
        .map(h => ({
          date: `${monthNames[h.month - 1]} ${h.year}`,
          stock: h.quantity
        }));
    }
  }, [product, monthNames]);

  const getStockStatus = (quantity: number, threshold: number) => {
    if (quantity <= threshold * 0.3) return { label: 'Critical', class: 'badge-danger' };
    if (quantity <= threshold) return { label: 'Low', class: 'badge-warning' };
    return { label: 'In Stock', class: 'badge-success' };
  };

  const handleTransactionSubmit = async (transaction: any) => {
    toast.success(`${transaction.type === 'sale' ? 'Sale' : 'Purchase'} recorded successfully!`);
    setIsTransactionModalOpen(false);
    fetchProductData(); // Refresh product data (stock level changes)
  };

  const handleEditSubmit = async (updatedProduct: Product) => {
    toast.success('Product updated successfully!');
    setIsEditModalOpen(false);
    fetchProductData(); // Refresh
  };

  const handleDelete = async () => {
    if (confirm('Are you sure you want to delete this product?')) {
      try {
        await productsService.delete(productId);
        toast.success('Product deleted successfully!');
        onBack(); // Go back to list
      } catch {
        toast.error('Failed to delete product');
      }
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="flex flex-col items-center justify-center h-64 gap-4">
        <p className="text-muted-foreground">Product not found</p>
        <button onClick={onBack} className="btn btn-secondary">
          Go Back
        </button>
      </div>
    );
  }

  const stockStatus = getStockStatus(product.currentStock ?? 0, product.lowStockThreshold ?? 0);

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div className="flex items-center gap-3 sm:gap-4">
          <button
            onClick={onBack}
            className="p-2 rounded-lg bg-secondary hover:bg-secondary/80 text-foreground transition-colors flex-shrink-0"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>
          <div className="min-w-0">
            <div className="flex flex-wrap items-center gap-2">
              <h1 className="text-lg sm:text-xl lg:text-2xl font-bold text-foreground truncate">{product.name}</h1>
              <span className="px-2 py-0.5 bg-secondary text-muted-foreground text-xs font-mono rounded flex-shrink-0">{product.sku}</span>
            </div>
            <p className="text-sm text-muted-foreground">{product.categoryName}</p>
          </div>
        </div>

        <div className="flex items-center gap-2 flex-wrap ml-11 sm:ml-0">
          {/* Quick Transaction Buttons */}
          <button
            onClick={() => {
              setTransactionType('sale');
              setIsTransactionModalOpen(true);
            }}
            className="flex items-center gap-2 px-3 py-1.5 sm:px-4 sm:py-2 bg-emerald-600 text-white rounded-lg hover:bg-emerald-700 transition-colors font-medium text-xs sm:text-sm"
          >
            <ShoppingCart className="w-4 h-4" />
            <span className="hidden sm:inline">Record</span> Sale
          </button>
          <button
            onClick={() => {
              setTransactionType('purchase');
              setIsTransactionModalOpen(true);
            }}
            className="flex items-center gap-2 px-3 py-1.5 sm:px-4 sm:py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium text-xs sm:text-sm"
          >
            <PackagePlus className="w-4 h-4" />
            <span className="hidden sm:inline">Record</span> Purchase
          </button>

          {canManageProducts && (
            <>
              <button
                onClick={() => setIsEditModalOpen(true)}
                className="flex items-center gap-2 px-3 py-1.5 sm:px-4 sm:py-2 bg-secondary border border-border text-foreground rounded-lg hover:bg-secondary/80 transition-colors font-medium text-xs sm:text-sm"
              >
                <Edit2 className="w-4 h-4" />
                <span className="hidden sm:inline">Edit</span>
              </button>
              <button
                onClick={handleDelete}
                className="flex items-center gap-2 px-3 py-1.5 sm:px-4 sm:py-2 bg-destructive text-destructive-foreground rounded-lg hover:bg-destructive/90 transition-colors font-medium text-xs sm:text-sm"
              >
                <Trash2 className="w-4 h-4" />
                <span className="hidden sm:inline">Delete</span>
              </button>
            </>
          )}
        </div>
      </div>

      {/* Main content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left column - Product info */}
        <div className="lg:col-span-2 space-y-6">
          {/* Product Card */}
          <div className="bg-card border border-border rounded-xl p-6">
            <div className="flex flex-col md:flex-row gap-6">
              {/* Product Image Placeholder */}
              <div className="w-full md:w-48 h-48 bg-secondary rounded-xl flex items-center justify-center flex-shrink-0 overflow-hidden">
                {product.imageUrl ? (
                  <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
                ) : (
                  <Package className="w-16 h-16 text-muted-foreground" />
                )}
              </div>

              {/* Product Details */}
              <div className="flex-1 space-y-4">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <Barcode className="w-4 h-4 text-muted-foreground" />
                    <span className="text-sm font-mono text-muted-foreground">{product.sku}</span>
                  </div>
                  <h2 className="text-xl font-semibold text-foreground">{product.name}</h2>
                  <p className="text-muted-foreground mt-1">{product.description}</p>
                </div>

                <div className="flex flex-wrap gap-3">
                  <span className={stockStatus.class}>{stockStatus.label}</span>
                  <span className="badge-secondary">{product.categoryName}</span>
                </div>

                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 pt-4 border-t border-border">
                  <div>
                    <p className="text-sm text-muted-foreground">Price</p>
                    <p className="text-lg font-semibold text-foreground">${(product.price ?? 0).toLocaleString()}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Stock</p>
                    <p className="text-lg font-semibold text-foreground">{product.currentStock ?? 0}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Threshold</p>
                    <p className="text-lg font-semibold text-foreground">{product.lowStockThreshold ?? 0}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Total Value</p>
                    <p className="text-lg font-semibold text-foreground">
                      ${((product.price ?? 0) * (product.currentStock ?? 0)).toLocaleString()}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Stock History Chart */}
          <div className="bg-card border border-border rounded-xl p-6">
            <div className="flex items-center gap-2 mb-6">
              <BarChart3 className="w-5 h-5 text-primary" />
              <h3 className="text-lg font-semibold text-foreground">Stock History</h3>
            </div>
            <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" />
                  <XAxis dataKey="date" stroke="hsl(var(--muted-foreground))" fontSize={12} />
                  <YAxis stroke="hsl(var(--muted-foreground))" fontSize={12} />
                  <Tooltip
                    contentStyle={{
                      backgroundColor: 'hsl(var(--card))',
                      border: '1px solid hsl(var(--border))',
                      borderRadius: '8px',
                    }}
                  />
                  <Line
                    type="monotone"
                    dataKey="stock"
                    stroke="hsl(var(--primary))"
                    strokeWidth={2}
                    dot={{ fill: 'hsl(var(--primary))' }}
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Right column - Additional info */}
        <div className="space-y-6">
          {/* Supplier Info */}
          <div className="bg-card border border-border rounded-xl p-6">
            <div className="flex items-center gap-2 mb-4">
              <Building className="w-5 h-5 text-primary" />
              <h3 className="font-semibold text-foreground">Supplier</h3>
            </div>
            <p className="text-foreground font-medium">{product.supplier}</p>
            <p className="text-sm text-muted-foreground mt-1">Primary supplier for this product</p>
          </div>

          {/* Dates */}
          <div className="bg-card border border-border rounded-xl p-6">
            <div className="flex items-center gap-2 mb-4">
              <Calendar className="w-5 h-5 text-primary" />
              <h3 className="font-semibold text-foreground">Timeline</h3>
            </div>
            <div className="space-y-3">
              <div>
                <p className="text-sm text-muted-foreground">Created</p>
                <p className="text-foreground">{product.createdAt ? new Date(product.createdAt).toLocaleDateString('en-US', { dateStyle: 'medium' }) : 'N/A'}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Last Updated</p>
                <p className="text-foreground">{product.updatedAt ? new Date(product.updatedAt).toLocaleDateString('en-US', { dateStyle: 'medium' }) : 'N/A'}</p>
              </div>
            </div>
          </div>

          {/* Quick Stats */}
          <div className="bg-card border border-border rounded-xl p-6">
            <div className="flex items-center gap-2 mb-4">
              <TrendingUp className="w-5 h-5 text-primary" />
              <h3 className="font-semibold text-foreground">Quick Stats</h3>
            </div>
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Monthly Sales</span>

                {/* Logic: Only show the percentage badge if sales are HIGHER than last month */}
                <div className="flex items-center gap-2">
                  <span className="text-foreground text-sm font-medium">
                    ${product.currentMonthSales.toLocaleString()}
                  </span>

                  {product.currentMonthSales > product.lastMonthSales && product.lastMonthSales > 0 && (
                    <span className="flex items-center gap-1 text-emerald-500 text-xs font-bold bg-emerald-500/10 px-2 py-0.5 rounded-full">
                      <TrendingUp className="w-3 h-3" />
                      +{(((product.currentMonthSales - product.lastMonthSales) / product.lastMonthSales) * 100).toFixed(1)}%
                    </span>
                  )}

                  {product.currentMonthSales < product.lastMonthSales && (
                    <TrendingDown className="w-4 h-4 text-red-500" />
                  )}
                </div>
              </div>

              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Avg. Restock Time</span>
                <span className="text-foreground text-sm font-medium">{product.avgReStockTime} days</span>
              </div>

              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">Last Restocked</span>
                <span className="text-foreground text-sm font-medium">
                  {new Date(product.lastRestocked).toLocaleDateString('en-US', { dateStyle: 'medium' })}
                </span>
              </div>
            </div>
          </div>

          {/* Recent Transactions */}
          <div className="bg-card border border-border rounded-xl p-6">
            <div className="flex items-center gap-2 mb-4">
              <History className="w-5 h-5 text-primary" />
              <h3 className="font-semibold text-foreground">Recent Activity</h3>
            </div>
            <div className="space-y-3">
              {product.recentActivities.length > 0 ? (
                product.recentActivities.map((transaction) => (
                  <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-secondary transition-colors">
                    <div className={`p-1.5 rounded-lg ${transaction.type === 0 ? 'bg-emerald-500/10' : 'bg-blue-500/10'}`}>
                      {transaction.type === 0 ? (
                        <TrendingUp className="w-4 h-4 text-emerald-500" />
                      ) : (
                        <TrendingDown className="w-4 h-4 text-blue-500" />
                      )}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm text-foreground capitalize">{transaction.type === 0 ? 'sale' : 'restock'}</p>
                      <p className="text-xs text-muted-foreground">
                        {new Date(transaction.date).toLocaleDateString()}
                      </p>
                    </div>
                    <p className={`text-sm font-medium ${transaction.type === 0 ? 'text-emerald-500' : 'text-blue-500'}`}>
                      {transaction.type === 0 ? '-' : '+'}{transaction.quantity}
                    </p>
                  </div>
                ))
              ) : (
                <p className="text-sm text-muted-foreground">No recent activity</p>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Transaction Modal */}
      <NewTransactionModal
        isOpen={isTransactionModalOpen}
        onClose={() => setIsTransactionModalOpen(false)}
        onSubmit={handleTransactionSubmit}
        preselectedProduct={product}
        preselectedType={transactionType}
      />

      {/* Edit Product Modal */}
      <EditProductModal
        isOpen={isEditModalOpen}
        product={product}
        categoryId={product.categoryId}
        categories={categories}
        onClose={() => setIsEditModalOpen(false)}
        onSubmit={handleEditSubmit}
      />
    </div>
  );
}
