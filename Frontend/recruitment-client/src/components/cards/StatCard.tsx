import type { LucideIcon } from "lucide-react";

interface StatCardProps {
  label: string;
  value: string | number;
  detail: string;
  icon: LucideIcon;
  tone?: "primary" | "green" | "blue" | "amber";
}

export default function StatCard({
  label,
  value,
  detail,
  icon: Icon,
  tone = "primary",
}: StatCardProps) {
  return (
    <article className="stat-card">
      <span className={`stat-icon stat-icon--${tone}`}>
        <Icon size={21} aria-hidden="true" />
      </span>
      <div>
        <p className="stat-label">{label}</p>
        <p className="stat-value">{value}</p>
      </div>
      <p className="stat-detail">{detail}</p>
    </article>
  );
}
