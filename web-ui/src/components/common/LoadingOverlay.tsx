/**
 * LoadingOverlay Component
 * 
 * Displays a loading indicator overlay.
 */

import { cn } from '@/lib/utils';
import { Loader2 } from 'lucide-react';

export interface LoadingOverlayProps {
  isLoading: boolean;
  children: React.ReactNode;
  className?: string;
  spinnerSize?: 'sm' | 'md' | 'lg';
  text?: string;
}

const spinnerSizes = {
  sm: 'w-4 h-4',
  md: 'w-8 h-8',
  lg: 'w-12 h-12',
};

export function LoadingOverlay({ 
  isLoading, 
  children, 
  className,
  spinnerSize = 'md',
  text,
}: LoadingOverlayProps) {
  return (
    <div className={cn('relative', className)}>
      {children}
      {isLoading && (
        <div className="absolute inset-0 bg-background/60 backdrop-blur-sm flex flex-col items-center justify-center z-10 rounded-xl">
          <Loader2 className={cn('animate-spin text-primary', spinnerSizes[spinnerSize])} />
          {text && <p className="mt-2 text-sm text-muted-foreground">{text}</p>}
        </div>
      )}
    </div>
  );
}

/**
 * LoadingSpinner Component
 * 
 * Simple inline loading spinner.
 */
export interface LoadingSpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

export function LoadingSpinner({ size = 'md', className }: LoadingSpinnerProps) {
  return (
    <Loader2 className={cn('animate-spin text-primary', spinnerSizes[size], className)} />
  );
}

/**
 * LoadingState Component
 * 
 * Full-page or section loading state.
 */
export interface LoadingStateProps {
  text?: string;
  className?: string;
}

export function LoadingState({ text = 'Loading...', className }: LoadingStateProps) {
  return (
    <div className={cn('flex flex-col items-center justify-center p-12', className)}>
      <Loader2 className="w-8 h-8 animate-spin text-primary mb-4" />
      <p className="text-muted-foreground">{text}</p>
    </div>
  );
}
