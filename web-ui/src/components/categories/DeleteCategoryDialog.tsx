import { useState } from 'react';
import { X, Loader2, AlertTriangle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { toast } from '@/hooks/use-toast';
import { CategoryListItem } from '@/types/inventory';

interface DeleteCategoryDialogProps {
  isOpen: boolean;
  category: CategoryListItem | null;
  onClose: () => void;
  onDelete: (categoryId: string) => Promise<void>;
}

export default function DeleteCategoryDialog({ isOpen, category, onClose, onDelete }: DeleteCategoryDialogProps) {
  const [isDeleting, setIsDeleting] = useState(false);

  if (!isOpen || !category) return null;

  const handleDelete = async () => {
    setIsDeleting(true);
    try {
      await onDelete(category.id);
      toast({ title: 'Category Deleted', description: `${category.name} has been deleted successfully.` });
      onClose();
    } catch (error: any) {
      const message = error.response?.data?.message || error.message || 'Failed to delete category.';
      toast({ title: 'Error', description: message, variant: 'destructive' });
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/50 backdrop-blur-sm" onClick={onClose} />
      <div className="relative bg-card border border-border rounded-xl shadow-xl w-full max-w-md mx-4 animate-fade-in">
        <div className="flex items-center justify-between p-6 border-b border-border">
          <h2 className="text-xl font-semibold text-foreground">Delete Category</h2>
          <button onClick={onClose} className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors">
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="p-6">
          <div className="flex items-start gap-4 mb-6">
            <div className="w-12 h-12 rounded-xl bg-destructive/20 flex items-center justify-center flex-shrink-0">
              <AlertTriangle className="w-6 h-6 text-destructive" />
            </div>
            <div>
              <h3 className="font-semibold text-foreground mb-1">Are you sure?</h3>
              <p className="text-sm text-muted-foreground">
                You are about to delete <span className="font-medium text-foreground">{category.name}</span>.
                This category has <span className="font-medium text-foreground">{category.productCount} products</span> associated with it.
              </p>
            </div>
          </div>

          <div className="bg-destructive/10 border border-destructive/20 rounded-lg p-4 mb-6">
            <p className="text-sm text-destructive">
              <strong>Warning:</strong> Products in this category will need to be reassigned to another category.
            </p>
          </div>

          <div className="flex gap-3">
            <Button type="button" variant="outline" onClick={onClose} className="flex-1">
              Cancel
            </Button>
            <Button type="button" variant="destructive" onClick={handleDelete} disabled={isDeleting} className="flex-1">
              {isDeleting ? <Loader2 className="w-4 h-4 animate-spin mr-2" /> : null}
              Delete Category
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
