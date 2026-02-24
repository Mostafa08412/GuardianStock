import { useState, useEffect } from 'react';
import { X, Loader2, UserCog } from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { User, UserRole, UserStatus } from '@/types/inventory';
import { useToast } from '@/hooks/use-toast';
import { usersService } from '@/api/services/users.service';

interface EditUserModalProps {
  isOpen: boolean;
  user: User | null;
  currentUserId: string;
  onClose: () => void;
  onSave: () => void;
}

export default function EditUserModal({ isOpen, user, currentUserId, onClose, onSave }: EditUserModalProps) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [role, setRole] = useState<UserRole>('staff');
  const [status, setStatus] = useState<UserStatus>('active');
  const [errors, setErrors] = useState<{ firstName?: string; lastName?: string; email?: string }>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { toast } = useToast();

  const isCurrentUser = user?.id === currentUserId;

  useEffect(() => {
    if (user && isOpen) {
      const name = user.name || '';
      const parts = name.split(' ');
      setFirstName(parts[0] || '');
      setLastName(parts.slice(1).join(' ') || '');
      setEmail(user.email || '');
      setRole(user.role);
      setStatus(user.status);
      setErrors({});
    }
  }, [user, isOpen]);

  const handleClose = () => {
    setErrors({});
    onClose();
  };

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!firstName?.trim()) newErrors.firstName = 'First name is required';
    // Only require last name if there was one before, or if we want to enforce it
    // For now, let's keep it required but add safety
    if (!lastName?.trim()) newErrors.lastName = 'Last name is required';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;

    if (!validate()) {
      toast({
        title: 'Validation Error',
        description: 'Please check the required fields.',
        variant: 'destructive',
      });
      return;
    }

    setIsSubmitting(true);
    try {
      // 1. Update basic info
      await usersService.update({
        userId: user.id,
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        role: role,
      });

      // 2. Update status if changed
      if (status !== user.status) {
        if (status === 'active') {
          await usersService.activate(user.id);
        } else {
          await usersService.deactivate(user.id);
        }
      }

      toast({
        title: 'User Updated',
        description: `${firstName} ${lastName} has been updated successfully.`
      });
      onSave();
      handleClose();
    } catch (error: any) {
      console.error('Failed to update user:', error);

      const validationErrors = error.validationErrors || (error.response?.data?.validationErrors);

      if (validationErrors) {
        setErrors(prev => ({ ...prev, ...validationErrors }));
        toast({
          title: 'Validation Error',
          description: 'The server rejected some values. Please check the fields.',
          variant: 'destructive'
        });
      } else {
        toast({
          title: 'Update Failed',
          description: error.message || 'Failed to update user. Please try again.',
          variant: 'destructive'
        });
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="sm:max-w-[500px] p-0 overflow-hidden bg-card border-border">
        <DialogHeader className="p-6 bg-secondary/30 border-b border-border">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-primary/20 flex items-center justify-center text-primary">
              <UserCog className="w-5 h-5" />
            </div>
            <div>
              <DialogTitle className="text-xl font-bold text-foreground">Edit User</DialogTitle>
              <DialogDescription className="text-muted-foreground">
                Update user profile and permissions.
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="edit-firstName">First Name</Label>
              <Input
                id="edit-firstName"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                className={errors.firstName ? 'border-destructive' : ''}
              />
              {errors.firstName && <p className="text-xs text-destructive">{errors.firstName}</p>}
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-lastName">Last Name</Label>
              <Input
                id="edit-lastName"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                className={errors.lastName ? 'border-destructive' : ''}
              />
              {errors.lastName && <p className="text-xs text-destructive">{errors.lastName}</p>}
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="edit-email">Email Address</Label>
            <Input
              id="edit-email"
              type="email"
              value={email}
              readOnly={true} // Usually email is not editable after creation in many systems
              className="bg-secondary/50"
            />
            <p className="text-[10px] text-muted-foreground italic">Email address cannot be changed.</p>
          </div>

          <div className="space-y-2">
            <Label htmlFor="edit-role">User Role</Label>
            <Select
              value={role}
              onValueChange={(v) => setRole(v as UserRole)}
              disabled={isCurrentUser}
            >
              <SelectTrigger className="bg-background border-border">
                <SelectValue placeholder="Select role" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="admin">Admin</SelectItem>
                <SelectItem value="manager">Manager</SelectItem>
                <SelectItem value="staff">Staff</SelectItem>
              </SelectContent>
            </Select>
            {isCurrentUser && (
              <p className="text-xs text-warning mt-1 italic">You cannot change your own role.</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="edit-status">Account Status</Label>
            <Select
              value={status}
              onValueChange={(v) => setStatus(v as UserStatus)}
              disabled={isCurrentUser}
            >
              <SelectTrigger className="bg-background border-border">
                <SelectValue placeholder="Select status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="active">Active</SelectItem>
                <SelectItem value="inactive">Inactive</SelectItem>
              </SelectContent>
            </Select>
            {isCurrentUser && (
              <p className="text-xs text-warning mt-1 italic">You cannot deactivate your own account.</p>
            )}
          </div>

          <div className="flex gap-3 mt-6 pt-4 border-t border-border">
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              className="flex-1"
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button type="submit" className="flex-1" disabled={isSubmitting}>
              {isSubmitting ? (
                <>
                  <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                  Saving...
                </>
              ) : (
                'Save Changes'
              )}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
