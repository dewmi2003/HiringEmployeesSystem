import { forwardRef, useId } from "react";
import type { SelectHTMLAttributes } from "react";
import type { SelectOption } from "../../models/common";

interface AppSelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label: string;
  options: SelectOption[];
  error?: string;
  placeholder?: string;
}

const AppSelect = forwardRef<HTMLSelectElement, AppSelectProps>(
  (
    {
      label,
      options,
      error,
      placeholder = "Select an option",
      id,
      ...props
    },
    ref,
  ) => {
    const generatedId = useId();
    const selectId = id || generatedId;

    return (
      <div className="field">
        <label className="field-label" htmlFor={selectId}>
          {label}
        </label>
        <select
          ref={ref}
          id={selectId}
          className="app-select"
          aria-invalid={Boolean(error)}
          {...props}
        >
          <option value="">{placeholder}</option>
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
        {error ? (
          <span className="field-error" role="alert">
            {error}
          </span>
        ) : null}
      </div>
    );
  },
);

AppSelect.displayName = "AppSelect";

export default AppSelect;
