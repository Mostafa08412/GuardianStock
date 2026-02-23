import { useMemo } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { UserRole } from '@/types/inventory';
import { 
  hasPageAccess, 
  hasActionPermission, 
  getAccessiblePages,
  ActionPermission,
  ROLE_INFO,
} from '@/lib/permissions';

export function usePermissions() {
  const { user } = useAuth();
  
  // Get role from user or default to staff (most restrictive)
  const role: UserRole = useMemo(() => {
    if (!user?.roles?.length) return 'staff';
    // Map backend roles to our app roles
    const roleMap: Record<string, UserRole> = {
      'Admin': 'admin',
      'Manager': 'manager',
      'Staff': 'staff',
      'admin': 'admin',
      'manager': 'manager',
      'staff': 'staff',
    };
    return roleMap[user.roles[0]] || 'staff';
  }, [user]);

  const canAccessPage = (page: string): boolean => {
    return hasPageAccess(page, role);
  };

  const canPerformAction = (action: ActionPermission): boolean => {
    return hasActionPermission(action, role);
  };

  const accessiblePages = useMemo(() => getAccessiblePages(role), [role]);

  const roleInfo = useMemo(() => ROLE_INFO[role], [role]);

  return {
    role,
    canAccessPage,
    canPerformAction,
    accessiblePages,
    roleInfo,
    isAdmin: role === 'admin',
    isManager: role === 'manager',
    isStaff: role === 'staff',
  };
}
