import { forwardRef, useId } from "react";
import type { TextareaHTMLAttributes } from "react";

interface AppTextareaProps
  extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  label: string;
  error?: string;
  hint?: string;
}

const AppTextarea = forwardRef<HTMLTextAreaElement, AppTextareaProps>(
  ({ label, error, hint, id, ...props }, ref) => {
    const generatedId = useId();
    const textareaId = id || generatedId;

    return (
      <div className="field">
        <label className="field-label" htmlFor={textareaId}>
          {label}
        </label>
        <textarea
          ref={ref}
          id={textareaId}
          className="app-textarea"
          aria-invalid={Boolean(error)}
          {...props}
        />
        {error ? (
          <span className="field-error" role="alert">
            {error}
          </span>
        ) : hint ? (
          <span className="field-hint">{hint}</span>
        ) : null}
      </div>
    );
  },
);

AppTextarea.displayName = "AppTextarea";

export default AppTextarea;
