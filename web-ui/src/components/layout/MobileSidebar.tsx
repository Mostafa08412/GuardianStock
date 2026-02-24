import {
  LayoutDashboard,
  Package,
  ShoppingCart,
  AlertTriangle,
  BarChart3,
  Users,
  FolderOpen,
  Settings,
  LogOut,
  Menu,
  Layers
} from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';
import { usePermissions } from '@/hooks/usePermissions';
import { cn } from '@/lib/utils';
import { useState } from 'react';
import { useNavigate, NavLink } from 'react-router-dom';
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from '@/components/ui/sheet';

interface NavItem {
  icon: React.ElementType;
  label: string;
  path: string;
}

const allNavItems: NavItem[] = [
  { icon: LayoutDashboard, label: 'Dashboard', path: 'dashboard' },
  { icon: Package, label: 'Products', path: 'products' },
  { icon: ShoppingCart, label: 'Transactions', path: 'transactions' },
  { icon: Layers, label: 'Inventories', path: 'inventories' },
  { icon: AlertTriangle, label: 'Low Stock Alerts', path: 'alerts' },
  { icon: FolderOpen, label: 'Categories', path: 'categories' },
  { icon: Users, label: 'User Management', path: 'users' },
];

interface MobileSidebarProps {
}

export default function MobileSidebar({ }: MobileSidebarProps) {
  const { user, logout } = useAuth();
  const { canAccessPage, roleInfo } = usePermissions();
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);

  // Filter nav items based on user permissions
  const navItems = allNavItems.filter(item => canAccessPage(item.path));

  const handleLogout = async () => {
    await logout();
    navigate('/auth');
    setOpen(false);
  };

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <button className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors lg:hidden">
          <Menu className="w-6 h-6" />
        </button>
      </SheetTrigger>
      <SheetContent side="left" className="w-[280px] p-0 bg-sidebar border-sidebar-border">
        <SheetHeader className="sr-only">
          <SheetTitle>Navigation Menu</SheetTitle>
        </SheetHeader>

        {/* Logo Section */}
        <div className="flex items-center gap-3 px-6 py-5 border-b border-sidebar-border">
          <div className="w-10 h-10 rounded-xl bg-primary/10 overflow-hidden flex items-center justify-center">
            <img
              src="https://github.com/Mostafa08412/insight-dash/blob/main/public/logo.png?raw=true"
              alt="GuardianStock Logo"
              className="w-full h-full object-cover"
            />
          </div>
          <div>
            <h1 className="font-bold text-foreground text-lg tracking-tight">GuardianStock</h1>
            <p className="text-xs text-muted-foreground">Inventory Guardian</p>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-3 py-4 space-y-1 overflow-y-auto scrollbar-thin">
          {navItems.map((item) => {
            const Icon = item.icon;

            return (
              <NavLink
                key={item.path}
                to={item.path === 'dashboard' ? '/dashboard' : `/${item.path}`}
                onClick={() => setOpen(false)}
                className={({ isActive }) => cn(
                  'nav-item w-full text-left',
                  isActive && 'active'
                )}
              >
                <Icon className="w-5 h-5" />
                <span>{item.label}</span>
              </NavLink>
            );
          })}
        </nav>

        {/* User Section */}
        <div className="px-3 py-4 border-t border-sidebar-border">
          {user && (
            <div className="flex items-center gap-3 px-4 py-2">
              <button
                onClick={() => {
                  navigate('/profile');
                  setOpen(false);
                }}
                className="flex items-center gap-3 flex-1 min-w-0 hover:opacity-80 transition-opacity text-left"
                title="View Profile"
              >
                <div className="w-9 h-9 rounded-full bg-primary/20 flex items-center justify-center text-primary font-medium text-sm shrink-0">
                  {(user.firstName?.[0] || '') + (user.lastName?.[0] || '')}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-foreground truncate">{user.firstName} {user.lastName}</p>
                  <p className="text-xs text-muted-foreground truncate">{roleInfo.label}</p>
                </div>
              </button>
              <button
                onClick={handleLogout}
                className="p-2 rounded-lg hover:bg-sidebar-accent text-muted-foreground hover:text-foreground transition-colors"
                title="Sign out"
              >
                <LogOut className="w-4 h-4" />
              </button>
            </div>
          )}
        </div>
      </SheetContent>
    </Sheet>
  );
}
