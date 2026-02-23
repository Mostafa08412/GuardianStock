/**
 * DataCard Component
 * 
 * Reusable card for displaying data in mobile/tablet views.
 */

import { cn } from '@/lib/utils';

export interface DataCardProps {
  children: React.ReactNode;
  onClick?: () => void;
  className?: string;
  animationDelay?: number;
}

export function DataCard({ children, onClick, className, animationDelay = 0 }: DataCardProps) {
  return (
    <div
      onClick={onClick}
      className={cn(
        'bg-card border border-border rounded-xl p-4 animate-fade-in transition-colors',
        onClick && 'cursor-pointer hover:border-primary/50',
        className
      )}
      style={{ animationDelay: `${animationDelay}ms` }}
    >
      {children}
    </div>
  );
}

/**
 * DataCardHeader Component
 */
export interface DataCardHeaderProps {
  children: React.ReactNode;
  className?: string;
}

export function DataCardHeader({ children, className }: DataCardHeaderProps) {
  return (
    <div className={cn('flex items-start justify-between mb-3', className)}>
      {children}
    </div>
  );
}

/**
 * DataCardContent Component
 */
export interface DataCardContentProps {
  children: React.ReactNode;
  className?: string;
}

export function DataCardContent({ children, className }: DataCardContentProps) {
  return (
    <div className={cn('space-y-2', className)}>
      {children}
    </div>
  );
}

/**
 * DataCardFooter Component
 */
export interface DataCardFooterProps {
  children: React.ReactNode;
  className?: string;
}

export function DataCardFooter({ children, className }: DataCardFooterProps) {
  return (
    <div className={cn('flex items-center justify-between mt-3 pt-3 border-t border-border', className)}>
      {children}
    </div>
  );
}
