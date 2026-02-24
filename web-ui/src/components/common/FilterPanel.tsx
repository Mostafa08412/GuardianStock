/**
 * FilterPanel Component
 * 
 * Reusable filter controls container with clear button.
 */

import { cn } from '@/lib/utils';
import { Filter, X } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';

export interface FilterPanelProps {
  children: React.ReactNode;
  activeFilterCount?: number;
  onClearAll?: () => void;
  className?: string;
  title?: string;
}

export function FilterPanel({ 
  children, 
  activeFilterCount = 0, 
  onClearAll,
  className,
  title = 'Filters',
}: FilterPanelProps) {
  return (
    <div className={cn('bg-card border border-border rounded-xl p-4', className)}>
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <Filter className="w-5 h-5 text-muted-foreground" />
          <h3 className="font-semibold text-foreground">{title}</h3>
          {activeFilterCount > 0 && (
            <Badge variant="secondary" className="ml-2">
              {activeFilterCount} active
            </Badge>
          )}
        </div>
        {activeFilterCount > 0 && onClearAll && (
          <Button 
            variant="ghost" 
            size="sm" 
            onClick={onClearAll} 
            className="text-muted-foreground hover:text-foreground"
          >
            <X className="w-4 h-4 mr-1" />
            Clear all
          </Button>
        )}
      </div>
      <div className="space-y-3">
        {children}
      </div>
    </div>
  );
}

/**
 * FilterRow Component
 * 
 * Horizontal row of filter controls.
 */
export interface FilterRowProps {
  children: React.ReactNode;
  className?: string;
}

export function FilterRow({ children, className }: FilterRowProps) {
  return (
    <div className={cn('flex flex-wrap gap-3', className)}>
      {children}
    </div>
  );
}
