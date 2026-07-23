import { forwardRef, useId } from "react";
import type { InputHTMLAttributes, ReactNode } from "react";
import { CircleAlert } from "lucide-react";

interface AppInputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
  hint?: string;
  icon?: ReactNode;
}

const AppInput = forwardRef<HTMLInputElement, AppInputProps>(
  ({ label, error, hint, icon, id, className = "", ...props }, ref) => {
    const generatedId = useId();
    const inputId = id || generatedId;
    const errorId = `${inputId}-error`;
    const hintId = `${inputId}-hint`;

    return (
      <div className="field">
        <label className="field-label" htmlFor={inputId}>
          {label}
        </label>
        <div className="input-wrap">
          {icon ? <span className="input-icon">{icon}</span> : null}
          <input
            ref={ref}
            id={inputId}
            className={[
              "app-input",
              icon ? "app-input--with-icon" : "",
              className,
            ]
              .filter(Boolean)
              .join(" ")}
            aria-invalid={Boolean(error)}
            aria-describedby={error ? errorId : hint ? hintId : undefined}
            {...props}
          />
        </div>
        {error ? (
          <span className="field-error" id={errorId} role="alert">
            <CircleAlert size={14} aria-hidden="true" />
            {error}
          </span>
        ) : hint ? (
          <span className="field-hint" id={hintId}>
            {hint}
          </span>
        ) : null}
      </div>
    );
  },
);

AppInput.displayName = "AppInput";

export default AppInput;
