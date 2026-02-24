/**
 * Transactions Page (Refactored)
 * 
 * Clean, composition-based page using the new architecture.
 */

import { memo } from 'react';
import { useNavigate } from 'react-router-dom';
import TransactionsTable from '@/components/transactions/TransactionsTable';

const Transactions = memo(function Transactions() {
  const navigate = useNavigate();

  return (
    <div className="space-y-6">
      <TransactionsTable onViewDetails={(id) => navigate(`/transactions/${id}`)} />
    </div>
  );
});

export default Transactions;
