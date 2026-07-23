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
import CandidateProfile from "../pages/candidate/CandidateProfile";
import ResumeUpload from "../pages/candidate/ResumeUpload";
import Skills from "../pages/candidate/Skills";
import Applications from "../pages/candidate/Applications";
import Jobs from "../pages/jobs/Jobs";
import JobDetails from "../pages/jobs/JobDetails";
import RecruiterJobs from "../pages/recruiter/RecruiterJobs";
import CreateJob from "../pages/recruiter/CreateJob";
import CompanyProfile from "../pages/recruiter/CompanyProfile";
import CandidateEvaluation from "../pages/recruiter/CandidateEvaluation";
import Applicants from "../pages/recruiter/Applicants";
import AdminUsers from "../pages/admin/AdminUsers";
import AdminReports from "../pages/admin/AdminReports";
import ResumeAnalysis from "../pages/ai/ResumeAnalysis";
import JobMatching from "../pages/ai/JobMatching";
import CandidateRanking from "../pages/ai/CandidateRanking";
import InterviewList from "../pages/interviews/InterviewList";
import ScheduleInterview from "../pages/interviews/ScheduleInterview";
import InterviewDetails from "../pages/interviews/InterviewDetails";

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

        <Route
          path="/candidate/profile"
          element={
            <ProtectedRoute allowedRoles={["Candidate"]}>
              <MainLayout>
                <CandidateProfile />
              </MainLayout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/candidate/resume"
          element={
            <ProtectedRoute allowedRoles={["Candidate"]}>
              <MainLayout>
                <ResumeUpload />
              </MainLayout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/candidate/skills"
          element={
            <ProtectedRoute allowedRoles={["Candidate"]}>
              <MainLayout>
                <Skills />
              </MainLayout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/candidate/applications"
          element={
            <ProtectedRoute allowedRoles={["Candidate"]}>
              <MainLayout>
                <Applications />
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

<Route
  path="/recruiter/jobs"
  element={
    <ProtectedRoute allowedRoles={["Recruiter"]}>
      <MainLayout>
        <RecruiterJobs />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/recruiter/jobs/create"
  element={
    <ProtectedRoute allowedRoles={["Recruiter"]}>
      <MainLayout>
        <CreateJob />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/recruiter/company"
  element={
    <ProtectedRoute allowedRoles={["Recruiter"]}>
      <MainLayout>
        <CompanyProfile />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/recruiter/evaluation"
  element={
    <ProtectedRoute allowedRoles={["Recruiter"]}>
      <MainLayout>
        <CandidateEvaluation />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/recruiter/applicants"
  element={
    <ProtectedRoute allowedRoles={["Recruiter"]}>
      <MainLayout>
        <Applicants />
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

<Route
  path="/admin/users"
  element={
    <ProtectedRoute allowedRoles={["Admin"]}>
      <MainLayout>
        <AdminUsers />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/admin/reports"
  element={
    <ProtectedRoute allowedRoles={["Admin"]}>
      <MainLayout>
        <AdminReports />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/jobs"
  element={
    <ProtectedRoute allowedRoles={["Candidate","Recruiter","Admin"]}>
      <MainLayout>
        <Jobs />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/jobs/:id"
  element={
    <ProtectedRoute allowedRoles={["Candidate","Recruiter","Admin"]}>
      <MainLayout>
        <JobDetails />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/interviews"
  element={
    <ProtectedRoute allowedRoles={["Recruiter","Admin"]}>
      <MainLayout>
        <InterviewList />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/interviews/schedule"
  element={
    <ProtectedRoute allowedRoles={["Recruiter","Admin"]}>
      <MainLayout>
        <ScheduleInterview />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/interviews/:id"
  element={
    <ProtectedRoute allowedRoles={["Recruiter","Admin"]}>
      <MainLayout>
        <InterviewDetails />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/ai/resume-analysis"
  element={
    <ProtectedRoute allowedRoles={["Candidate","Recruiter","Admin"]}>
      <MainLayout>
        <ResumeAnalysis />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/ai/job-matching"
  element={
    <ProtectedRoute allowedRoles={["Candidate","Recruiter","Admin"]}>
      <MainLayout>
        <JobMatching />
      </MainLayout>
    </ProtectedRoute>
  }
/>

<Route
  path="/ai/candidate-ranking"
  element={
    <ProtectedRoute allowedRoles={["Recruiter","Admin"]}>
      <MainLayout>
        <CandidateRanking />
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
