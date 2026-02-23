import { useState, useMemo, useEffect, useCallback, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Plus, Edit2, Trash2, FolderOpen, Package, Search, ChevronLeft, ChevronRight, X } from 'lucide-react';
import { categoriesService } from '@/api';
import { Category, CategoryListItem } from '@/types/inventory';
import { useRole } from '@/contexts/RoleContext';
import { cn } from '@/lib/utils';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import AddCategoryModal from '@/components/categories/AddCategoryModal';
import EditCategoryModal from '@/components/categories/EditCategoryModal';
import DeleteCategoryDialog from '@/components/categories/DeleteCategoryDialog';
import { PermissionGate } from '@/components/auth/PermissionGate';

interface CategoriesTableProps {
    onViewDetails?: (category: CategoryListItem) => void;
}

export default function CategoriesTable({ onViewDetails }: CategoriesTableProps) {
    const [searchParams, setSearchParams] = useSearchParams();
    const isInitialMount = useRef(true);

    const { hasPermission } = useRole();
    const canManage = hasPermission(['admin']);

    const [categories, setCategories] = useState<CategoryListItem[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(1);

    // Initialize state from URL params
    const [searchInput, setSearchInput] = useState(searchParams.get('search') || '');
    const [searchQuery, setSearchQuery] = useState(searchParams.get('search') || '');
    const [productCountFilter, setProductCountFilter] = useState<'all' | '0-20' | '20-50' | '50-100' | '100+'>(
        (searchParams.get('count') as any) || 'all'
    );
    const [sortBy, setSortBy] = useState<'name' | 'products'>(
        (searchParams.get('sortBy') as any) || 'name'
    );
    const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>(
        (searchParams.get('sortOrder') as any) || 'asc'
    );
    const [currentPage, setCurrentPage] = useState(parseInt(searchParams.get('page') || '1'));
    const [itemsPerPage, setItemsPerPage] = useState(parseInt(searchParams.get('pageSize') || '6'));

    // Debounce search input
    useEffect(() => {
        const timer = setTimeout(() => {
            if (searchInput !== searchQuery) {
                setSearchQuery(searchInput);
            }
        }, 400);
        return () => clearTimeout(timer);
    }, [searchInput, searchQuery]);

    // Modal states
    const [showAddModal, setShowAddModal] = useState(false);
    const [editingCategory, setEditingCategory] = useState<CategoryListItem | null>(null);
    const [deletingCategory, setDeletingCategory] = useState<CategoryListItem | null>(null);

    // Update URL params whenever state changes
    useEffect(() => {
        if (isInitialMount.current) {
            isInitialMount.current = false;
            return;
        }

        const params: Record<string, string> = {};
        if (searchQuery) params.search = searchQuery;
        if (productCountFilter !== 'all') params.count = productCountFilter;
        if (sortBy !== 'name') params.sortBy = sortBy;
        if (sortOrder !== 'asc') params.sortOrder = sortOrder;
        if (currentPage !== 1) params.page = currentPage.toString();
        if (itemsPerPage !== 6) params.pageSize = itemsPerPage.toString();

        setSearchParams(params, { replace: true });
    }, [searchQuery, productCountFilter, sortBy, sortOrder, currentPage, itemsPerPage, setSearchParams]);

    // Handle browser back/forward buttons
    useEffect(() => {
        const search = searchParams.get('search') || '';
        const count = (searchParams.get('count') as any) || 'all';
        const sort = (searchParams.get('sortBy') as any) || 'name';
        const order = (searchParams.get('sortOrder') as any) || 'asc';
        const page = parseInt(searchParams.get('page') || '1');
        const pageSize = parseInt(searchParams.get('pageSize') || '6');

        setSearchQuery(search);
        setSearchInput(search);
        setProductCountFilter(count);
        setSortBy(sort);
        setSortOrder(order);
        setCurrentPage(page);
        setItemsPerPage(pageSize);
    }, [searchParams]);

    const getProductCountRange = useCallback(() => {
        switch (productCountFilter) {
            case '0-20': return { min: 0, max: 20 };
            case '20-50': return { min: 20, max: 50 };
            case '50-100': return { min: 50, max: 100 };
            case '100+': return { min: 100 };
            default: return {};
        }
    }, [productCountFilter]);

    const fetchCategories = useCallback(async () => {
        setIsLoading(true);
        try {
            const { min, max } = getProductCountRange();
            const res = await categoriesService.getList({
                search: searchQuery || undefined,
                minProductCount: min,
                maxProductCount: max,
                sortBy: sortBy,
                sortOrder: sortOrder,
                page: currentPage,
                pageSize: itemsPerPage
            });
            setCategories(res.items);
            setTotalCount(res.totalCount);
            setTotalPages(res.totalPages);
        } catch (error) {
            console.error('Failed to fetch categories:', error);
        } finally {
            setIsLoading(false);
        }
    }, [searchQuery, productCountFilter, sortBy, sortOrder, currentPage, itemsPerPage, getProductCountRange]);

    useEffect(() => {
        fetchCategories();
    }, [fetchCategories]);

    // Reset page when filters change (via user interaction)
    useEffect(() => {
        if (!isInitialMount.current) {
            setCurrentPage(1);
        }
    }, [searchQuery, productCountFilter, sortBy, sortOrder]);

    const clearFilters = () => {
        setSearchInput('');
        setSearchQuery('');
        setProductCountFilter('all');
        setSortBy('name');
        setSortOrder('asc');
        setCurrentPage(1);
    };

    const hasActiveFilters = searchInput || productCountFilter !== 'all';

    const handleAddCategory = async (newCategory: { name: string; description: string }) => {
        // Ideally define this in service and call it
        // For now, mirroring previous behavior but with refresh
        // Assuming there is a create method in service based on earlier view
        try {
            await categoriesService.create({ name: newCategory.name, description: newCategory.description });
            fetchCategories();
        } catch (error) {
            console.error("Failed to create category", error);
            throw error;
        }
    };

    const handleEditCategory = async (updatedCategory: CategoryListItem) => {
        try {
            await categoriesService.update({ id: updatedCategory.id, name: updatedCategory.name, description: updatedCategory.description });
            fetchCategories();
        } catch (error) {
            console.error("Failed to update category", error);
            throw error;
        }
    };

    const handleDeleteCategory = async (categoryId: string) => {
        try {
            await categoriesService.delete(categoryId);
            fetchCategories(); // Refresh list after delete
        } catch (error) {
            console.error("Failed to delete category", error);
            throw error;
        }
    };

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

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 animate-fade-in">
                <div>
                    <h2 className="text-xl font-bold text-foreground">Product Categories</h2>
                    <p className="text-sm text-muted-foreground">{totalCount} categories found</p>
                </div>
                <PermissionGate action="categories.create">
                    <button
                        onClick={() => setShowAddModal(true)}
                        className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium text-sm"
                    >
                        <Plus className="w-4 h-4" />
                        Add Category
                    </button>
                </PermissionGate>
            </div>

            {/* Filters */}
            <div className="bg-card border border-border rounded-xl p-4 animate-fade-in">
                <div className="space-y-3">
                    <div className="relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                        <input
                            type="text"
                            placeholder="Search categories..."
                            value={searchInput}
                            onChange={(e) => setSearchInput(e.target.value)}
                            className="w-full pl-10 pr-4 py-2 bg-secondary border border-border rounded-lg text-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary/50"
                        />
                    </div>

                    <div className="flex flex-wrap gap-3">
                        <Select value={productCountFilter} onValueChange={(v) => setProductCountFilter(v as any)}>
                            <SelectTrigger className="w-[160px] bg-secondary border-border">
                                <SelectValue placeholder="Product Count" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All Counts</SelectItem>
                                <SelectItem value="0-20">0 - 20 Products</SelectItem>
                                <SelectItem value="20-50">20 - 50 Products</SelectItem>
                                <SelectItem value="50-100">50 - 100 Products</SelectItem>
                                <SelectItem value="100+">100+ Products</SelectItem>
                            </SelectContent>
                        </Select>

                        <Select value={`${sortBy}-${sortOrder}`} onValueChange={(v) => {
                            const [sort, order] = v.split('-') as [typeof sortBy, typeof sortOrder];
                            setSortBy(sort);
                            setSortOrder(order);
                        }}>
                            <SelectTrigger className="w-[180px] bg-secondary border-border">
                                <SelectValue placeholder="Sort By" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="name-asc">Name (A-Z)</SelectItem>
                                <SelectItem value="name-desc">Name (Z-A)</SelectItem>
                                <SelectItem value="products-asc">Products (Low-High)</SelectItem>
                                <SelectItem value="products-desc">Products (High-Low)</SelectItem>
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

            {/* Categories Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {categories.map((category, index) => (
                    <div
                        key={category.id}
                        onClick={() => onViewDetails?.(category)}
                        className="bg-card border border-border rounded-xl p-6 hover:border-primary/50 transition-all duration-300 animate-fade-in group cursor-pointer"
                        style={{ animationDelay: `${index * 50}ms` }}
                    >
                        <div className="flex items-start justify-between mb-4">
                            <div className="w-12 h-12 rounded-xl bg-primary/20 flex items-center justify-center">
                                <FolderOpen className="w-6 h-6 text-primary" />
                            </div>
                            <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                                <PermissionGate action="categories.edit">
                                    <button
                                        onClick={(e) => { e.stopPropagation(); setEditingCategory(category); }}
                                        className="p-2 rounded-lg hover:bg-secondary text-muted-foreground hover:text-foreground transition-colors"
                                    >
                                        <Edit2 className="w-4 h-4" />
                                    </button>
                                </PermissionGate>
                                <PermissionGate action="categories.delete">
                                    <button
                                        onClick={(e) => { e.stopPropagation(); setDeletingCategory(category); }}
                                        className="p-2 rounded-lg hover:bg-destructive/20 text-muted-foreground hover:text-destructive transition-colors"
                                    >
                                        <Trash2 className="w-4 h-4" />
                                    </button>
                                </PermissionGate>
                            </div>
                        </div>

                        <h3 className="text-lg font-semibold text-foreground mb-1">{category.name}</h3>
                        <p className="text-sm text-muted-foreground mb-4">{category.description}</p>

                        <div className="flex items-center gap-2 pt-4 border-t border-border">
                            <Package className="w-4 h-4 text-muted-foreground" />
                            <span className="text-sm text-muted-foreground">
                                <span className="font-semibold text-foreground">{category.productCount}</span> products
                            </span>
                        </div>
                    </div>
                ))}
            </div>

            {categories.length === 0 && !isLoading && (
                <div className="p-12 text-center bg-card border border-border rounded-xl">
                    <p className="text-muted-foreground">No categories found</p>
                </div>
            )}

            {/* Pagination Footer */}
            {totalCount > 0 && (
                <div className="bg-card border border-border rounded-xl px-6 py-4 flex flex-col sm:flex-row items-center justify-between gap-4">
                    <div className="flex items-center gap-4">
                        <p className="text-sm text-muted-foreground">
                            Showing <span className="font-medium text-foreground">{((currentPage - 1) * itemsPerPage) + 1}</span> to{' '}
                            <span className="font-medium text-foreground">{Math.min(currentPage * itemsPerPage, totalCount)}</span> of{' '}
                            <span className="font-medium text-foreground">{totalCount}</span> results
                        </p>
                        <Select value={itemsPerPage.toString()} onValueChange={(v) => { setItemsPerPage(parseInt(v)); setCurrentPage(1); }}>
                            <SelectTrigger className="w-[80px] bg-secondary border-border">
                                <SelectValue />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="3">3</SelectItem>
                                <SelectItem value="6">6</SelectItem>
                                <SelectItem value="9">9</SelectItem>
                                <SelectItem value="12">12</SelectItem>
                            </SelectContent>
                        </Select>
                    </div>

                    <div className="flex items-center gap-1">
                        <button
                            onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
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
                                    onClick={() => setCurrentPage(page)}
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
                            onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
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
            <AddCategoryModal
                isOpen={showAddModal}
                onClose={() => setShowAddModal(false)}
                onAdd={handleAddCategory}
            />

            <EditCategoryModal
                isOpen={!!editingCategory}
                category={editingCategory}
                onClose={() => setEditingCategory(null)}
                onSave={handleEditCategory}
            />

            <DeleteCategoryDialog
                isOpen={!!deletingCategory}
                category={deletingCategory}
                onClose={() => setDeletingCategory(null)}
                onDelete={handleDeleteCategory}
            />
        </div>
    );
}
