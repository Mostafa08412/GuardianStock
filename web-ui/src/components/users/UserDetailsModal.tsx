import { useState, useEffect } from 'react';
import { X, Shield, UserCheck, User as UserIcon, Mail, Calendar, Clock, Loader2, Power } from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { User } from '@/types/inventory';
import { format } from 'date-fns';
import { cn } from '@/lib/utils';
import { usersService } from '@/api/services/users.service';
import { useToast } from '@/hooks/use-toast';

interface UserDetailsModalProps {
  isOpen: boolean;
  userId: string | null;
  onClose: () => void;
  onEdit: () => void;
  onStatusChange: () => void;
}

const roleIcons = {
  admin: Shield,
  manager: UserCheck,
  staff: UserIcon,
};

const roleColors = {
  admin: 'bg-destructive/20 text-destructive',
  manager: 'bg-warning/20 text-warning',
  staff: 'bg-info/20 text-info',
};

export default function UserDetailsModal({ isOpen, userId, onClose, onEdit, onStatusChange }: UserDetailsModalProps) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(false);
  const [togglingStatus, setTogglingStatus] = useState(false);
  const { toast } = useToast();

  useEffect(() => {
    if (isOpen && userId) {
      fetchUserDetails();
    }
  }, [isOpen, userId]);

  const fetchUserDetails = async () => {
    if (!userId) return;
    setLoading(true);
    try {
      const details = await usersService.getById(userId);
      setUser(details);
    } catch (error) {
      console.error('Failed to fetch user details:', error);
      toast({ title: 'Error', description: 'Failed to load user details.', variant: 'destructive' });
      onClose();
    } finally {
      setLoading(false);
    }
  };

  const handleToggleStatus = async () => {
    if (!user) return;
    setTogglingStatus(true);
    try {
      if (user.status === 'active') {
        await usersService.deactivate(user.id);
      } else {
        await usersService.activate(user.id);
      }
      toast({
        title: user.status === 'active' ? 'Account Deactivated' : 'Account Activated',
        description: `User ${user.name} status has been updated.`
      });
      fetchUserDetails();
      onStatusChange();
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error.message || 'Failed to update user status.',
        variant: 'destructive'
      });
    } finally {
      setTogglingStatus(false);
    }
  };

  if (!isOpen) return null;

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[450px] p-0 overflow-hidden bg-card border-border">
        <DialogHeader className="p-6 bg-secondary/30 border-b border-border">
          <div className="flex items-center justify-between">
            <DialogTitle className="text-xl font-bold text-foreground">User Profile</DialogTitle>
          </div>
        </DialogHeader>

        <div className="p-6">
          {loading ? (
            <div className="flex flex-col items-center justify-center py-12 space-y-4">
              <Loader2 className="w-8 h-8 animate-spin text-primary" />
              <p className="text-sm text-muted-foreground font-medium">Loading user details...</p>
            </div>
          ) : user ? (
            <>
              {/* User Avatar & Name */}
              <div className="flex items-center gap-4 mb-8">
                <div className="w-16 h-16 rounded-full bg-primary/20 ring-4 ring-primary/10 flex items-center justify-center text-primary text-2xl font-bold">
                  {user.avatar}
                </div>
                <div>
                  <h3 className="text-xl font-bold text-foreground leading-tight">{user.name}</h3>
                  <div className="flex items-center gap-2 mt-1.5">
                    <span className={cn(
                      'text-[10px] uppercase tracking-wider font-bold px-2 py-0.5 rounded-full',
                      user.status === 'active' ? 'bg-success/20 text-success' : 'bg-muted text-muted-foreground border border-border'
                    )}>
                      {user.status === 'active' ? 'Active' : 'Inactive'}
                    </span>
                  </div>
                </div>
              </div>

              {/* User Info Grid */}
              <div className="grid gap-3">
                <div className="flex items-center gap-3 p-3 bg-secondary/20 rounded-xl border border-border/50">
                  <div className="w-9 h-9 rounded-lg bg-background flex items-center justify-center text-muted-foreground">
                    <Mail className="w-4 h-4" />
                  </div>
                  <div>
                    <p className="text-[10px] uppercase tracking-wider text-muted-foreground font-semibold">Email Address</p>
                    <p className="text-sm font-medium text-foreground">{user.email}</p>
                  </div>
                </div>

                <div className="flex items-center gap-3 p-3 bg-secondary/20 rounded-xl border border-border/50">
                  <div className={cn('w-9 h-9 rounded-lg flex items-center justify-center', roleColors[user.role])}>
                    {user.role === 'admin' && <Shield className="w-4 h-4" />}
                    {user.role === 'manager' && <UserCheck className="w-4 h-4" />}
                    {user.role === 'staff' && <UserIcon className="w-4 h-4" />}
                  </div>
                  <div>
                    <p className="text-[10px] uppercase tracking-wider text-muted-foreground font-semibold">Permission Level</p>
                    <p className="text-sm font-medium text-foreground capitalize">{user.role}</p>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-3">
                  <div className="flex items-center gap-3 p-3 bg-secondary/20 rounded-xl border border-border/50">
                    <div className="w-9 h-9 rounded-lg bg-background flex items-center justify-center text-muted-foreground">
                      <Calendar className="w-4 h-4" />
                    </div>
                    <div>
                      <p className="text-[10px] uppercase tracking-wider text-muted-foreground font-semibold">Created</p>
                      <p className="text-sm font-medium text-foreground">{format(new Date(user.createdAt), 'MMM d, yyyy')}</p>
                    </div>
                  </div>

                  <div className="flex items-center gap-3 p-3 bg-secondary/20 rounded-xl border border-border/50">
                    <div className="w-9 h-9 rounded-lg bg-background flex items-center justify-center text-muted-foreground">
                      <Clock className="w-4 h-4" />
                    </div>
                    <div>
                      <p className="text-[10px] uppercase tracking-wider text-muted-foreground font-semibold">Last Login</p>
                      <p className="text-sm font-medium text-foreground">
                        {user.lastLogin ? format(new Date(user.lastLogin), 'MMM d, hh:mm a') : 'Never'}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Actions */}
              <div className="flex flex-col gap-2 mt-8">
                <div className="flex gap-2">
                  <Button type="button" variant="outline" onClick={onEdit} className="flex-1">
                    Edit Profile
                  </Button>
                  <Button
                    type="button"
                    variant={user.status === 'active' ? 'destructive' : 'default'}
                    onClick={handleToggleStatus}
                    className="flex-1"
                    disabled={togglingStatus}
                  >
                    {togglingStatus ? (
                      <Loader2 className="w-4 h-4 animate-spin" />
                    ) : (
                      <>
                        <Power className="w-4 h-4 mr-2" />
                        {user.status === 'active' ? 'Deactivate' : 'Activate'}
                      </>
                    )}
                  </Button>
                </div>
                <Button type="button" variant="ghost" onClick={onClose} className="w-full text-muted-foreground font-normal">
                  Close Details
                </Button>
              </div>
            </>
          ) : (
            <div className="text-center py-8">
              <p className="text-sm text-destructive font-medium">Failed to load user information.</p>
              <Button variant="link" onClick={fetchUserDetails} className="mt-2">Try Again</Button>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
