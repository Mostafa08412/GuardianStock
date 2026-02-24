import { useMemo, memo } from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import Sidebar from '@/components/layout/Sidebar';
import Header from '@/components/layout/Header';
import { usePermissions } from '@/hooks/usePermissions';
import { AccessDenied } from '@/components/auth/AccessDenied';

const pageTitles: Record<string, { title: string; subtitle: string }> = {
    dashboard: { title: 'Dashboard', subtitle: 'Overview of your inventory management' },
    products: { title: 'Products', subtitle: 'Manage your product inventory' },
    transactions: { title: 'Transactions', subtitle: 'Track sales and purchases' },
    alerts: { title: 'Stock Alerts', subtitle: 'Monitor low stock items' },
    reports: { title: 'Reports & Analytics', subtitle: 'Insights and performance metrics' },
    categories: { title: 'Categories', subtitle: 'Organize your products' },
    users: { title: 'User Management', subtitle: 'Manage users and permissions' },
    settings: { title: 'Settings', subtitle: 'System configuration' },
    profile: { title: 'Profile', subtitle: 'Manage your account settings' },
    search: { title: 'Search Results', subtitle: 'Search results for products and transactions' },
};

const MemoizedSidebar = memo(Sidebar);
const MemoizedHeader = memo(Header);

export default function MainLayout() {
    const location = useLocation();
    const { canAccessPage } = usePermissions();

    // Extract the page identifier from the pathname (e.g., /products -> products)
    const activePage = useMemo(() => {
        const path = location.pathname.split('/')[1] || 'dashboard';
        return path;
    }, [location.pathname]);

    const pageInfo = useMemo(() => {
        const info = pageTitles[activePage] || pageTitles.dashboard;

        if (activePage === 'search') {
            const query = new URLSearchParams(location.search).get('q');
            return {
                ...info,
                subtitle: query ? `Showing results for "${query}"` : info.subtitle
            };
        }

        return info;
    }, [activePage, location.search]);

    // Check page access and show access denied for restricted pages
    if (!canAccessPage(activePage)) {
        return (
            <div className="min-h-screen bg-background flex flex-col items-center justify-center p-4">
                <AccessDenied />
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-background">
            <MemoizedSidebar />

            <main className="lg:ml-64">
                <MemoizedHeader
                    title={pageInfo.title}
                    subtitle={pageInfo.subtitle}
                />
                <div className="p-4 sm:p-6 lg:p-8">
                    <Outlet />
                </div>
            </main>
        </div>
    );
}
