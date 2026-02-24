import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { Search, ShoppingBag, ArrowRight, Activity, ChevronRight, Package, Loader2 } from 'lucide-react';
import { productsService } from '@/api/services/products.service';
import { transactionsService } from '@/api/services/transactions.service';
import { ProductListItem, Transaction } from '@/types/inventory';
import { cn } from '@/lib/utils';

export default function SearchResults() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const query = searchParams.get('q') || '';

    const [loading, setLoading] = useState(true);
    const [products, setProducts] = useState<ProductListItem[]>([]);
    const [transactions, setTransactions] = useState<Transaction[]>([]);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchResults = async () => {
            if (!query) return;

            setLoading(true);
            setError(null);

            try {
                const [productRes, transactionRes] = await Promise.all([
                    productsService.getList({ search: query, pageSize: 50 }),
                    transactionsService.getList({ search: query, pageSize: 50 })
                ]);

                setProducts(productRes.items);
                setTransactions(transactionRes.items);
            } catch (err) {
                console.error('Search failed:', err);
                setError('Failed to fetch search results. Please try again.');
            } finally {
                setLoading(false);
            }
        };

        fetchResults();
    }, [query]);

    if (!query) {
        return (
            <div className="flex flex-col items-center justify-center min-h-[60vh] text-center px-4">
                <div className="w-16 h-16 bg-secondary rounded-full flex items-center justify-center mb-4">
                    <Search className="w-8 h-8 text-muted-foreground" />
                </div>
                <h2 className="text-2xl font-bold text-foreground mb-2">Search for something</h2>
                <p className="text-muted-foreground max-w-md">
                    Type a product name, SKU, or transaction details in the search bar above to get started.
                </p>
            </div>
        );
    }

    return (
        <div className="space-y-8 animate-fade-in">
            {loading ? (
                <div className="flex flex-col items-center justify-center py-20 text-center">
                    <Loader2 className="w-10 h-10 text-primary animate-spin mb-4" />
                    <h3 className="text-lg font-medium text-foreground">Searching...</h3>
                    <p className="text-sm text-muted-foreground mt-1">We're looking through your inventory and logs</p>
                </div>
            ) : error ? (
                <div className="bg-destructive/10 border border-destructive/20 rounded-xl p-8 text-center">
                    <h3 className="text-lg font-semibold text-destructive mb-2">Error</h3>
                    <p className="text-destructive/80 mb-4">{error}</p>
                    <button
                        onClick={() => window.location.reload()}
                        className="px-4 py-2 bg-destructive text-destructive-foreground rounded-lg hover:bg-destructive/90 transition-colors"
                    >
                        Try Again
                    </button>
                </div>
            ) : (
                <>
                    {/* Summary counters */}
                    <div className="flex flex-wrap gap-4 mb-4">
                        <div className="px-4 py-2 bg-card border border-border rounded-full flex items-center gap-2">
                            <Package className="w-4 h-4 text-primary" />
                            <span className="text-sm font-medium text-foreground">{products.length} Products</span>
                        </div>
                        <div className="px-4 py-2 bg-card border border-border rounded-full flex items-center gap-2">
                            <Activity className="w-4 h-4 text-primary" />
                            <span className="text-sm font-medium text-foreground">{transactions.length} Transactions</span>
                        </div>
                    </div>

                    {/* Products Section */}
                    <section>
                        <div className="flex items-center justify-between mb-4 px-2">
                            <div className="flex items-center gap-2">
                                <h3 className="text-lg font-bold text-foreground">Products</h3>
                                <span className="px-2 py-0.5 bg-secondary text-secondary-foreground text-xs font-semibold rounded-full">
                                    {products.length}
                                </span>
                            </div>
                            {products.length > 0 && (
                                <button
                                    onClick={() => navigate(`/products?search=${query}`)}
                                    className="text-xs font-medium text-primary hover:underline flex items-center gap-1"
                                >
                                    View all in Products page <ChevronRight className="w-3 h-3" />
                                </button>
                            )}
                        </div>

                        {products.length === 0 ? (
                            <div className="bg-card/50 border border-border border-dashed rounded-xl p-8 text-center">
                                <p className="text-sm text-muted-foreground">No products found matching "{query}"</p>
                            </div>
                        ) : (
                            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                                {products.map(product => (
                                    <div
                                        key={product.id}
                                        onClick={() => navigate(`/products/${product.id}`)}
                                        className="group bg-card border border-border rounded-xl p-4 hover:border-primary/50 hover:shadow-md transition-all cursor-pointer"
                                    >
                                        <div className="flex flex-col h-full justify-between">
                                            <div>
                                                <div className="text-xs font-mono text-muted-foreground mb-1 group-hover:text-primary transition-colors">
                                                    {product.sku}
                                                </div>
                                                <h4 className="text-sm font-semibold text-foreground group-hover:text-primary transition-colors line-clamp-2">
                                                    {product.name}
                                                </h4>
                                                <p className="text-xs text-muted-foreground mt-1 line-clamp-2">
                                                    {product.description}
                                                </p>
                                            </div>
                                            <div className="mt-4 pt-3 border-t border-border flex items-center justify-between">
                                                <span className="text-sm font-bold text-foreground">${product.price.toLocaleString()}</span>
                                                <span className={cn(
                                                    "text-[10px] px-2 py-0.5 rounded-full font-bold uppercase",
                                                    product.currentStock <= product.lowStockThreshold ? "bg-destructive/10 text-destructive" : "bg-success/10 text-success"
                                                )}>
                                                    {product.currentStock} in stock
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </section>

                    {/* Transactions Section */}
                    <section>
                        <div className="flex items-center justify-between mb-4 px-2">
                            <div className="flex items-center gap-2">
                                <h3 className="text-lg font-bold text-foreground">Transactions</h3>
                                <span className="px-2 py-0.5 bg-secondary text-secondary-foreground text-xs font-semibold rounded-full">
                                    {transactions.length}
                                </span>
                            </div>
                            {transactions.length > 0 && (
                                <button
                                    onClick={() => navigate(`/transactions?search=${query}`)}
                                    className="text-xs font-medium text-primary hover:underline flex items-center gap-1"
                                >
                                    View all in Transactions <ChevronRight className="w-3 h-3" />
                                </button>
                            )}
                        </div>

                        {transactions.length === 0 ? (
                            <div className="bg-card/50 border border-border border-dashed rounded-xl p-8 text-center">
                                <p className="text-sm text-muted-foreground">No transactions found matching "{query}"</p>
                            </div>
                        ) : (
                            <div className="bg-card border border-border rounded-xl overflow-hidden">
                                <table className="w-full text-left">
                                    <thead>
                                        <tr className="bg-secondary/30 text-xs font-semibold text-muted-foreground uppercase border-b border-border">
                                            <th className="px-4 py-3">Date</th>
                                            <th className="px-4 py-3">Product/SKU</th>
                                            <th className="px-4 py-3 text-right">Qty</th>
                                            <th className="px-4 py-3 text-right">Amount</th>
                                            <th className="px-4 py-3 text-center">Action</th>
                                        </tr>
                                    </thead>
                                    <tbody className="divide-y divide-border">
                                        {transactions.map(tx => (
                                            <tr
                                                key={tx.id}
                                                onClick={() => navigate(`/transactions/${tx.id}`)}
                                                className="hover:bg-accent/50 transition-colors cursor-pointer group"
                                            >
                                                <td className="px-4 py-3 text-sm text-foreground">
                                                    {new Date(tx.date).toLocaleDateString()}
                                                </td>
                                                <td className="px-4 py-3">
                                                    <div className="text-sm font-medium text-foreground">{tx.productName}</div>
                                                    <div className="text-[10px] font-mono text-muted-foreground">{tx.productSku}</div>
                                                </td>
                                                <td className="px-4 py-3 text-sm text-right font-medium">
                                                    {tx.type === 'sale' ? '-' : '+'}{tx.quantity}
                                                </td>
                                                <td className="px-4 py-3 text-sm text-right font-bold text-foreground">
                                                    ${tx.amount.toLocaleString()}
                                                </td>
                                                <td className="px-4 py-3">
                                                    <div className="flex justify-center">
                                                        <button className="p-1.5 rounded-lg bg-secondary text-muted-foreground group-hover:text-primary transition-colors">
                                                            <ArrowRight className="w-4 h-4" />
                                                        </button>
                                                    </div>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )}
                    </section>

                    {products.length === 0 && transactions.length === 0 && (
                        <div className="flex flex-col items-center justify-center py-20 text-center">
                            <div className="w-16 h-16 bg-secondary rounded-full flex items-center justify-center mb-4">
                                <Search className="w-8 h-8 text-muted-foreground" />
                            </div>
                            <h3 className="text-xl font-bold text-foreground mb-2">No matching results</h3>
                            <p className="text-muted-foreground max-w-sm">
                                We couldn't find any products or transactions matching your search term. Try adjusting your query.
                            </p>
                        </div>
                    )}
                </>
            )}
        </div>
    );
}
