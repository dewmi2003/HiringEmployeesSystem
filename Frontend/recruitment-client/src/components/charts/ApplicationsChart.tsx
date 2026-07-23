import {
  Area,
  AreaChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import type { MonthlyStat } from "../../models/dashboard";
import EmptyState from "../common/EmptyState";

interface ApplicationsChartProps {
  data: MonthlyStat[];
}

export default function ApplicationsChart({ data }: ApplicationsChartProps) {
  if (data.length === 0) {
    return (
      <EmptyState
        title="No trend data"
        description="Monthly activity will appear when records are available."
      />
    );
  }

  return (
    <div className="chart-container" aria-label="Monthly recruitment activity">
      <ResponsiveContainer width="100%" height="100%">
        <AreaChart data={data} margin={{ top: 10, right: 10, left: -18, bottom: 0 }}>
          <defs>
            <linearGradient id="activityFill" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#4f46e5" stopOpacity={0.26} />
              <stop offset="95%" stopColor="#4f46e5" stopOpacity={0.02} />
            </linearGradient>
          </defs>
          <CartesianGrid stroke="#dbe2ea" strokeDasharray="3 3" vertical={false} />
          <XAxis
            dataKey="monthName"
            axisLine={false}
            tickLine={false}
            tick={{ fill: "#667085", fontSize: 12 }}
          />
          <YAxis
            allowDecimals={false}
            axisLine={false}
            tickLine={false}
            tick={{ fill: "#667085", fontSize: 12 }}
          />
          <Tooltip
            contentStyle={{
              border: "1px solid #dbe2ea",
              borderRadius: 6,
              boxShadow: "0 10px 28px rgba(16, 24, 40, 0.09)",
            }}
          />
          <Area
            type="monotone"
            dataKey="count"
            name="Activity"
            stroke="#4f46e5"
            strokeWidth={2}
            fill="url(#activityFill)"
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
}
