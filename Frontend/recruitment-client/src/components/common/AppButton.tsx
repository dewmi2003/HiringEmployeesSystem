import type { ButtonHTMLAttributes, ReactNode } from "react";

type ButtonVariant = "primary" | "secondary" | "ghost" | "danger";
type ButtonSize = "normal" | "small";

interface AppButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  icon?: ReactNode;
  loading?: boolean;
  iconOnly?: boolean;
}

export default function AppButton({
  variant = "primary",
  size = "normal",
  icon,
  loading = false,
  iconOnly = false,
  className = "",
  children,
  disabled,
  type = "button",
  ...props
}: AppButtonProps) {
  const classes = [
    "app-button",
    `app-button--${variant}`,
    size === "small" ? "app-button--small" : "",
    iconOnly ? "app-button--icon" : "",
    className,
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <button
      className={classes}
      disabled={disabled || loading}
      type={type}
      {...props}
    >
      {loading ? <span className="button-spinner" aria-hidden="true" /> : icon}
      {iconOnly ? <span className="sr-only">{children}</span> : children}
    </button>
  );
}
