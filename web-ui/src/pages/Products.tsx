/**
 * Products Page (Refactored)
 * 
 * Clean, composition-based page using the new architecture.
 */

import { memo } from 'react';
import { useNavigate } from 'react-router-dom';
import ProductsTable from '@/components/products/ProductsTable';

const Products = memo(function Products() {
  const navigate = useNavigate();

  return (
    <div className="space-y-6">
      <ProductsTable onViewProduct={(id) => navigate(`/products/${id}`)} />
    </div>
  );
});

export default Products;
