/**
 * StatusBadge Component
 * 
 * Reusable badge for displaying status with consistent styling.
 */

import { cn } from '@/lib/utils';

export type StatusVariant = 'success' | 'warning' | 'danger' | 'info' | 'default';

export interface StatusBadgeProps {
  children: React.ReactNode;
  variant?: StatusVariant;
  className?: string;
}

const variantStyles: Record<StatusVariant, string> = {
  success: 'bg-success/20 text-success',
  warning: 'bg-warning/20 text-warning',
  danger: 'bg-destructive/20 text-destructive',
  info: 'bg-info/20 text-info',
  default: 'bg-muted text-muted-foreground',
};

export function StatusBadge({ children, variant = 'default', className }: StatusBadgeProps) {
  return (
    <span className={cn(
      'inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium',
      variantStyles[variant],
      className
    )}>
      {children}
    </span>
  );
}

/**
 * Helper function to get stock status
 */
export function getStockStatus(quantity: number, threshold: number): { label: string; variant: StatusVariant } {
  if (quantity <= threshold * 0.3) return { label: 'Critical', variant: 'danger' };
  if (quantity <= threshold) return { label: 'Low', variant: 'warning' };
  return { label: 'In Stock', variant: 'success' };
}

/**
 * Helper function to get user status
 */
export function getUserStatus(status: 'active' | 'inactive'): { label: string; variant: StatusVariant } {
  return status === 'active' 
    ? { label: 'Active', variant: 'success' }
    : { label: 'Inactive', variant: 'default' };
}

/**
 * Helper function to get alert severity
 */
export function getAlertSeverity(currentStock: number, threshold: number): { label: string; variant: StatusVariant } {
  const percentage = (currentStock / threshold) * 100;
  if (percentage < 30) return { label: 'Critical', variant: 'danger' };
  return { label: 'Low Stock', variant: 'warning' };
}
