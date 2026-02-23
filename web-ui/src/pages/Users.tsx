import { Shield, UserCheck, User as UserIcon } from 'lucide-react';
import { usePermissions } from '@/hooks/usePermissions';
import { AccessDenied } from '@/components/auth/AccessDenied';
import UsersTable from '@/components/users/UsersTable';

export default function Users() {
  const { canAccessPage } = usePermissions();

  // Check page access
  if (!canAccessPage('users')) {
    return <AccessDenied message="Only administrators can access User Management." />;
  }

  return (
    <div className="space-y-6">
      <UsersTable />

      {/* Role Permissions Info - kept in parent as it's informational content for the page, not strictly table data */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '200ms' }}>
          <div className="flex items-center gap-3 mb-4">
            <div className="w-10 h-10 rounded-lg bg-destructive/20 flex items-center justify-center">
              <Shield className="w-5 h-5 text-destructive" />
            </div>
            <h3 className="font-semibold text-foreground">Admin</h3>
          </div>
          <ul className="space-y-2 text-sm text-muted-foreground">
            <li>• Full system access</li>
            <li>• User management</li>
            <li>• All reports & analytics</li>
            <li>• Settings configuration</li>
          </ul>
        </div>

        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '250ms' }}>
          <div className="flex items-center gap-3 mb-4">
            <div className="w-10 h-10 rounded-lg bg-warning/20 flex items-center justify-center">
              <UserCheck className="w-5 h-5 text-warning" />
            </div>
            <h3 className="font-semibold text-foreground">Manager</h3>
          </div>
          <ul className="space-y-2 text-sm text-muted-foreground">
            <li>• Product management</li>
            <li>• Category management</li>
            <li>• View reports</li>
            <li>• Transaction oversight</li>
          </ul>
        </div>

        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '300ms' }}>
          <div className="flex items-center gap-3 mb-4">
            <div className="w-10 h-10 rounded-lg bg-info/20 flex items-center justify-center">
              <UserIcon className="w-5 h-5 text-info" />
            </div>
            <h3 className="font-semibold text-foreground">Staff</h3>
          </div>
          <ul className="space-y-2 text-sm text-muted-foreground">
            <li>• View products</li>
            <li>• Record transactions</li>
            <li>• View stock alerts</li>
            <li>• Limited dashboard access</li>
          </ul>
        </div>
      </div>
    </div>
  );
}
