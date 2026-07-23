import { Component, type ErrorInfo, type PropsWithChildren } from "react";
import { TriangleAlert } from "lucide-react";

interface AppErrorBoundaryState {
  hasError: boolean;
}

export default class AppErrorBoundary extends Component<
  PropsWithChildren,
  AppErrorBoundaryState
> {
  state: AppErrorBoundaryState = { hasError: false };

  static getDerivedStateFromError(): AppErrorBoundaryState {
    return { hasError: true };
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error("TalentAI frontend runtime error", error, info);
  }

  render() {
    if (this.state.hasError) {
      return (
        <main className="not-found">
          <div className="state-content">
            <span className="state-icon">
              <TriangleAlert size={26} aria-hidden="true" />
            </span>
            <h1>The application could not open this view</h1>
            <p>
              Refresh the page to retry. The technical error is available in
              the browser console for diagnosis.
            </p>
            <button
              className="app-button app-button--primary"
              type="button"
              onClick={() => window.location.reload()}
            >
              Refresh application
            </button>
          </div>
        </main>
      );
    }

    return this.props.children;
  }
}
