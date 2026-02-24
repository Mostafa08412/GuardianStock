import { useEffect, useState, useCallback } from 'react';
import { ArrowLeft, Edit2, Trash2, Package, DollarSign, TrendingUp, FolderOpen, Loader2 } from 'lucide-react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { CategoryDetails as CategoryDetailsType, CategoryListItem } from '@/types/inventory';
import { categoriesService } from '@/api';
import { mockProducts } from '@/data/mockData'; // Keeping for now if products list not in category details
import { cn } from '@/lib/utils';
import { toast } from 'sonner';
import EditCategoryModal from '@/components/categories/EditCategoryModal';
import DeleteCategoryDialog from '@/components/categories/DeleteCategoryDialog';

export default function CategoryDetails() {
  const { id: categoryId } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const onBack = useCallback(() => navigate('/categories'), [navigate]);
  const onViewProduct = useCallback((productId: string) => navigate(`/products/${productId}`), [navigate]);
  const [category, setCategory] = useState<CategoryDetailsType | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Modal states
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);

  const fetchCategoryDetails = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await categoriesService.getById(categoryId);
      if (data) {
        setCategory(data);
      } else {
        toast.error('Category not found');
        onBack();
      }
    } catch (error) {
      console.error('Failed to fetch category details:', error);
      toast.error('Failed to load category details');
    } finally {
      setIsLoading(false);
    }
  }, [categoryId, onBack]);

  useEffect(() => {
    if (categoryId) {
      fetchCategoryDetails();
    }
  }, [categoryId, fetchCategoryDetails]);

  const handleEditSave = async (updatedCategory: CategoryListItem) => {
    try {
      await categoriesService.update({
        id: updatedCategory.id,
        name: updatedCategory.name,
        description: updatedCategory.description
      });
      // Refresh details
      const data = await categoriesService.getById(categoryId);
      if (data) setCategory(data);
      setShowEditModal(false);
    } catch (error) {
      console.error("Failed to update category", error);
      throw error; // Modal handles the error toast
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await categoriesService.delete(id);
      onBack(); // Go back to list after delete
    } catch (error) {
      console.error("Failed to delete category", error);
      throw error; // Modal handles the error toast
    }
  }

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!category) {
    return null;
  }

  // TODO: Fetch real products for this category instead of mock
  const categoryProducts = mockProducts.filter(p => p.categoryId === category.id);

  // Adapter for EditModal which expects CategoryListItem
  const categoryForEdit: CategoryListItem = {
    id: category.id,
    name: category.name,
    description: category.description,
    productCount: category.productsCount
  };

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={onBack}>
            <ArrowLeft className="w-5 h-5" />
          </Button>
          <div>
            <h2 className="text-xl font-bold text-foreground">{category.name}</h2>
            <p className="text-sm text-muted-foreground">{category.description}</p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" onClick={() => setShowEditModal(true)}>
            <Edit2 className="w-4 h-4 mr-2" />
            Edit
          </Button>
          <Button variant="destructive" onClick={() => setShowDeleteDialog(true)}>
            <Trash2 className="w-4 h-4 mr-2" />
            Delete
          </Button>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-xl bg-primary/20 flex items-center justify-center">
              <Package className="w-6 h-6 text-primary" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total Products</p>
              <p className="text-2xl font-bold text-foreground">{category.productsCount}</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-xl bg-success/20 flex items-center justify-center">
              <DollarSign className="w-6 h-6 text-success" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total Value</p>
              <p className="text-2xl font-bold text-foreground">${category.totalValue.toLocaleString()}</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-xl bg-info/20 flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-info" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Avg. Price</p>
              <p className="text-2xl font-bold text-foreground">${category.averagePrice.toFixed(2)}</p>
            </div>
          </div>
        </div>

        <div className="bg-card border border-border rounded-xl p-6">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-xl bg-warning/20 flex items-center justify-center">
              <FolderOpen className="w-6 h-6 text-warning" />
            </div>
            <div>
              <p className="text-sm text-muted-foreground">Total Stock</p>
              <p className="text-2xl font-bold text-foreground">
                {category.totalStock}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Products Table */}
      <div className="bg-card border border-border rounded-xl">
        <div className="p-6 border-b border-border">
          <h3 className="text-lg font-semibold text-foreground">Products in this Category</h3>
          <p className="text-sm text-muted-foreground">{categoryProducts.length} products found</p>
        </div>

        {categoryProducts.length === 0 ? (
          <div className="p-12 text-center">
            <Package className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
            <h4 className="text-lg font-semibold text-foreground mb-2">No products found</h4>
            <p className="text-muted-foreground">This category doesn't have any products yet.</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-border">
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Product</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">SKU</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Price</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Stock</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Status</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {categoryProducts.map((product) => {
                  const isLowStock = product.stockQuantity <= product.lowStockThreshold;
                  return (
                    <tr
                      key={product.id}
                      className="hover:bg-secondary/50 transition-colors cursor-pointer"
                      onClick={() => onViewProduct?.(product.id)}
                    >
                      <td className="px-6 py-4">
                        <div>
                          <p className="font-medium text-foreground">{product.name}</p>
                          <p className="text-sm text-muted-foreground">{product.description}</p>
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        <span className="text-sm font-mono text-muted-foreground">{product.sku}</span>
                      </td>
                      <td className="px-6 py-4">
                        <span className="text-sm font-semibold text-foreground">${product.price.toLocaleString()}</span>
                      </td>
                      <td className="px-6 py-4">
                        <span className="text-sm text-foreground">{product.stockQuantity}</span>
                      </td>
                      <td className="px-6 py-4">
                        <span className={cn(
                          'text-xs font-medium px-2.5 py-1 rounded-full',
                          isLowStock ? 'bg-destructive/20 text-destructive' : 'bg-success/20 text-success'
                        )}>
                          {isLowStock ? 'Low Stock' : 'In Stock'}
                        </span>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <EditCategoryModal
        isOpen={showEditModal}
        category={categoryForEdit}
        onClose={() => setShowEditModal(false)}
        onSave={handleEditSave}
      />

      <DeleteCategoryDialog
        isOpen={showDeleteDialog}
        category={categoryForEdit}
        onClose={() => setShowDeleteDialog(false)}
        onDelete={handleDelete}
      />
    </div>
  );
}
