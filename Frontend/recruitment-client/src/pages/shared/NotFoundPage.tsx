import { Link } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";
import { getRoleHome } from "../../utils/routePaths";

export default function NotFoundPage() {
  const { user } = useAuth();

  return (
    <main className="not-found">
      <div className="state-content">
        <p className="not-found-code">404</p>
        <h1>Page not found</h1>
        <p>The page may have moved or the address may be incorrect.</p>
        <Link
          className="app-button app-button--primary"
          to={user ? getRoleHome(user.role) : "/login"}
        >
          Go to workspace
        </Link>
      </div>
    </main>
  );
}
