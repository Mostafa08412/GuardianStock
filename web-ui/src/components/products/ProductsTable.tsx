import { useState, useEffect, useCallback, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Search, Plus, Edit2, Trash2, Upload, Eye, X, Package } from 'lucide-react';
import { Product, ProductListItem, Category } from '@/types/inventory';
import { productsService } from '@/api/services/products.service';
import { categoriesService } from '@/api/services/categories.service';
import { useRole } from '@/contexts/RoleContext';
import { toast } from 'sonner';
import CSVImportModal from './CSVImportModal';
import EditProductModal from './EditProductModal';
import AddProductModal from './AddProductModal';
import { PaginationFooter } from '@/components/common/PaginationFooter';
import { PermissionGate } from '@/components/auth/PermissionGate';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

interface ProductsTableProps {
  onViewProduct?: (productId: string) => void;
}

export default function ProductsTable({ onViewProduct }: ProductsTableProps) {
  const [searchParams, setSearchParams] = useSearchParams();

  // URL parameters (single source of truth)
  const searchQuery = searchParams.get('search') || '';
  const selectedCategory = searchParams.get('category') || 'all';
  const stockFilter = (searchParams.get('stock') as 'all' | 'in-stock' | 'low' | 'critical') || 'all';
  const priceRange = (searchParams.get('price') as 'all' | '0-100' | '100-500' | '500-1000' | '1000+') || 'all';
  const sortBy = (searchParams.get('sortBy') as 'name' | 'price' | 'stock' | 'sku') || 'name';
  const sortOrder = (searchParams.get('sortOrder') as 'asc' | 'desc') || 'asc';
  const currentPage = parseInt(searchParams.get('page') || '1');
  const itemsPerPage = parseInt(searchParams.get('pageSize') || '10');

  // Local UI state
  const [searchInput, setSearchInput] = useState(searchQuery);
  const [products, setProducts] = useState<ProductListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [categories, setCategories] = useState<Category[]>([]);

  const [isImportModalOpen, setIsImportModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | ProductListItem | null>(null);

  const { hasPermission } = useRole();
  const canManageProducts = hasPermission(['admin', 'manager']);

  // Update specific URL params helper
  const updateSearchParams = useCallback((updates: Record<string, string | null>, resetPage = true) => {
    setSearchParams(prev => {
      const next = new URLSearchParams(prev);
      Object.entries(updates).forEach(([key, value]) => {
        if (value === null || value === 'all' || (key === 'sortBy' && value === 'name') || (key === 'sortOrder' && value === 'asc')) {
          next.delete(key);
        } else {
          next.set(key, value);
        }
      });
      if (resetPage) next.delete('page');
      return next;
    }, { replace: true });
  }, [setSearchParams]);

  // Fetch categories once on mount
  useEffect(() => {
    categoriesService.getList({ pageSize: 100 }).then(res => {
      setCategories(res.items);
    }).catch(() => {
      toast.error('Failed to load categories');
    });
  }, []);

  // Compute price range values
  const getPriceRange = useCallback((): { minPrice?: number; maxPrice?: number } => {
    switch (priceRange) {
      case '0-100': return { minPrice: 0, maxPrice: 100 };
      case '100-500': return { minPrice: 100, maxPrice: 500 };
      case '500-1000': return { minPrice: 500, maxPrice: 1000 };
      case '1000+': return { minPrice: 1000 };
      default: return {};
    }
  }, [priceRange]);

  // Fetch products triggered by URL change
  const fetchProducts = useCallback(async () => {
    setLoading(true);
    try {
      const { minPrice, maxPrice } = getPriceRange();
      const result = await productsService.getList({
        search: searchQuery || undefined,
        categoryId: selectedCategory !== 'all' ? selectedCategory : undefined,
        stockFilter: stockFilter !== 'all' ? stockFilter : undefined,
        minPrice,
        maxPrice,
        sortBy: sortBy as any,
        sortOrder: sortOrder as any,
        page: currentPage,
        pageSize: itemsPerPage,
      });
      setProducts(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err) {
      toast.error('Failed to load products');
    } finally {
      setLoading(false);
    }
  }, [searchQuery, selectedCategory, stockFilter, getPriceRange, sortBy, sortOrder, currentPage, itemsPerPage]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  // Sync search input with URL when typing (debounced)
  useEffect(() => {
    const timer = setTimeout(() => {
      if (searchInput !== searchQuery) {
        updateSearchParams({ search: searchInput || null });
      }
    }, 400);
    return () => clearTimeout(timer);
  }, [searchInput, searchQuery, updateSearchParams]);

  // Handle browser back button (sync local search input)
  useEffect(() => {
    setSearchInput(searchQuery);
  }, [searchQuery]);

  const getStockStatus = (quantity: number, threshold: number) => {
    if (quantity <= threshold * 0.3) return { label: 'Critical', class: 'badge-danger' };
    if (quantity <= threshold) return { label: 'Low', class: 'badge-warning' };
    return { label: 'In Stock', class: 'badge-success' };
  };

  const getCategoryName = (categoryId: string) => {
    return categories.find(c => c.id === categoryId)?.name || 'Unknown';
  };

  const handleEdit = (e: React.MouseEvent, product: ProductListItem) => {
    e.stopPropagation();
    setSelectedProduct(product);
    setIsEditModalOpen(true);
  };

  const handleDelete = async (e: React.MouseEvent, productId: string) => {
    e.stopPropagation();
    if (confirm('Are you sure you want to delete this product?')) {
      try {
        await productsService.delete(productId);
        toast.success('Product deleted successfully!');
        fetchProducts();
      } catch {
        toast.error('Failed to delete product');
      }
    }
  };

  const clearFilters = () => {
    setSearchInput('');
    setSearchParams(new URLSearchParams(), { replace: true });
  };

  const hasActiveFilters = searchInput || selectedCategory !== 'all' || stockFilter !== 'all' || priceRange !== 'all';

  return (
    <div className="bg-card border border-border rounded-xl animate-fade-in">
      {/* Header */}
      <div className="p-6 border-b border-border">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div>
            <h3 className="text-lg font-semibold text-foreground">Products Inventory</h3>
            <p className="text-sm text-muted-foreground">{totalCount} products found</p>
          </div>

          <div className="flex items-center gap-2">
            <PermissionGate action="products.import">
              <button
                onClick={() => setIsImportModalOpen(true)}
                className="flex items-center gap-2 px-4 py-2 bg-secondary border border-border text-foreground rounded-lg hover:bg-secondary/80 transition-colors font-medium text-sm"
              >
                <Upload className="w-4 h-4" />
                Import CSV
              </button>
            </PermissionGate>
            <PermissionGate action="products.create">
              <button
                onClick={() => setIsAddModalOpen(true)}
                className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium text-sm"
              >
                <Plus className="w-4 h-4" />
                Add Product
              </button>
            </PermissionGate>
          </div>
        </div>

        {/* Filters */}
        <div className="space-y-3 mt-4">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              type="text"
              placeholder="Search by name, SKU, or description..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              className="w-full pl-10 pr-4 py-2 bg-secondary border border-border rounded-lg text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
            />
          </div>

          <div className="flex flex-wrap gap-3">
            <Select value={selectedCategory} onValueChange={(v) => updateSearchParams({ category: v })}>
              <SelectTrigger className="w-[160px] bg-secondary border-border">
                <SelectValue placeholder="Category" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Categories</SelectItem>
                {categories.map(cat => (
                  <SelectItem key={cat.id} value={cat.id}>{cat.name}</SelectItem>
                ))}
              </SelectContent>
            </Select>

            <Select value={stockFilter} onValueChange={(v) => updateSearchParams({ stock: v })}>
              <SelectTrigger className="w-[140px] bg-secondary border-border">
                <SelectValue placeholder="Stock Status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Stock</SelectItem>
                <SelectItem value="in-stock">Normal</SelectItem>
                <SelectItem value="low">Low Stock</SelectItem>
                <SelectItem value="critical">Critical</SelectItem>
              </SelectContent>
            </Select>

            <Select value={priceRange} onValueChange={(v) => updateSearchParams({ price: v })}>
              <SelectTrigger className="w-[140px] bg-secondary border-border">
                <SelectValue placeholder="Price Range" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Prices</SelectItem>
                <SelectItem value="0-100">$0 - $100</SelectItem>
                <SelectItem value="100-500">$100 - $500</SelectItem>
                <SelectItem value="500-1000">$500 - $1,000</SelectItem>
                <SelectItem value="1000+">$1,000+</SelectItem>
              </SelectContent>
            </Select>

            <Select value={`${sortBy}-${sortOrder}`} onValueChange={(v) => {
              const [sort, order] = v.split('-');
              updateSearchParams({ sortBy: sort, sortOrder: order });
            }}>
              <SelectTrigger className="w-[160px] bg-secondary border-border">
                <SelectValue placeholder="Sort By" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="name-asc">Name (A-Z)</SelectItem>
                <SelectItem value="name-desc">Name (Z-A)</SelectItem>
                <SelectItem value="sku-asc">SKU (A-Z)</SelectItem>
                <SelectItem value="sku-desc">SKU (Z-A)</SelectItem>
                <SelectItem value="price-asc">Price (Low-High)</SelectItem>
                <SelectItem value="price-desc">Price (High-Low)</SelectItem>
                <SelectItem value="stock-asc">Stock (Low-High)</SelectItem>
                <SelectItem value="stock-desc">Stock (High-Low)</SelectItem>
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
      {loading && (
        <div className="p-12 text-center">
          <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-primary border-r-transparent" />
          <p className="mt-2 text-sm text-muted-foreground">Loading products...</p>
        </div>
      )}

      {/* Table Content */}
      {!loading && (
        <>
          <div className="hidden lg:block overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-border">
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">SKU</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Product</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Category</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Price</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Stock</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Status</th>
                  <th className="px-6 py-4 text-right text-xs font-semibold text-muted-foreground uppercase tracking-wider">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {products.map((product) => {
                  const stockStatus = getStockStatus(product.currentStock, product.lowStockThreshold);
                  return (
                    <tr key={product.id} className="table-row-hover cursor-pointer" onClick={() => onViewProduct?.(product.id)}>
                      <td className="px-6 py-4">
                        <span className="text-sm font-mono text-muted-foreground">{product.sku}</span>
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-3">
                          <div className="w-10 h-10 rounded-lg bg-secondary flex items-center justify-center overflow-hidden flex-shrink-0">
                            {product.imageUrl ? (
                              <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
                            ) : (
                              <Package className="w-5 h-5 text-muted-foreground" />
                            )}
                          </div>
                          <div>
                            <p className="text-sm font-medium text-foreground">{product.name}</p>
                            <p className="text-xs text-muted-foreground truncate max-w-[200px]">{product.description}</p>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        <span className="text-sm text-foreground">{getCategoryName(product.categoryId)}</span>
                      </td>
                      <td className="px-6 py-4">
                        <span className="text-sm font-semibold text-foreground">${product.price.toLocaleString()}</span>
                      </td>
                      <td className="px-6 py-4">
                        <span className="text-sm text-foreground">{product.currentStock}</span>
                      </td>
                      <td className="px-6 py-4">
                        <span className={stockStatus.class}>{stockStatus.label}</span>
                      </td>
                      <td className="px-6 py-4 text-right" onClick={(e) => e.stopPropagation()}>
                        <div className="flex items-center justify-end gap-2">
                          <button
                            onClick={() => onViewProduct?.(product.id)}
                            className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                          >
                            <Eye className="w-4 h-4" />
                          </button>
                          <PermissionGate action="products.edit">
                            <button
                              onClick={(e) => handleEdit(e, product)}
                              className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                            >
                              <Edit2 className="w-4 h-4" />
                            </button>
                          </PermissionGate>
                          <PermissionGate action="products.delete">
                            <button
                              onClick={(e) => handleDelete(e, product.id)}
                              className="p-2 rounded-lg hover:bg-destructive/20 text-muted-foreground hover:text-destructive transition-colors"
                            >
                              <Trash2 className="w-4 h-4" />
                            </button>
                          </PermissionGate>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          <div className="lg:hidden divide-y divide-border">
            {products.map((product) => {
              const stockStatus = getStockStatus(product.currentStock, product.lowStockThreshold);
              return (
                <div
                  key={product.id}
                  className="p-4 hover:bg-secondary/50 transition-colors cursor-pointer"
                  onClick={() => onViewProduct?.(product.id)}
                >
                  <div className="flex items-start gap-4 mb-2">
                    <div className="w-12 h-12 rounded-lg bg-secondary flex items-center justify-center overflow-hidden flex-shrink-0">
                      {product.imageUrl ? (
                        <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
                      ) : (
                        <Package className="w-6 h-6 text-muted-foreground" />
                      )}
                    </div>
                    <div className="min-w-0 flex-1">
                      <p className="text-sm font-medium text-foreground truncate">{product.name}</p>
                      <p className="text-xs font-mono text-muted-foreground">{product.sku}</p>
                    </div>
                  </div>
                  <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-xs">
                    <span className="text-foreground font-semibold">${product.price.toLocaleString()}</span>
                    <span className="text-muted-foreground">{getCategoryName(product.categoryId)}</span>
                    <span className="text-muted-foreground">Stock: {product.currentStock}</span>
                    <span className={stockStatus.class}>{stockStatus.label}</span>
                  </div>
                </div>
              );
            })}
          </div>
        </>
      )}

      {!loading && products.length === 0 && (
        <div className="p-12 text-center">
          <p className="text-muted-foreground">No products found</p>
        </div>
      )}

      {/* Pagination Footer */}
      {!loading && totalCount > 0 && (
        <PaginationFooter
          currentPage={currentPage}
          pageSize={itemsPerPage}
          totalCount={totalCount}
          totalPages={totalPages}
          onPageChange={(page) => updateSearchParams({ page: page.toString() }, false)}
          onPageSizeChange={(size) => updateSearchParams({ pageSize: size.toString() })}
        />
      )}

      <CSVImportModal isOpen={isImportModalOpen} onClose={() => setIsImportModalOpen(false)} />

      <EditProductModal
        isOpen={isEditModalOpen}
        product={selectedProduct}
        categoryId={selectedProduct?.categoryId}
        categories={categories}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedProduct(null);
        }}
        onSubmit={() => fetchProducts()}
      />

      <AddProductModal
        isOpen={isAddModalOpen}
        categories={categories}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={() => fetchProducts()}
      />
    </div>
  );
}
