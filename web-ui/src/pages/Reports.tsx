import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, LineChart, Line } from 'recharts';
import { FileText, Download, TrendingUp, TrendingDown, DollarSign, Package } from 'lucide-react';
import { salesChartData, topProductsData, mockDashboardStats } from '@/data/mockData';

const monthlyData = [
  { month: 'Jan', revenue: 45000, expenses: 32000, profit: 13000 },
  { month: 'Feb', revenue: 52000, expenses: 28000, profit: 24000 },
  { month: 'Mar', revenue: 48000, expenses: 35000, profit: 13000 },
  { month: 'Apr', revenue: 61000, expenses: 42000, profit: 19000 },
  { month: 'May', revenue: 55000, expenses: 38000, profit: 17000 },
  { month: 'Jun', revenue: 67000, expenses: 45000, profit: 22000 },
];

export default function Reports() {
  return (
    <div className="space-y-6">
      {/* Report Actions */}
      <div className="flex items-center justify-between animate-fade-in">
        <div>
          <h2 className="text-xl font-bold text-foreground">Analytics & Reports</h2>
          <p className="text-sm text-muted-foreground">Generate and export inventory reports</p>
        </div>
        <div className="flex items-center gap-3">
          <button className="flex items-center gap-2 px-4 py-2 bg-secondary text-foreground rounded-lg hover:bg-accent transition-colors font-medium text-sm">
            <FileText className="w-4 h-4" />
            Generate Report
          </button>
          <button className="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors font-medium text-sm">
            <Download className="w-4 h-4" />
            Export CSV
          </button>
        </div>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '50ms' }}>
          <div className="flex items-center justify-between mb-4">
            <div className="w-10 h-10 rounded-lg bg-success/20 flex items-center justify-center">
              <TrendingUp className="w-5 h-5 text-success" />
            </div>
            <span className="badge-success">+23%</span>
          </div>
          <p className="text-sm text-muted-foreground">Total Revenue</p>
          <p className="text-2xl font-bold text-foreground">$328,000</p>
        </div>

        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '100ms' }}>
          <div className="flex items-center justify-between mb-4">
            <div className="w-10 h-10 rounded-lg bg-warning/20 flex items-center justify-center">
              <DollarSign className="w-5 h-5 text-warning" />
            </div>
            <span className="badge-warning">+8%</span>
          </div>
          <p className="text-sm text-muted-foreground">Total Expenses</p>
          <p className="text-2xl font-bold text-foreground">$220,000</p>
        </div>

        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '150ms' }}>
          <div className="flex items-center justify-between mb-4">
            <div className="w-10 h-10 rounded-lg bg-primary/20 flex items-center justify-center">
              <TrendingUp className="w-5 h-5 text-primary" />
            </div>
            <span className="badge-success">+31%</span>
          </div>
          <p className="text-sm text-muted-foreground">Net Profit</p>
          <p className="text-2xl font-bold text-foreground">$108,000</p>
        </div>

        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '200ms' }}>
          <div className="flex items-center justify-between mb-4">
            <div className="w-10 h-10 rounded-lg bg-info/20 flex items-center justify-center">
              <Package className="w-5 h-5 text-info" />
            </div>
            <span className="badge-info">1,247</span>
          </div>
          <p className="text-sm text-muted-foreground">Items Sold</p>
          <p className="text-2xl font-bold text-foreground">This Month</p>
        </div>
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Revenue Bar Chart */}
        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '250ms' }}>
          <h3 className="text-lg font-semibold text-foreground mb-1">Revenue vs Expenses</h3>
          <p className="text-sm text-muted-foreground mb-6">Monthly comparison</p>
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={monthlyData} margin={{ top: 10, right: 10, left: -10, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="hsl(217 33% 22%)" vertical={false} />
                <XAxis 
                  dataKey="month" 
                  axisLine={false} 
                  tickLine={false}
                  tick={{ fill: 'hsl(215 20% 55%)', fontSize: 12 }}
                />
                <YAxis 
                  axisLine={false} 
                  tickLine={false}
                  tick={{ fill: 'hsl(215 20% 55%)', fontSize: 12 }}
                  tickFormatter={(value) => `$${value / 1000}k`}
                />
                <Tooltip
                  contentStyle={{
                    backgroundColor: 'hsl(222 47% 13%)',
                    border: '1px solid hsl(217 33% 22%)',
                    borderRadius: '8px',
                  }}
                  formatter={(value: number) => [`$${value.toLocaleString()}`, '']}
                />
                <Bar dataKey="revenue" fill="hsl(173 80% 40%)" radius={[4, 4, 0, 0]} name="Revenue" />
                <Bar dataKey="expenses" fill="hsl(38 92% 50%)" radius={[4, 4, 0, 0]} name="Expenses" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Profit Line Chart */}
        <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '300ms' }}>
          <h3 className="text-lg font-semibold text-foreground mb-1">Profit Trend</h3>
          <p className="text-sm text-muted-foreground mb-6">Monthly net profit</p>
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={monthlyData} margin={{ top: 10, right: 10, left: -10, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="hsl(217 33% 22%)" vertical={false} />
                <XAxis 
                  dataKey="month" 
                  axisLine={false} 
                  tickLine={false}
                  tick={{ fill: 'hsl(215 20% 55%)', fontSize: 12 }}
                />
                <YAxis 
                  axisLine={false} 
                  tickLine={false}
                  tick={{ fill: 'hsl(215 20% 55%)', fontSize: 12 }}
                  tickFormatter={(value) => `$${value / 1000}k`}
                />
                <Tooltip
                  contentStyle={{
                    backgroundColor: 'hsl(222 47% 13%)',
                    border: '1px solid hsl(217 33% 22%)',
                    borderRadius: '8px',
                  }}
                  formatter={(value: number) => [`$${value.toLocaleString()}`, 'Profit']}
                />
                <Line 
                  type="monotone" 
                  dataKey="profit" 
                  stroke="hsl(142 76% 36%)" 
                  strokeWidth={3}
                  dot={{ fill: 'hsl(142 76% 36%)', strokeWidth: 2 }}
                  activeDot={{ r: 6, fill: 'hsl(142 76% 36%)' }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Top Products Table */}
      <div className="bg-card border border-border rounded-xl animate-fade-in" style={{ animationDelay: '350ms' }}>
        <div className="p-6 border-b border-border">
          <h3 className="text-lg font-semibold text-foreground">Top Performing Products</h3>
          <p className="text-sm text-muted-foreground">Products with highest revenue this month</p>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border">
                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Rank</th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Product</th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Units Sold</th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Revenue</th>
                <th className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider">Performance</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-border">
              {topProductsData.map((product, index) => {
                const maxRevenue = Math.max(...topProductsData.map(p => p.revenue));
                const percentage = (product.revenue / maxRevenue) * 100;
                
                return (
                  <tr key={index} className="table-row-hover">
                    <td className="px-6 py-4">
                      <span className="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center text-primary font-bold text-sm">
                        {index + 1}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm font-medium text-foreground">{product.name}</span>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm text-foreground">{product.sales}</span>
                    </td>
                    <td className="px-6 py-4">
                      <span className="text-sm font-semibold text-success">${product.revenue.toLocaleString()}</span>
                    </td>
                    <td className="px-6 py-4 w-48">
                      <div className="h-2 bg-secondary rounded-full overflow-hidden">
                        <div 
                          className="h-full bg-primary rounded-full"
                          style={{ width: `${percentage}%` }}
                        />
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
