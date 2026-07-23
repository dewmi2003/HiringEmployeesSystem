import { ShieldX } from "lucide-react";
import { Link } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";
import { getRoleHome } from "../../utils/routePaths";

export default function UnauthorizedPage() {
  const { user } = useAuth();

  return (
    <main className="not-found">
      <div className="state-content">
        <span className="state-icon">
          <ShieldX size={26} aria-hidden="true" />
        </span>
        <h1>Access restricted</h1>
        <p>Your account does not have permission to open this page.</p>
        <Link
          className="app-button app-button--primary"
          to={user ? getRoleHome(user.role) : "/login"}
        >
          Return to dashboard
        </Link>
      </div>
    </main>
  );
}
