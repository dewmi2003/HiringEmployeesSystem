import { Navigate, Outlet } from "react-router-dom";
import type { UserRole } from "../../models/auth";
import { useAuth } from "../../hooks/useAuth";
import { routes } from "../../utils/routePaths";

interface RoleRouteProps {
  allowedRoles: UserRole[];
}

export default function RoleRoute({ allowedRoles }: RoleRouteProps) {
  const { user } = useAuth();

  if (!user || !allowedRoles.includes(user.role)) {
    return <Navigate to={routes.unauthorized} replace />;
  }

  return <Outlet />;
}
