/**
 * ActionButtons Component
 * 
 * Reusable action button group for tables and cards.
 */

import { cn } from '@/lib/utils';
import { Eye, Edit2, Trash2, LucideIcon } from 'lucide-react';

export interface ActionButtonProps {
  icon: LucideIcon;
  onClick: (e: React.MouseEvent) => void;
  label?: string;
  variant?: 'default' | 'danger';
  className?: string;
}

export function ActionButton({ 
  icon: Icon, 
  onClick, 
  label,
  variant = 'default',
  className 
}: ActionButtonProps) {
  return (
    <button
      onClick={(e) => {
        e.stopPropagation();
        onClick(e);
      }}
      title={label}
      className={cn(
        'p-2 rounded-lg transition-colors',
        variant === 'default' 
          ? 'hover:bg-secondary text-muted-foreground hover:text-foreground'
          : 'hover:bg-destructive/20 text-muted-foreground hover:text-destructive',
        className
      )}
    >
      <Icon className="w-4 h-4" />
    </button>
  );
}

export interface ActionButtonsProps {
  onView?: (e: React.MouseEvent) => void;
  onEdit?: (e: React.MouseEvent) => void;
  onDelete?: (e: React.MouseEvent) => void;
  className?: string;
}

export function ActionButtons({ onView, onEdit, onDelete, className }: ActionButtonsProps) {
  return (
    <div className={cn('flex items-center gap-1', className)}>
      {onView && (
        <ActionButton icon={Eye} onClick={onView} label="View" />
      )}
      {onEdit && (
        <ActionButton icon={Edit2} onClick={onEdit} label="Edit" />
      )}
      {onDelete && (
        <ActionButton icon={Trash2} onClick={onDelete} label="Delete" variant="danger" />
      )}
    </div>
  );
}
