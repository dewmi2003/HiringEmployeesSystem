import { BrowserRouter, Routes, Route } from "react-router-dom";


// testing only
import AuthTest from "../components/AuthTest";
import ProtectedRoute from "../components/ProtectedRoute";
import Unauthorized from "../pages/Unauthorized";
import Login from "../pages/auth/Login";
import Register from "../pages/auth/Register";
import MainLayout from "../layouts/MainLayout";


import CandidateDashboard from "../pages/candidate/CandidateDashboard";
import RecruiterDashboard from "../pages/recruiter/RecruiterDashboard";
import ManagerDashboard from "../pages/manager/ManagerDashboard";
import AdminDashboard from "../pages/admin/AdminDashboard";

function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>

        {/* Public Routes */}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* Role Based Dashboards */}
       <Route
 path="/candidate/dashboard"
 element={
   <ProtectedRoute allowedRoles={["Candidate"]}>
     <MainLayout>
       <CandidateDashboard />
     </MainLayout>
   </ProtectedRoute>
 }
/>

{/* Recruiter */}
<Route
  path="/recruiter/dashboard"
  element={
    <ProtectedRoute allowedRoles={["Recruiter"]}>
      <MainLayout>
        <RecruiterDashboard />
      </MainLayout>
    </ProtectedRoute>
  }
/>


{/* Manager */}
<Route
  path="/manager/dashboard"
  element={
    <ProtectedRoute allowedRoles={["Manager"]}>
      <MainLayout>
        <ManagerDashboard />
      </MainLayout>
    </ProtectedRoute>
  }
/>


{/* Admin */}
<Route
  path="/admin/dashboard"
  element={
    <ProtectedRoute allowedRoles={["Admin"]}>
      <MainLayout>
        <AdminDashboard />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route 
 path="/unauthorized" 
 element={<Unauthorized />} 
/>

  <Route 
  path="/auth-test" 
  element={<AuthTest />} 
/>

      </Routes>
    </BrowserRouter>
  );
}

export default AppRoutes;