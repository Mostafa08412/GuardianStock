import { Bell } from 'lucide-react';
import { AlertsTable } from '@/components/alerts/AlertsTable';

export default function Alerts() {
  return (
    <div className="p-6 space-y-6 animate-fade-in">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-border pb-6">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Stock Alerts</h1>
          <p className="text-muted-foreground">Monitor and manage items that need restocking.</p>
        </div>
        <div className="p-3 bg-warning/10 text-warning rounded-lg">
          <Bell className="w-6 h-6" />
        </div>
      </div>

      <AlertsTable />
    </div>
  );
}
