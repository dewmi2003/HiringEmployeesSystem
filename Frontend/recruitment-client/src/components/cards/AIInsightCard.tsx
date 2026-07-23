import { Sparkles } from "lucide-react";

interface AIInsightCardProps {
  title: string;
  score?: number;
  summary: string;
  children?: React.ReactNode;
}

export default function AIInsightCard({
  title,
  score,
  summary,
  children,
}: AIInsightCardProps) {
  return (
    <article className="ai-insight">
      <span className="ai-label">
        <Sparkles size={15} aria-hidden="true" />
        AI-generated insight
      </span>
      <h2>{title}</h2>
      {typeof score === "number" ? (
        <>
          <p className="ai-score">{Math.max(0, Math.min(100, score))}%</p>
          <progress
            className="score-progress"
            value={Math.max(0, Math.min(100, score))}
            max={100}
            aria-label={title}
          />
        </>
      ) : null}
      <p>{summary}</p>
      {children}
      <p className="ai-disclaimer">
        Review this recommendation before making a hiring decision.
      </p>
    </article>
  );
}
