/**
 * PaginationFooter Component
 * 
 * Reusable pagination controls with page size selector.
 */

import { cn } from '@/lib/utils';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';

export interface PaginationFooterProps {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
  pageSizeOptions?: number[];
  className?: string;
}

export function PaginationFooter({
  currentPage,
  pageSize,
  totalCount,
  totalPages,
  onPageChange,
  onPageSizeChange,
  pageSizeOptions = [5, 10, 20, 50],
  className,
}: PaginationFooterProps) {
  const startIndex = (currentPage - 1) * pageSize + 1;
  const endIndex = Math.min(currentPage * pageSize, totalCount);

  const getPageNumbers = (): (number | 'ellipsis')[] => {
    const pages: (number | 'ellipsis')[] = [];
    
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      if (currentPage <= 3) {
        pages.push(1, 2, 3, 4, 'ellipsis', totalPages);
      } else if (currentPage >= totalPages - 2) {
        pages.push(1, 'ellipsis', totalPages - 3, totalPages - 2, totalPages - 1, totalPages);
      } else {
        pages.push(1, 'ellipsis', currentPage - 1, currentPage, currentPage + 1, 'ellipsis', totalPages);
      }
    }
    
    return pages;
  };

  if (totalCount === 0) return null;

  return (
    <div className={cn(
      'bg-card border border-border rounded-xl px-6 py-4 flex flex-col sm:flex-row items-center justify-between gap-4',
      className
    )}>
      <div className="flex items-center gap-4">
        <p className="text-sm text-muted-foreground">
          Showing <span className="font-medium text-foreground">{startIndex}</span> to{' '}
          <span className="font-medium text-foreground">{endIndex}</span> of{' '}
          <span className="font-medium text-foreground">{totalCount}</span> results
        </p>
        <Select 
          value={pageSize.toString()} 
          onValueChange={(v) => onPageSizeChange(parseInt(v))}
        >
          <SelectTrigger className="w-[80px] bg-secondary border-border">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            {pageSizeOptions.map(size => (
              <SelectItem key={size} value={size.toString()}>{size}</SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
      
      <div className="flex items-center gap-1">
        <button
          onClick={() => onPageChange(currentPage - 1)}
          disabled={currentPage === 1}
          className={cn(
            'flex items-center gap-1 px-3 py-1.5 text-sm rounded-lg transition-colors',
            currentPage === 1 
              ? 'bg-secondary text-muted-foreground cursor-not-allowed'
              : 'bg-secondary text-foreground hover:bg-accent'
          )}
        >
          <ChevronLeft className="w-4 h-4" />
          Previous
        </button>
        
        {getPageNumbers().map((page, i) => (
          page === 'ellipsis' ? (
            <span key={`ellipsis-${i}`} className="px-2 text-muted-foreground">...</span>
          ) : (
            <button
              key={page}
              onClick={() => onPageChange(page)}
              className={cn(
                'px-3 py-1.5 text-sm rounded-lg transition-colors',
                currentPage === page
                  ? 'bg-primary text-primary-foreground'
                  : 'bg-secondary text-muted-foreground hover:text-foreground hover:bg-accent'
              )}
            >
              {page}
            </button>
          )
        ))}
        
        <button
          onClick={() => onPageChange(currentPage + 1)}
          disabled={currentPage === totalPages || totalPages === 0}
          className={cn(
            'flex items-center gap-1 px-3 py-1.5 text-sm rounded-lg transition-colors',
            currentPage === totalPages || totalPages === 0
              ? 'bg-secondary text-muted-foreground cursor-not-allowed'
              : 'bg-secondary text-foreground hover:bg-accent'
          )}
        >
          Next
          <ChevronRight className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
}
