import { useState } from 'react';
import { X, Package, DollarSign, Hash, Building2, AlertTriangle, FileText, Barcode, User, ImagePlus, Upload } from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';
import { Product, Category } from '@/types/inventory';
import { productsService } from '@/api/services/products.service';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { toast } from 'sonner';

interface AddProductModalProps {
  isOpen: boolean;
  categories: Category[];
  onClose: () => void;
  onSubmit: (product: Product) => void;
}


export default function AddProductModal({ isOpen, categories, onClose, onSubmit }: AddProductModalProps) {
  const { user } = useAuth();
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    quantityInStock: '',
    categoryId: '',
    supplier: '',
    lowStockThreshold: '10',
    image: null as File | null,
  });
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) newErrors.name = 'Product name is required';
    if (!formData.price || parseFloat(formData.price) <= 0) newErrors.price = 'Valid price is required';
    if (!formData.quantityInStock || parseInt(formData.quantityInStock) < 0) newErrors.quantityInStock = 'Valid quantity is required';
    if (!formData.categoryId) newErrors.categoryId = 'Category is required';
    if (!formData.supplier.trim()) newErrors.supplier = 'Supplier is required';
    if (!formData.lowStockThreshold || parseInt(formData.lowStockThreshold) < 0) newErrors.lowStockThreshold = 'Valid threshold is required';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    setIsSubmitting(true);

    try {
      var result = await productsService.create({
        name: formData.name.trim(),
        description: formData.description.trim(),
        price: parseFloat(formData.price),
        supplier: formData.supplier.trim(),
        categoryId: formData.categoryId,
        initialQuantity: parseInt(formData.quantityInStock),
        lowStockThreshold: parseInt(formData.lowStockThreshold),
        image: formData.image || undefined,
      });

      console.log("result:", result);
      const newProduct: Product = {
        id: result.id,
        sku: result.sku,
        name: result.name,
        description: result.description,
        price: result.price,
        stockQuantity: result.currentStock,
        categoryId: result.categoryId,
        categoryName: result.categoryName,
        supplier: result.supplier,
        lowStockThreshold: result.lowStockThreshold
      };

      onSubmit(newProduct);
      toast.success("Product added successfully");
      setIsSubmitting(false);
      handleClose();
    } catch (error) {
      setIsSubmitting(false);
      toast.error(error.message);

    }
  };

  const handleClose = () => {
    setFormData({
      name: '',
      description: '',
      price: '',
      quantityInStock: '',
      categoryId: '',
      supplier: '',
      lowStockThreshold: '10',
      image: null,
    });
    setImagePreview(null);
    setErrors({});
    onClose();
  };

  if (!isOpen) return null;

  const selectedCategory = categories.find(c => c.id === formData.categoryId);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-background/80 backdrop-blur-sm"
        onClick={handleClose}
      />

      {/* Modal */}
      <div className="relative w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto bg-card border border-border rounded-2xl shadow-2xl animate-fade-in">
        {/* Header */}
        <div className="sticky top-0 z-10 flex items-center justify-between p-6 border-b border-border bg-card">
          <div>
            <h2 className="text-xl font-semibold text-foreground">Add New Product</h2>
            <p className="text-sm text-muted-foreground mt-1">Create a new product in your inventory</p>
          </div>
          <button
            onClick={handleClose}
            className="p-2 rounded-lg hover:bg-secondary transition-colors"
          >
            <X className="w-5 h-5 text-muted-foreground" />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-6">
          {/* SKU Info */}
          <div className="p-3 bg-muted/50 border border-border rounded-lg">
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Barcode className="w-4 h-4" />
              <span>SKU will be auto-generated after creation</span>
            </div>
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
                  <p className="text-sm">Click or drag to upload image</p>
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

          {/* Product Name */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <Package className="w-4 h-4" />
              Product Name
            </Label>
            <Input
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              placeholder="Enter product name"
              className="bg-secondary border-border"
            />
            {errors.name && <p className="text-sm text-destructive">{errors.name}</p>}
          </div>

          {/* Description */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <FileText className="w-4 h-4" />
              Description
            </Label>
            <Textarea
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              placeholder="Enter product description"
              className="bg-secondary border-border min-h-[100px]"
            />
          </div>

          {/* Price and Quantity Row */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label className="text-foreground font-medium flex items-center gap-2">
                <DollarSign className="w-4 h-4" />
                Price
              </Label>
              <div className="relative">
                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">$</span>
                <Input
                  type="number"
                  step="0.01"
                  min="0"
                  value={formData.price}
                  onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                  placeholder="0.00"
                  className="pl-8 bg-secondary border-border"
                />
              </div>
              {errors.price && <p className="text-sm text-destructive">{errors.price}</p>}
            </div>

            <div className="space-y-2">
              <Label className="text-foreground font-medium flex items-center gap-2">
                <Hash className="w-4 h-4" />
                Initial Stock Quantity
              </Label>
              <Input
                type="number"
                min="0"
                value={formData.quantityInStock}
                onChange={(e) => setFormData({ ...formData, quantityInStock: e.target.value })}
                placeholder="0"
                className="bg-secondary border-border"
              />
              {errors.quantityInStock && <p className="text-sm text-destructive">{errors.quantityInStock}</p>}
            </div>
          </div>

          {/* Category */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium">Category</Label>
            <Select value={formData.categoryId} onValueChange={(value) => setFormData({ ...formData, categoryId: value })}>
              <SelectTrigger className="w-full bg-secondary border-border">
                <SelectValue placeholder="Select a category" />
              </SelectTrigger>
              <SelectContent>
                {categories.map(category => (
                  <SelectItem key={category.id} value={category.id}>
                    {category.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.categoryId && <p className="text-sm text-destructive">{errors.categoryId}</p>}
            {selectedCategory && (
              <p className="text-xs text-muted-foreground">{selectedCategory.description}</p>
            )}
          </div>

          {/* Supplier */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <Building2 className="w-4 h-4" />
              Supplier
            </Label>
            <Input
              value={formData.supplier}
              onChange={(e) => setFormData({ ...formData, supplier: e.target.value })}
              placeholder="Enter supplier name"
              className="bg-secondary border-border"
            />
            {errors.supplier && <p className="text-sm text-destructive">{errors.supplier}</p>}
          </div>

          {/* Low Stock Threshold */}
          <div className="space-y-2">
            <Label className="text-foreground font-medium flex items-center gap-2">
              <AlertTriangle className="w-4 h-4" />
              Low Stock Threshold
            </Label>
            <Input
              type="number"
              min="0"
              value={formData.lowStockThreshold}
              onChange={(e) => setFormData({ ...formData, lowStockThreshold: e.target.value })}
              placeholder="10"
              className="bg-secondary border-border"
            />
            {errors.lowStockThreshold && <p className="text-sm text-destructive">{errors.lowStockThreshold}</p>}
            <p className="text-xs text-muted-foreground">
              Alert will trigger when stock falls below this number
            </p>
          </div>

          {/* Stock Status Preview */}
          {formData.quantityInStock && formData.lowStockThreshold && (
            <div className="p-4 bg-secondary rounded-xl space-y-3 animate-fade-in">
              <h4 className="font-medium text-foreground">Stock Status Preview</h4>
              <div className="flex items-center gap-4">
                <div className="flex-1">
                  <div className="flex justify-between text-sm mb-1">
                    <span className="text-muted-foreground">Initial Stock</span>
                    <span className="text-foreground font-medium">{formData.quantityInStock || 0} units</span>
                  </div>
                  <div className="h-2 bg-muted rounded-full overflow-hidden">
                    <div
                      className={`h-full transition-all ${parseInt(formData.quantityInStock) <= parseInt(formData.lowStockThreshold) * 0.5
                        ? 'bg-destructive'
                        : parseInt(formData.quantityInStock) <= parseInt(formData.lowStockThreshold)
                          ? 'bg-amber-500'
                          : 'bg-emerald-500'
                        }`}
                      style={{
                        width: `${Math.min(100, (parseInt(formData.quantityInStock) || 0) / (parseInt(formData.lowStockThreshold) || 1) * 50)}%`
                      }}
                    />
                  </div>
                </div>
                <div className={`px-3 py-1 rounded-full text-xs font-medium ${parseInt(formData.quantityInStock) <= parseInt(formData.lowStockThreshold) * 0.5
                  ? 'bg-destructive/20 text-destructive'
                  : parseInt(formData.quantityInStock) <= parseInt(formData.lowStockThreshold)
                    ? 'bg-amber-500/20 text-amber-500'
                    : 'bg-emerald-500/20 text-emerald-500'
                  }`}>
                  {parseInt(formData.quantityInStock) <= parseInt(formData.lowStockThreshold) * 0.5
                    ? 'Critical'
                    : parseInt(formData.quantityInStock) <= parseInt(formData.lowStockThreshold)
                      ? 'Low Stock'
                      : 'In Stock'}
                </div>
              </div>
            </div>
          )}

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
                  <span>Creating this product</span>
                </div>
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
              disabled={isSubmitting}
              className="flex-1"
            >
              {isSubmitting ? 'Creating...' : 'Create Product'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
