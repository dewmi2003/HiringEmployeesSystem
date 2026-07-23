import type { PropsWithChildren } from "react";
import { humanizeStatus } from "../../utils/formatters";

type BadgeTone = "neutral" | "success" | "warning" | "danger" | "info" | "primary";

const toneByStatus: Record<string, BadgeTone> = {
  published: "success",
  active: "success",
  hired: "success",
  selected: "success",
  completed: "success",
  shortlisted: "primary",
  interviewscheduled: "info",
  scheduled: "info",
  submitted: "info",
  underreview: "warning",
  pending: "warning",
  draft: "neutral",
  closed: "neutral",
  archived: "neutral",
  rejected: "danger",
  failed: "danger",
};

interface AppBadgeProps extends PropsWithChildren {
  tone?: BadgeTone;
}

export default function AppBadge({ tone, children }: AppBadgeProps) {
  const text = String(children ?? "");
  const resolvedTone =
    tone || toneByStatus[text.toLowerCase().replace(/\s+/g, "")] || "neutral";

  return (
    <span className={`app-badge app-badge--${resolvedTone}`}>
      {humanizeStatus(text)}
    </span>
  );
}
