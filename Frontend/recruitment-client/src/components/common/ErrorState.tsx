import { TriangleAlert } from "lucide-react";
import AppButton from "./AppButton";

interface ErrorStateProps {
  message: string;
  onRetry?: () => void;
}

export default function ErrorState({ message, onRetry }: ErrorStateProps) {
  return (
    <div className="state-panel">
      <div className="state-content">
        <span className="state-icon">
          <TriangleAlert size={24} aria-hidden="true" />
        </span>
        <h2>We could not load this view</h2>
        <p>{message}</p>
        {onRetry ? <AppButton onClick={onRetry}>Try again</AppButton> : null}
      </div>
    </div>
  );
}
