import { ShieldX, ArrowLeft } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { usePermissions } from '@/hooks/usePermissions';

interface AccessDeniedProps {
  onGoBack?: () => void;
  message?: string;
}

export function AccessDenied({ onGoBack, message }: AccessDeniedProps) {
  const { roleInfo } = usePermissions();

  return (
    <div className="flex flex-col items-center justify-center min-h-[400px] p-8 animate-fade-in">
      <div className="w-20 h-20 rounded-full bg-destructive/20 flex items-center justify-center mb-6">
        <ShieldX className="w-10 h-10 text-destructive" />
      </div>
      
      <h2 className="text-2xl font-bold text-foreground mb-2">Access Denied</h2>
      
      <p className="text-muted-foreground text-center max-w-md mb-4">
        {message || "You don't have permission to access this page."}
      </p>
      
      <div className="bg-secondary/50 border border-border rounded-lg px-4 py-3 mb-6">
        <p className="text-sm text-muted-foreground">
          Your current role: <span className="font-medium text-foreground">{roleInfo.label}</span>
        </p>
        <p className="text-xs text-muted-foreground mt-1">{roleInfo.description}</p>
      </div>

      {onGoBack && (
        <Button variant="outline" onClick={onGoBack} className="gap-2">
          <ArrowLeft className="w-4 h-4" />
          Go Back
        </Button>
      )}
    </div>
  );
}
