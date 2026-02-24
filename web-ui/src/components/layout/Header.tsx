import { Search } from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';
import { usePermissions } from '@/hooks/usePermissions';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import MobileSidebar from './MobileSidebar';
import ThemeToggle from './ThemeToggle';

interface HeaderProps {
  title: string;
  subtitle?: string;
}

export default function Header({ title, subtitle }: HeaderProps) {
  const { user } = useAuth();
  const { roleInfo } = usePermissions();
  const navigate = useNavigate();
  const [searchValue, setSearchValue] = useState('');

  const handleSearch = (e?: React.FormEvent) => {
    e?.preventDefault();
    if (searchValue.trim()) {
      navigate(`/search?q=${encodeURIComponent(searchValue.trim())}`);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  return (
    <header className="sticky top-0 z-30 bg-background/80 backdrop-blur-xl border-b border-border">
      <div className="flex items-center justify-between px-4 sm:px-6 lg:px-8 py-4 gap-4">
        {/* Mobile Menu + Title Section */}
        <div className="flex items-center gap-3 min-w-0">
          <MobileSidebar />
          <div className="animate-fade-in min-w-0">
            <h1 className="text-lg sm:text-xl lg:text-2xl font-bold text-foreground truncate">{title}</h1>
            {subtitle && <p className="text-xs sm:text-sm text-muted-foreground mt-0.5 hidden sm:block">{subtitle}</p>}
          </div>
        </div>

        {/* Actions Section */}
        <div className="flex items-center gap-2 sm:gap-4 flex-shrink-0">
          {/* Search Bar - Hidden on mobile, visible from sm */}
          <div className="relative hidden md:block">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              type="text"
              placeholder="Search products, transactions..."
              value={searchValue}
              onChange={(e) => setSearchValue(e.target.value)}
              onKeyDown={handleKeyDown}
              className="w-48 lg:w-72 pl-10 pr-4 py-2 bg-secondary border border-border rounded-lg text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
            />
          </div>

          {/* Mobile Search Button */}
          <button
            onClick={() => handleSearch()}
            className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors md:hidden"
          >
            <Search className="w-5 h-5" />
          </button>

          {/* Theme Toggle */}
          <ThemeToggle />

          {/* User Avatar - Simplified on mobile */}
          {user && (
            <button
              onClick={() => navigate('/profile')}
              className="flex items-center gap-2 sm:gap-3 pl-2 sm:pl-4 border-l border-border hover:opacity-80 transition-opacity text-left"
              title="View Profile"
            >
              <div className="text-right hidden sm:block">
                <p className="text-sm font-medium text-foreground">{user.firstName} {user.lastName}</p>
                <p className="text-xs text-muted-foreground">{roleInfo.label}</p>
              </div>
              <div className="w-8 h-8 sm:w-10 sm:h-10 rounded-full bg-primary/20 flex items-center justify-center text-primary font-semibold text-sm">
                {(user.firstName?.[0] || '') + (user.lastName?.[0] || '')}
              </div>
            </button>
          )}
        </div>
      </div>
    </header>
  );
}
