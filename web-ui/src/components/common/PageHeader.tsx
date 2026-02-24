/**
 * PageHeader Component
 * 
 * Consistent page headers with title, description, and actions.
 */

import { cn } from '@/lib/utils';
import { LucideIcon } from 'lucide-react';

export interface PageHeaderProps {
  title: string;
  description?: string;
  icon?: LucideIcon;
  actions?: React.ReactNode;
  className?: string;
}

export function PageHeader({ title, description, icon: Icon, actions, className }: PageHeaderProps) {
  return (
    <div className={cn('flex flex-col sm:flex-row sm:items-center justify-between gap-4 animate-fade-in', className)}>
      <div className="flex items-center gap-3">
        {Icon && (
          <div className="w-10 h-10 rounded-lg bg-primary/20 flex items-center justify-center">
            <Icon className="w-5 h-5 text-primary" />
          </div>
        )}
        <div>
          <h2 className="text-xl font-bold text-foreground">{title}</h2>
          {description && <p className="text-sm text-muted-foreground">{description}</p>}
        </div>
      </div>
      {actions && <div className="flex items-center gap-2">{actions}</div>}
    </div>
  );
}
