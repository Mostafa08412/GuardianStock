import { useState } from 'react';
import { Loader2, AlertTriangle } from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { toast } from '@/hooks/use-toast';
import { User } from '@/types/inventory';
import { usersService } from '@/api/services/users.service';

interface DeleteUserDialogProps {
  isOpen: boolean;
  user: User | null;
  currentUserId: string;
  onClose: () => void;
  onDelete: () => void;
}

export default function DeleteUserDialog({ isOpen, user, currentUserId, onClose, onDelete }: DeleteUserDialogProps) {
  const [isDeleting, setIsDeleting] = useState(false);

  const isCurrentUser = user?.id === currentUserId;

  const handleDelete = async () => {
    if (!user) return;
    if (isCurrentUser) {
      toast({ title: 'Error', description: 'You cannot delete your own account.', variant: 'destructive' });
      return;
    }

    setIsDeleting(true);
    try {
      await usersService.delete(user.id);
      toast({ title: 'User Deleted', description: `${user.name} has been deleted successfully.` });
      onDelete();
      onClose();
    } catch (error: any) {
      console.error('Failed to delete user:', error);
      toast({
        title: 'Error',
        description: error.message || 'Failed to delete user.',
        variant: 'destructive'
      });
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[400px] p-0 overflow-hidden bg-card border-border">
        <DialogHeader className="p-6 bg-destructive/10 border-b border-destructive/20">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-destructive/20 flex items-center justify-center text-destructive">
              <AlertTriangle className="w-5 h-5" />
            </div>
            <DialogTitle className="text-xl font-bold text-foreground">Delete User</DialogTitle>
          </div>
        </DialogHeader>

        <div className="p-6">
          {isCurrentUser ? (
            <div className="space-y-4">
              <p className="text-sm text-muted-foreground">
                You cannot delete your own account. Please contact another administrator to perform this action.
              </p>
              <Button type="button" variant="outline" onClick={onClose} className="w-full">
                Close
              </Button>
            </div>
          ) : (
            <div className="space-y-6">
              <div className="space-y-2">
                <p className="text-sm text-foreground font-medium">
                  Are you sure you want to delete this user?
                </p>
                <div className="p-3 bg-secondary/50 rounded-lg border border-border">
                  <p className="text-sm font-bold text-foreground">{user?.name}</p>
                  <p className="text-xs text-muted-foreground">{user?.email}</p>
                </div>
              </div>

              <div className="p-3 bg-destructive/5 rounded-lg border border-destructive/10">
                <p className="text-xs text-destructive leading-relaxed">
                  <strong>Warning:</strong> This action cannot be undone. The user will be permanently removed from the system and lose all access.
                </p>
              </div>

              <div className="flex gap-3 pt-2">
                <Button
                  type="button"
                  variant="outline"
                  onClick={onClose}
                  className="flex-1"
                  disabled={isDeleting}
                >
                  Cancel
                </Button>
                <Button
                  type="button"
                  variant="destructive"
                  onClick={handleDelete}
                  disabled={isDeleting}
                  className="flex-1"
                >
                  {isDeleting ? (
                    <>
                      <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                      Deleting...
                    </>
                  ) : (
                    'Delete User'
                  )}
                </Button>
              </div>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
