import { LucideIcon } from 'lucide-react';
import { cn } from '@/lib/utils';

interface StatCardProps {
  title: string;
  value: string | number;
  change?: {
    value: number;
    positive: boolean;
  };
  icon: LucideIcon;
  variant?: 'default' | 'primary' | 'success' | 'warning' | 'danger';
  delay?: number;
}

export default function StatCard({ title, value, change, icon: Icon, variant = 'default', delay = 0 }: StatCardProps) {
  const variantStyles = {
    default: 'from-card to-card',
    primary: 'from-primary/10 to-primary/5',
    success: 'from-success/10 to-success/5',
    warning: 'from-warning/10 to-warning/5',
    danger: 'from-destructive/10 to-destructive/5',
  };

  const iconStyles = {
    default: 'bg-secondary text-muted-foreground',
    primary: 'bg-primary/20 text-primary',
    success: 'bg-success/20 text-success',
    warning: 'bg-warning/20 text-warning',
    danger: 'bg-destructive/20 text-destructive',
  };

  return (
    <div 
      className={cn(
        'stat-card bg-gradient-to-br border border-border/50 rounded-xl',
        variantStyles[variant]
      )}
      style={{ animationDelay: `${delay}ms` }}
    >
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm font-medium text-muted-foreground mb-1">{title}</p>
          <p className="text-3xl font-bold text-foreground tracking-tight">{value}</p>
          {change && (
            <p className={cn(
              'text-sm font-medium mt-2 flex items-center gap-1',
              change.positive ? 'text-success' : 'text-destructive'
            )}>
              <span>{change.positive ? '↑' : '↓'}</span>
              {Math.abs(change.value)}% from last month
            </p>
          )}
        </div>
        <div className={cn('w-12 h-12 rounded-xl flex items-center justify-center', iconStyles[variant])}>
          <Icon className="w-6 h-6" />
        </div>
      </div>
    </div>
  );
}
