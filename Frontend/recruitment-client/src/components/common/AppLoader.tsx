interface AppLoaderProps {
  label?: string;
}

export default function AppLoader({ label = "Loading" }: AppLoaderProps) {
  return (
    <div className="loader" role="status" aria-live="polite">
      <span className="loader-ring" aria-hidden="true" />
      <span>{label}</span>
    </div>
  );
}
