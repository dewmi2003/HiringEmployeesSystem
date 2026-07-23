import type { LucideIcon } from "lucide-react";
import { Inbox } from "lucide-react";
import AppButton from "./AppButton";

interface EmptyStateProps {
  title: string;
  description: string;
  icon?: LucideIcon;
  actionLabel?: string;
  onAction?: () => void;
}

export default function EmptyState({
  title,
  description,
  icon: Icon = Inbox,
  actionLabel,
  onAction,
}: EmptyStateProps) {
  return (
    <div className="state-panel">
      <div className="state-content">
        <span className="state-icon">
          <Icon size={24} aria-hidden="true" />
        </span>
        <h2>{title}</h2>
        <p>{description}</p>
        {actionLabel && onAction ? (
          <AppButton onClick={onAction}>{actionLabel}</AppButton>
        ) : null}
      </div>
    </div>
  );
}
