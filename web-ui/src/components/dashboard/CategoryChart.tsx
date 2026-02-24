import { useMemo } from 'react';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts';
import { CategoryDistribution } from '@/types';

interface CategoryChartProps {
  data: CategoryDistribution[];
}

export default function CategoryChart({ data }: CategoryChartProps) {
  // Process data to show top 10 and group others
  const processedData = useMemo(() => {
    if (!data || data.length === 0) return [];

    // Sort by count descending
    const sorted = [...data].sort((a, b) => b.value - a.value);

    if (sorted.length <= 10) return sorted;

    const top10 = sorted.slice(0, 10);
    const others = sorted.slice(10);
    const othersCount = others.reduce((sum, item) => sum + item.value, 0);

    return [
      ...top10,
      { name: 'Others..', value: othersCount }
    ];
  }, [data]);

  // Use a default palette if data items don't have fill color (API might not send color)
  const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8', '#82ca9d', '#ffc658', '#8dd1e1', '#a4de6c', '#d0ed57', '#a8a8a8'];

  return (
    <div className="bg-card border border-border rounded-xl p-6 animate-fade-in" style={{ animationDelay: '300ms' }}>
      <div className="mb-4">
        <h3 className="text-lg font-semibold text-foreground">Category Distribution</h3>
        <p className="text-sm text-muted-foreground">Products by category</p>
      </div>

      <div className="h-64">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={processedData}
              cx="50%"
              cy="50%"
              innerRadius={60}
              outerRadius={90}
              paddingAngle={4}
              dataKey="value"
            >
              {processedData.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
              ))}
            </Pie>
            <Tooltip
              contentStyle={{
                backgroundColor: 'hsl(var(--card))',
                border: '1px solid hsl(var(--border))',
                borderRadius: '8px',
                color: 'hsl(var(--foreground))',
              }}
              labelStyle={{ color: 'hsl(var(--foreground))' }}
              itemStyle={{ color: 'hsl(var(--foreground))' }}
            />
          </PieChart>
        </ResponsiveContainer>
      </div>

      <div className="grid grid-cols-2 gap-2 mt-2">
        {processedData.map((item, index) => (
          <div key={index} className="flex items-center gap-2">
            <div className="w-3 h-3 rounded-full" style={{ backgroundColor: COLORS[index % COLORS.length] }} />
            <span className="text-xs text-muted-foreground truncate">{item.name}</span>
            <span className="text-xs font-medium text-foreground ml-auto">{item.value}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
