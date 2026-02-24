import { useState, useEffect } from 'react';
import { User, Mail, Camera, Save, Shield, Key, Loader2 } from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';
import { usePermissions } from '@/hooks/usePermissions';
import { toast } from 'sonner';

export default function Profile() {
  const { user, updateProfile, changePassword } = useAuth();
  const { roleInfo } = usePermissions();
  const [activeTab, setActiveTab] = useState('general');
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const [profile, setProfile] = useState({
    firstName: '',
    lastName: '',
    email: '',
    location: 'San Francisco, CA',
    company: 'InventoryPro Inc.',
    avatar: '',
  });

  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: '',
  });

  // Update profile when user changes
  useEffect(() => {
    if (user) {
      setProfile(prev => ({
        ...prev,
        firstName: user.firstName || '',
        lastName: user.lastName || '',
        email: user.email || '',
        avatar: (user.firstName?.[0] || '') + (user.lastName?.[0] || ''),
      }));
    }
  }, [user]);

  const handleProfileChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setProfile({ ...profile, [e.target.name]: e.target.value });
  };

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPasswordData({ ...passwordData, [e.target.name]: e.target.value });
  };

  const handleSaveProfile = async () => {
    setIsLoading(true);
    const result = await updateProfile({
      firstName: profile.firstName,
      lastName: profile.lastName,
    });

    if (result.success) {
      toast.success('Profile updated successfully!');
      setIsEditing(false);
    } else {
      toast.error(result.error || 'Failed to update profile');
    }
    setIsLoading(false);
  };

  const handleUpdatePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (passwordData.newPassword !== passwordData.confirmNewPassword) {
      toast.error('New passwords do not match');
      return;
    }

    setIsLoading(true);
    const result = await changePassword({
      currentPassword: passwordData.currentPassword,
      newPassword: passwordData.newPassword,
      confirmNewPassword: passwordData.confirmNewPassword,
    });

    if (result.success) {
      toast.success('Password updated successfully!');
      setPasswordData({
        currentPassword: '',
        newPassword: '',
        confirmNewPassword: '',
      });
    } else {
      toast.error(result.error || 'Failed to update password');
    }
    setIsLoading(false);
  };

  const tabs = [
    { id: 'general', label: 'General', icon: User },
    { id: 'security', label: 'Security', icon: Shield },
  ];

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Profile Header */}
      <div className="bg-card border border-border rounded-xl overflow-hidden">
        <div className="h-32 bg-gradient-to-r from-primary via-primary/80 to-primary/60"></div>
        <div className="px-6 pb-6">
          <div className="flex flex-col sm:flex-row sm:items-end gap-4 -mt-12">
            <div className="relative">
              <div className="w-24 h-24 rounded-xl border-4 border-card bg-secondary overflow-hidden">
                <div className="w-full h-full flex items-center justify-center bg-primary/10">
                  <span className="text-2xl font-bold text-primary">{profile.avatar || 'U'}</span>
                </div>
              </div>
              <button className="absolute -bottom-1 -right-1 p-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors">
                <Camera className="w-4 h-4" />
              </button>
            </div>
            <div className="flex-1">
              <h2 className="text-xl font-bold text-foreground">
                {user?.fullName || `${profile.firstName} ${profile.lastName}`.trim() || 'User'}
              </h2>
              <p className="text-muted-foreground">{roleInfo.label}</p>
            </div>
            <button
              onClick={() => isEditing ? handleSaveProfile() : setIsEditing(true)}
              disabled={isLoading}
              className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium text-sm disabled:opacity-50"
            >
              {isLoading ? (
                <Loader2 className="w-4 h-4 animate-spin" />
              ) : (
                <Save className="w-4 h-4" />
              )}
              {isEditing ? 'Save Changes' : 'Edit Profile'}
            </button>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="bg-card border border-border rounded-xl">
        <div className="flex border-b border-border overflow-x-auto">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`flex items-center gap-2 px-6 py-4 text-sm font-medium transition-colors border-b-2 -mb-px whitespace-nowrap ${activeTab === tab.id
                ? 'border-primary text-primary'
                : 'border-transparent text-muted-foreground hover:text-foreground'
                }`}
            >
              <tab.icon className="w-4 h-4" />
              {tab.label}
            </button>
          ))}
        </div>

        <div className="p-6">
          {activeTab === 'general' && (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">First Name</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="text"
                    name="firstName"
                    value={profile.firstName}
                    onChange={handleProfileChange}
                    disabled={!isEditing}
                    className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground disabled:opacity-60 focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">Last Name</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="text"
                    name="lastName"
                    value={profile.lastName}
                    onChange={handleProfileChange}
                    disabled={!isEditing}
                    className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground disabled:opacity-60 focus:outline-none focus:ring-2 focus:ring-primary/50"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground">Email</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                  <input
                    type="email"
                    name="email"
                    value={profile.email}
                    disabled={true}
                    className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground opacity-60 cursor-not-allowed"
                  />
                </div>
              </div>
            </div>
          )}

          {activeTab === 'security' && (
            <div className="space-y-6 max-w-lg">
              <form onSubmit={handleUpdatePassword} className="space-y-4">
                <h3 className="text-lg font-semibold text-foreground">Change Password</h3>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-foreground">Current Password</label>
                  <div className="relative">
                    <Key className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                    <input
                      type="password"
                      name="currentPassword"
                      value={passwordData.currentPassword}
                      onChange={handlePasswordChange}
                      placeholder="••••••••"
                      required
                      className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-foreground">New Password</label>
                  <div className="relative">
                    <Key className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                    <input
                      type="password"
                      name="newPassword"
                      value={passwordData.newPassword}
                      onChange={handlePasswordChange}
                      placeholder="••••••••"
                      required
                      className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-foreground">Confirm New Password</label>
                  <div className="relative">
                    <Key className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-muted-foreground" />
                    <input
                      type="password"
                      name="confirmNewPassword"
                      value={passwordData.confirmNewPassword}
                      onChange={handlePasswordChange}
                      placeholder="••••••••"
                      required
                      className="w-full pl-11 pr-4 py-3 bg-secondary border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={isLoading}
                  className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium text-sm disabled:opacity-50"
                >
                  {isLoading && <Loader2 className="w-4 h-4 animate-spin" />}
                  Update Password
                </button>
              </form>

              <div className="pt-6 border-t border-border space-y-4">
                <h3 className="text-lg font-semibold text-destructive">Danger Zone</h3>
                <p className="text-sm text-muted-foreground">
                  Once you delete your account, there is no going back. Please be certain.
                </p>
                <button disabled className="px-4 py-2 bg-destructive/10 text-destructive rounded-lg opacity-50 cursor-not-allowed font-medium text-sm">
                  Delete Account
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
