import { useState, useEffect, useCallback } from 'react';
import { X, Package, DollarSign, Hash, Building2, AlertTriangle, FileText, Barcode, TrendingUp, User, ImagePlus, Upload } from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';
import { Product, ProductListItem } from '@/types/inventory';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Category } from '@/types/inventory';
import { productsService } from '@/api/services/products.service';
import { categoriesService } from '@/api/services/categories.service';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

interface EditProductModalProps {
  isOpen: boolean;
  product: Product | ProductListItem | null;
  categoryId?: string;
  categories: Category[];
  onClose: () => void;
  onSubmit: (product: Product) => void;
}

export default function EditProductModal({ isOpen, product, categoryId, categories, onClose, onSubmit }: EditProductModalProps) {
  const { user } = useAuth();
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    quantityInStock: '',
    categoryId: '',
    supplier: '',
    lowStockThreshold: '',
    image: null as File | null,
    imageUrl: '',
  });
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [isLoading, setIsLoading] = useState(true); // 1. Added loading state

  // Load categories once or when product changes
  // Load data when product changes
  const loadData = useCallback(() => {
    if (product) {
      // Initialize form data
      const catId = categoryId || product.categoryId || '';
      setFormData({
        name: product.name,
        description: product.description,
        price: product.price?.toString() ?? '',
        quantityInStock: product.currentStock?.toString() ?? '',
        categoryId: catId,
        supplier: product.supplier || '',
        lowStockThreshold: product.lowStockThreshold?.toString() ?? '',
        image: null,
        imageUrl: (product as Product).imageUrl || '',
      });
      setImagePreview((product as Product).imageUrl || null);

      // Find and set the selected category object from the provided list
      const activeCategory = categories.find((c: Category) => c.id.toString() === catId.toString());
      setSelectedCategory(activeCategory || null);
      console.log("Selected category:", activeCategory);
    }
  }, [product, categories]);

  useEffect(() => {
    if (isOpen && product) {
      loadData();
    }
  }, [isOpen, product, loadData]);

  // Update selectedCategory details whenever categoryId changes in the dropdown
  useEffect(() => {
    const category = categories.find(c => c.id === formData.categoryId);
    setSelectedCategory(category || null);
  }, [formData.categoryId, categories]);

  const validateForm = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.name.trim()) newErrors.name = 'Product name is required';
    if (!formData.price || parseFloat(formData.price) <= 0) newErrors.price = 'Valid price is required';
    if (!formData.categoryId) newErrors.categoryId = 'Category is required';
    if (!formData.supplier.trim()) newErrors.supplier = 'Supplier is required';
    if (!formData.lowStockThreshold || parseInt(formData.lowStockThreshold) < 0) newErrors.lowStockThreshold = 'Valid threshold is required';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!product || !validateForm()) return;

    setIsSubmitting(true);
    try {
      await productsService.update(product.id, {
        id: product.id,
        name: formData.name.trim(),
        description: formData.description.trim(),
        price: parseFloat(formData.price),
        categoryId: formData.categoryId,
        supplier: formData.supplier.trim(),
        lowStockAlertThreshold: parseInt(formData.lowStockThreshold),
        image: formData.image || undefined,
      });

      const updatedProduct: Product = {
        ...product,
        name: formData.name.trim(),
        description: formData.description.trim(),
        price: parseFloat(formData.price),
        currentStock: parseInt(formData.quantityInStock),
        stockQuantity: parseInt(formData.quantityInStock),
        categoryId: formData.categoryId,
        supplier: formData.supplier.trim(),
        lowStockThreshold: parseInt(formData.lowStockThreshold),
        imageUrl: imagePreview || (product as Product).imageUrl,
        lastUpdatedAt: new Date().toISOString(),
      };

      toast.success("Product updated successfully");
      onSubmit(updatedProduct);
      onClose();
    } catch (error: any) {
      toast.error(error.message || "Failed to update product");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    setErrors({});
    onClose();
  };

  if (!isOpen || !product) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-background/80 backdrop-blur-sm" onClick={handleClose} />

      <div className="relative w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto bg-card border border-border rounded-2xl shadow-2xl animate-fade-in">
        <div className="sticky top-0 z-10 flex items-center justify-between p-6 border-b border-border bg-card">
          <div>
            <h2 className="text-xl font-semibold text-foreground">Edit Product</h2>
            <p className="text-sm text-muted-foreground mt-1">Update product information</p>
          </div>
          <button onClick={handleClose} className="p-2 rounded-lg hover:bg-secondary transition-colors">
            <X className="w-5 h-5 text-muted-foreground" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-6">
          {/* SKU */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <Barcode className="w-4 h-4" /> SKU
            </Label>
            <Input value={product.sku || ''} disabled className="bg-muted border-border font-mono opacity-70" />
          </div>

          {/* Image Upload */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <ImagePlus className="w-4 h-4" />
              Product Image
            </Label>
            <div className="flex flex-col items-center justify-center border-2 border-dashed border-border rounded-xl p-4 bg-secondary/30 hover:bg-secondary/50 transition-colors cursor-pointer relative overflow-hidden group">
              {imagePreview ? (
                <div className="relative w-full aspect-video rounded-lg overflow-hidden">
                  <img src={imagePreview} alt="Preview" className="w-full h-full object-cover" />
                  <div className="absolute inset-0 bg-black/40 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                    <Button
                      type="button"
                      variant="destructive"
                      size="sm"
                      onClick={(e) => {
                        e.stopPropagation();
                        setFormData({ ...formData, image: null });
                        setImagePreview(null);
                      }}
                    >
                      Remove
                    </Button>
                  </div>
                </div>
              ) : (
                <div className="flex flex-col items-center py-4 text-muted-foreground">
                  <Upload className="w-8 h-8 mb-2" />
                  <p className="text-sm">Click or drag to upload new image</p>
                  <p className="text-xs mt-1">PNG, JPG or WebP (max 5MB)</p>
                </div>
              )}
              <input
                type="file"
                accept="image/*"
                className="absolute inset-0 opacity-0 cursor-pointer"
                onChange={(e) => {
                  const file = e.target.files?.[0];
                  if (file) {
                    setFormData({ ...formData, image: file });
                    const reader = new FileReader();
                    reader.onloadend = () => {
                      setImagePreview(reader.result as string);
                    };
                    reader.readAsDataURL(file);
                  }
                }}
              />
            </div>
          </div>

          {/* Name */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <Package className="w-4 h-4" /> Product Name
            </Label>
            <Input
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="bg-secondary border-border"
            />
            {errors.name && <p className="text-sm text-destructive">{errors.name}</p>}
          </div>

          {/* Category Selection */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium">Category</Label>
            <Select
              value={formData.categoryId}
              onValueChange={(value) => setFormData({ ...formData, categoryId: value })}
            >
              <SelectTrigger className="w-full bg-secondary border-border">
                <SelectValue placeholder="Select a category" />
              </SelectTrigger>
              <SelectContent>
                {categories.map((category) => (
                  <SelectItem key={category.id} value={category.id}>
                    {category.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.categoryId && <p className="text-sm text-destructive">{errors.categoryId}</p>}
            {selectedCategory && (
              <p className="text-xs text-muted-foreground animate-in fade-in slide-in-from-top-1">
                {selectedCategory.description}
              </p>
            )}
          </div>

          {/* Price and Stock */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label className="flex items-center gap-2"> <DollarSign className="w-4 h-4" /> Price </Label>
              <Input
                type="number"
                value={formData.price}
                onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                className="bg-secondary border-border"
              />
            </div>
            <div className="space-y-2">
              <Label className="flex items-center gap-2"> <Hash className="w-4 h-4" /> Stock </Label>
              <Input value={product.currentStock || 0} disabled className="bg-muted opacity-70" />
            </div>
          </div>

          {/* Supplier & Threshold */}
          <div className="space-y-2">
            <Label className="flex items-center gap-2"> <Building2 className="w-4 h-4" /> Supplier </Label>
            <Input
              value={formData.supplier}
              onChange={(e) => setFormData({ ...formData, supplier: e.target.value })}
              className="bg-secondary border-border"
            />
          </div>

          <div className="space-y-2">
            <Label className="flex items-center gap-2"> <AlertTriangle className="w-4 h-4" /> Low Stock Threshold </Label>
            <Input
              type="number"
              value={formData.lowStockThreshold}
              onChange={(e) => setFormData({ ...formData, lowStockThreshold: e.target.value })}
              className="bg-secondary border-border"
            />
          </div>

          {/* User Info */}
          <div className="p-4 bg-secondary/50 border border-border rounded-xl">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary font-semibold text-sm border border-primary/20">
                {user?.fullName?.slice(0, 2).toUpperCase() || user?.userName?.slice(0, 2).toUpperCase() || '??'}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm font-semibold text-foreground truncate">{user?.fullName || user?.userName || 'Unknown User'}</p>
                <div className="flex items-center gap-2 text-xs text-muted-foreground">
                  <User className="w-3 h-3" />
                  <span>Updating this product</span>
                </div>
              </div>
            </div>
          </div>

          <div className="flex gap-3 pt-2">
            <Button type="button" variant="outline" onClick={handleClose} className="flex-1">Cancel</Button>
            <Button type="submit" disabled={isSubmitting} className="flex-1">
              {isSubmitting ? 'Saving...' : 'Save Changes'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}