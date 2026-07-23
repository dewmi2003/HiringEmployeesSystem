import { Navigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "../context/AuthContext";


function ProtectedRoute({ children, allowedRoles }) {

  const { user } = useContext(AuthContext);


  // Not logged in
  if (!user) {
    return <Navigate to="/login" />;
  }


  // Role checking
  if (
    allowedRoles &&
    !allowedRoles.includes(user.role)
  ) {
    return <Navigate to="/unauthorized" />;
  }


  return children;
}


export default ProtectedRoute;
