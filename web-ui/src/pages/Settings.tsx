import { Save, Bell, Shield, Database, Mail, Globe } from 'lucide-react';

export default function Settings() {
  return (
    <div className="space-y-6 max-w-4xl">
      {/* Header */}
      <div className="animate-fade-in">
        <h2 className="text-xl font-bold text-foreground">System Settings</h2>
        <p className="text-sm text-muted-foreground">Configure your inventory management system</p>
      </div>

      {/* General Settings */}
      <div className="bg-card border border-border rounded-xl animate-fade-in" style={{ animationDelay: '50ms' }}>
        <div className="p-6 border-b border-border">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-primary/20 flex items-center justify-center">
              <Globe className="w-5 h-5 text-primary" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-foreground">General Settings</h3>
              <p className="text-sm text-muted-foreground">Basic system configuration</p>
            </div>
          </div>
        </div>
        <div className="p-6 space-y-4">
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">Company Name</label>
            <input
              type="text"
              defaultValue="Acme Corporation"
              className="w-full px-4 py-2 bg-secondary border border-border rounded-lg text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">Currency</label>
            <select className="w-full px-4 py-2 bg-secondary border border-border rounded-lg text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50">
              <option>USD ($)</option>
              <option>EUR (€)</option>
              <option>GBP (£)</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">Timezone</label>
            <select className="w-full px-4 py-2 bg-secondary border border-border rounded-lg text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50">
              <option>UTC-5 (Eastern Time)</option>
              <option>UTC-8 (Pacific Time)</option>
              <option>UTC+0 (GMT)</option>
            </select>
          </div>
        </div>
      </div>

      {/* Notification Settings */}
      <div className="bg-card border border-border rounded-xl animate-fade-in" style={{ animationDelay: '100ms' }}>
        <div className="p-6 border-b border-border">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-warning/20 flex items-center justify-center">
              <Bell className="w-5 h-5 text-warning" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-foreground">Notification Settings</h3>
              <p className="text-sm text-muted-foreground">Configure alerts and notifications</p>
            </div>
          </div>
        </div>
        <div className="p-6 space-y-4">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-foreground">Low Stock Alerts</p>
              <p className="text-xs text-muted-foreground">Get notified when stock falls below threshold</p>
            </div>
            <button className="w-12 h-6 bg-primary rounded-full relative">
              <span className="absolute right-1 top-1 w-4 h-4 bg-primary-foreground rounded-full" />
            </button>
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-foreground">Email Notifications</p>
              <p className="text-xs text-muted-foreground">Receive alerts via email</p>
            </div>
            <button className="w-12 h-6 bg-primary rounded-full relative">
              <span className="absolute right-1 top-1 w-4 h-4 bg-primary-foreground rounded-full" />
            </button>
          </div>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-foreground">Transaction Confirmations</p>
              <p className="text-xs text-muted-foreground">Confirm each transaction via email</p>
            </div>
            <button className="w-12 h-6 bg-secondary rounded-full relative">
              <span className="absolute left-1 top-1 w-4 h-4 bg-muted-foreground rounded-full" />
            </button>
          </div>
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">Default Stock Threshold</label>
            <input
              type="number"
              defaultValue={10}
              className="w-32 px-4 py-2 bg-secondary border border-border rounded-lg text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
            />
          </div>
        </div>
      </div>

      {/* Email Settings */}
      <div className="bg-card border border-border rounded-xl animate-fade-in" style={{ animationDelay: '150ms' }}>
        <div className="p-6 border-b border-border">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-info/20 flex items-center justify-center">
              <Mail className="w-5 h-5 text-info" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-foreground">Email Configuration</h3>
              <p className="text-sm text-muted-foreground">SMTP settings for email notifications</p>
            </div>
          </div>
        </div>
        <div className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-foreground mb-2">SMTP Host</label>
              <input
                type="text"
                placeholder="smtp.example.com"
                className="w-full px-4 py-2 bg-secondary border border-border rounded-lg text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-foreground mb-2">SMTP Port</label>
              <input
                type="text"
                placeholder="587"
                className="w-full px-4 py-2 bg-secondary border border-border rounded-lg text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-foreground mb-2">Sender Email</label>
            <input
              type="email"
              placeholder="noreply@company.com"
              className="w-full px-4 py-2 bg-secondary border border-border rounded-lg text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
            />
          </div>
        </div>
      </div>

      {/* Save Button */}
      <div className="flex justify-end animate-fade-in" style={{ animationDelay: '200ms' }}>
        <button className="flex items-center gap-2 px-6 py-3 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium">
          <Save className="w-5 h-5" />
          Save Changes
        </button>
      </div>
    </div>
  );
}
