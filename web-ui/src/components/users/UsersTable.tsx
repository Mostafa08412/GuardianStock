import { useState, useMemo, useEffect } from 'react';
import { Plus, Edit2, Trash2, Shield, UserCheck, User as UserIcon, Search, ChevronLeft, ChevronRight, X, Eye, Loader2 } from 'lucide-react';
import { cn } from '@/lib/utils';
import { User, UserRole, UserStatus } from '@/types/inventory';
import { useAuth } from '@/contexts/AuthContext';
import { usePermissions } from '@/hooks/usePermissions';
import { useSearchParams } from 'react-router-dom';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import AddUserModal from '@/components/users/AddUserModal';
import EditUserModal from '@/components/users/EditUserModal';
import DeleteUserDialog from '@/components/users/DeleteUserDialog';
import UserDetailsModal from '@/components/users/UserDetailsModal';
import { usersService } from '@/api/services/users.service';

interface UsersTableProps {
    // Add props if needed, e.g. custom handlers or overrides
}

export default function UsersTable({ }: UsersTableProps) {
    const { user: authUser } = useAuth();
    const { canPerformAction } = usePermissions();
    const [searchParams, setSearchParams] = useSearchParams();

    // Data states
    const [users, setUsers] = useState<User[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(0);

    // Filter states from URL
    const searchQuery = searchParams.get('q') || '';
    const roleFilter = (searchParams.get('role') as UserRole | 'all') || 'all';
    const statusFilter = (searchParams.get('status') as UserStatus | 'all') || 'all';
    const sortBy = (searchParams.get('sortBy') as 'name' | 'email' | 'role') || 'name';
    const sortOrder = (searchParams.get('sortOrder') as 'asc' | 'desc') || 'asc';
    const currentPage = parseInt(searchParams.get('page') || '1', 10);
    const itemsPerPage = parseInt(searchParams.get('pageSize') || '10', 10);

    // Modal states
    const [showAddModal, setShowAddModal] = useState(false);
    const [editingUser, setEditingUser] = useState<User | null>(null);
    const [deletingUser, setDeletingUser] = useState<User | null>(null);
    const [viewingUser, setViewingUser] = useState<User | null>(null);

    const roleIcons = {
        admin: Shield,
        manager: UserCheck,
        staff: UserIcon,
    };

    const roleColors = {
        admin: 'bg-destructive/20 text-destructive',
        manager: 'bg-warning/20 text-warning',
        staff: 'bg-info/20 text-info',
    };

    const fetchUsers = async () => {
        setIsLoading(true);
        try {
            const response = await usersService.getList({
                SearchTerm: searchQuery || undefined,
                Role: roleFilter === 'all' ? undefined : roleFilter,
                IsActive: statusFilter === 'all' ? undefined : (statusFilter === 'active'),
                SortBy: sortBy,
                SortDescending: sortOrder === 'desc',
                PageNumber: currentPage,
                PageSize: itemsPerPage
            });
            setUsers(response.items);
            setTotalCount(response.totalCount);
            setTotalPages(response.totalPages);
        } catch (error) {
            console.error('Failed to fetch users:', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchUsers();
    }, [searchQuery, roleFilter, statusFilter, sortBy, sortOrder, currentPage, itemsPerPage]);

    const updateFilter = (updates: Record<string, string | null>) => {
        const newParams = new URLSearchParams(searchParams);
        Object.entries(updates).forEach(([key, value]) => {
            if (value === null || value === 'all' || value === '') {
                newParams.delete(key);
            } else {
                newParams.set(key, value);
            }
        });
        // Reset to page 1 for any filter change except page/pageSize
        if (!updates.page && !updates.pageSize) {
            newParams.delete('page');
        }
        setSearchParams(newParams);
    };

    const clearFilters = () => {
        setSearchParams({});
    };

    const hasActiveFilters = searchQuery || roleFilter !== 'all' || statusFilter !== 'all' || sortBy !== 'name' || sortOrder !== 'asc';

    const getPageNumbers = () => {
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

    // Current user ID for self-protection checks
    const currentUserId = authUser?.id || '';

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 animate-fade-in">
                <div>
                    <h2 className="text-xl font-bold text-foreground">User Management</h2>
                    <p className="text-sm text-muted-foreground">{totalCount} users found</p>
                </div>
                {canPerformAction('users.create') && (
                    <button
                        onClick={() => setShowAddModal(true)}
                        className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium text-sm"
                    >
                        <Plus className="w-4 h-4" />
                        Add User
                    </button>
                )}
            </div>

            {/* Filters */}
            <div className="bg-card border border-border rounded-xl p-4 animate-fade-in">
                <div className="space-y-3">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                        <input
                            type="text"
                            placeholder="Search by name or email..."
                            value={searchQuery}
                            onChange={(e) => updateFilter({ q: e.target.value })}
                            className="w-full pl-10 pr-4 py-2 bg-secondary border border-border rounded-lg text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
                        />
                    </div>

                    <div className="flex flex-wrap gap-3">
                        <Select value={roleFilter} onValueChange={(v) => updateFilter({ role: v })}>
                            <SelectTrigger className="w-[140px] bg-secondary border-border">
                                <SelectValue placeholder="Role" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All Roles</SelectItem>
                                <SelectItem value="admin">Admin</SelectItem>
                                <SelectItem value="manager">Manager</SelectItem>
                                <SelectItem value="staff">Staff</SelectItem>
                            </SelectContent>
                        </Select>

                        <Select value={statusFilter} onValueChange={(v) => updateFilter({ status: v })}>
                            <SelectTrigger className="w-[140px] bg-secondary border-border">
                                <SelectValue placeholder="Status" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All Status</SelectItem>
                                <SelectItem value="active">Active</SelectItem>
                                <SelectItem value="inactive">Inactive</SelectItem>
                            </SelectContent>
                        </Select>

                        <Select value={`${sortBy}-${sortOrder}`} onValueChange={(v) => {
                            const [sort, order] = v.split('-') as [typeof sortBy, typeof sortOrder];
                            updateFilter({ sortBy: sort, sortOrder: order });
                        }}>
                            <SelectTrigger className="w-[160px] bg-secondary border-border">
                                <SelectValue placeholder="Sort By" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="name-asc">Name (A-Z)</SelectItem>
                                <SelectItem value="name-desc">Name (Z-A)</SelectItem>
                                <SelectItem value="email-asc">Email (A-Z)</SelectItem>
                                <SelectItem value="email-desc">Email (Z-A)</SelectItem>
                                <SelectItem value="role-asc">Role (A-Z)</SelectItem>
                                <SelectItem value="role-desc">Role (Z-A)</SelectItem>
                            </SelectContent>
                        </Select>

                        {hasActiveFilters && (
                            <button
                                onClick={clearFilters}
                                className="flex items-center gap-1 px-3 py-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
                            >
                                <X className="w-4 h-4" />
                                Clear Filters
                            </button>
                        )}
                    </div>
                </div>
            </div>

            {/* Desktop Table */}
            <div className="hidden md:block bg-card border border-border rounded-xl animate-fade-in relative min-h-[300px]" style={{ animationDelay: '100ms' }}>
                {isLoading && (
                    <div className="absolute inset-0 bg-background/50 backdrop-blur-[1px] flex items-center justify-center z-10 rounded-xl">
                        <Loader2 className="w-8 h-8 text-primary animate-spin" />
                    </div>
                )}
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead>
                            <tr className="border-b border-border">
                                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">User</th>
                                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Email</th>
                                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Role</th>
                                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Status</th>
                                <th className="px-6 py-4 text-right text-xs font-semibold text-muted-foreground uppercase tracking-wider">Actions</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-border">
                            {!isLoading && users.map((user) => {
                                const RoleIcon = roleIcons[user.role as keyof typeof roleIcons] || UserIcon;

                                return (
                                    <tr
                                        key={user.id}
                                        onClick={() => setViewingUser(user)}
                                        className="hover:bg-secondary/50 transition-colors cursor-pointer"
                                    >
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-3">
                                                <div className="w-10 h-10 rounded-full bg-primary/20 flex items-center justify-center text-primary font-semibold">
                                                    {user.avatar}
                                                </div>
                                                <span className="text-sm font-medium text-foreground">{user.name}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className="text-sm text-muted-foreground">{user.email}</span>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-2">
                                                <div className={cn('w-8 h-8 rounded-lg flex items-center justify-center', roleColors[user.role as keyof typeof roleColors])}>
                                                    <RoleIcon className="w-4 h-4" />
                                                </div>
                                                <span className="text-sm font-medium text-foreground capitalize">{user.role}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className={cn(
                                                'text-xs font-medium px-2.5 py-1 rounded-full',
                                                user.status === 'active' ? 'bg-success/20 text-success' : 'bg-muted text-muted-foreground'
                                            )}>
                                                {user.status === 'active' ? 'Active' : 'Inactive'}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4 text-right">
                                            <div className="flex items-center justify-end gap-1">
                                                <button
                                                    onClick={(e) => { e.stopPropagation(); setViewingUser(user); }}
                                                    className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                                                >
                                                    <Eye className="w-4 h-4" />
                                                </button>
                                                <button
                                                    onClick={(e) => { e.stopPropagation(); setEditingUser(user); }}
                                                    className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                                                >
                                                    <Edit2 className="w-4 h-4" />
                                                </button>
                                                <button
                                                    onClick={(e) => { e.stopPropagation(); setDeletingUser(user); }}
                                                    className="p-2 rounded-lg hover:bg-destructive/20 text-muted-foreground hover:text-destructive transition-colors"
                                                >
                                                    <Trash2 className="w-4 h-4" />
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </div>

            {/* Mobile Cards */}
            <div className="md:hidden space-y-4 relative min-h-[200px]">
                {isLoading && (
                    <div className="absolute inset-0 bg-background/50 backdrop-blur-[1px] flex items-center justify-center z-10 rounded-xl">
                        <Loader2 className="w-8 h-8 text-primary animate-spin" />
                    </div>
                )}
                {!isLoading && users.map((user, index) => {
                    const RoleIcon = roleIcons[user.role as keyof typeof roleIcons] || UserIcon;

                    return (
                        <div
                            key={user.id}
                            onClick={() => setViewingUser(user)}
                            className="bg-card border border-border rounded-xl p-4 animate-fade-in cursor-pointer hover:border-primary/50 transition-colors"
                            style={{ animationDelay: `${index * 50}ms` }}
                        >
                            <div className="flex items-start justify-between mb-3">
                                <div className="flex items-center gap-3">
                                    <div className="w-12 h-12 rounded-full bg-primary/20 flex items-center justify-center text-primary font-semibold text-lg">
                                        {user.avatar}
                                    </div>
                                    <div>
                                        <h3 className="font-semibold text-foreground">{user.name}</h3>
                                        <p className="text-sm text-muted-foreground">{user.email}</p>
                                    </div>
                                </div>
                                <span className={cn(
                                    'text-xs font-medium px-2.5 py-1 rounded-full',
                                    user.status === 'active' ? 'bg-success/20 text-success' : 'bg-muted text-muted-foreground'
                                )}>
                                    {user.status === 'active' ? 'Active' : 'Inactive'}
                                </span>
                            </div>

                            <div className="flex items-center justify-between">
                                <div className="flex items-center gap-2">
                                    <div className={cn('w-8 h-8 rounded-lg flex items-center justify-center', roleColors[user.role as keyof typeof roleColors])}>
                                        <RoleIcon className="w-4 h-4" />
                                    </div>
                                    <span className="text-sm font-medium text-foreground capitalize">{user.role}</span>
                                </div>

                                <div className="flex items-center gap-1">
                                    <button
                                        onClick={(e) => { e.stopPropagation(); setEditingUser(user); }}
                                        className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                                    >
                                        <Edit2 className="w-4 h-4" />
                                    </button>
                                    <button
                                        onClick={(e) => { e.stopPropagation(); setDeletingUser(user); }}
                                        className="p-2 rounded-lg hover:bg-destructive/20 text-muted-foreground hover:text-destructive transition-colors"
                                    >
                                        <Trash2 className="w-4 h-4" />
                                    </button>
                                </div>
                            </div>
                        </div>
                    );
                })}
            </div>

            {!isLoading && users.length === 0 && (
                <div className="p-12 text-center bg-card border border-border rounded-xl">
                    <UserIcon className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
                    <h4 className="text-lg font-semibold text-foreground mb-2">No users found</h4>
                    <p className="text-muted-foreground">Try adjusting your filters to see more results.</p>
                </div>
            )}

            {/* Pagination Footer */}
            {!isLoading && totalCount > 0 && (
                <div className="bg-card border border-border rounded-xl px-6 py-4 flex flex-col sm:flex-row items-center justify-between gap-4">
                    <div className="flex items-center gap-4">
                        <p className="text-sm text-muted-foreground">
                            Showing <span className="font-medium text-foreground">{((currentPage - 1) * itemsPerPage) + 1}</span> to{' '}
                            <span className="font-medium text-foreground">{Math.min(currentPage * itemsPerPage, totalCount)}</span> of{' '}
                            <span className="font-medium text-foreground">{totalCount}</span> results
                        </p>
                        <Select value={itemsPerPage.toString()} onValueChange={(v) => updateFilter({ pageSize: v, page: '1' })}>
                            <SelectTrigger className="w-[80px] bg-secondary border-border">
                                <SelectValue />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="5">5</SelectItem>
                                <SelectItem value="10">10</SelectItem>
                                <SelectItem value="20">20</SelectItem>
                            </SelectContent>
                        </Select>
                    </div>

                    <div className="flex items-center gap-1">
                        <button
                            onClick={() => updateFilter({ page: Math.max(1, currentPage - 1).toString() })}
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
                                    onClick={() => updateFilter({ page: page.toString() })}
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
                            onClick={() => updateFilter({ page: Math.min(totalPages, currentPage + 1).toString() })}
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
            )}

            {/* Modals */}
            <AddUserModal
                isOpen={showAddModal}
                onClose={() => setShowAddModal(false)}
                onAdd={fetchUsers}
            />

            <EditUserModal
                isOpen={!!editingUser}
                user={editingUser}
                currentUserId={currentUserId}
                onClose={() => setEditingUser(null)}
                onSave={fetchUsers}
            />

            <DeleteUserDialog
                isOpen={!!deletingUser}
                user={deletingUser}
                currentUserId={currentUserId}
                onClose={() => setDeletingUser(null)}
                onDelete={fetchUsers}
            />

            <UserDetailsModal
                isOpen={!!viewingUser}
                userId={viewingUser?.id || null}
                onClose={() => setViewingUser(null)}
                onEdit={() => {
                    setEditingUser(viewingUser);
                    setViewingUser(null);
                }}
                onStatusChange={fetchUsers}
            />
        </div>
    );
}
