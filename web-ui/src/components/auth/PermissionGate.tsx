import { ReactNode } from 'react';
import { usePermissions } from '@/hooks/usePermissions';
import { ActionPermission } from '@/lib/permissions';

interface PermissionGateProps {
  children: ReactNode;
  action?: ActionPermission;
  page?: string;
  fallback?: ReactNode;
}

/**
 * Component that conditionally renders children based on user permissions.
 * Use `action` for specific actions like 'products.create'
 * Use `page` for page-level access like 'users'
 */
export function PermissionGate({ 
  children, 
  action, 
  page, 
  fallback = null 
}: PermissionGateProps) {
  const { canAccessPage, canPerformAction } = usePermissions();

  // Check page access if provided
  if (page && !canAccessPage(page)) {
    return <>{fallback}</>;
  }

  // Check action permission if provided
  if (action && !canPerformAction(action)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}

/**
 * Higher-order component for permission-based rendering
 */
export function withPermission<P extends object>(
  WrappedComponent: React.ComponentType<P>,
  action?: ActionPermission,
  page?: string
) {
  return function WithPermissionComponent(props: P) {
    return (
      <PermissionGate action={action} page={page}>
        <WrappedComponent {...props} />
      </PermissionGate>
    );
  };
}
