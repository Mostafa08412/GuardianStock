import { useNavigate } from 'react-router-dom';
import CategoriesTable from '@/components/categories/CategoriesTable';
import { memo } from 'react';

const Categories = memo(function Categories() {
  const navigate = useNavigate();

  return (
    <div className="space-y-6">
      <CategoriesTable onViewDetails={(category) => navigate(`/categories/${category.id}`)} />
    </div>
  );
});

export default Categories;
