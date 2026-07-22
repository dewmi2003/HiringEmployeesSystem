import { Navigate, Route, Routes, Link, useLocation, useParams } from "react-router-dom";

type Row = Record<string, string>;

function Shell({
  title,
  description,
  children
}: {
  title: string;
  description?: string;
  children?: React.ReactNode;
}) {
  const location = useLocation();
  const links = [
    ["/login", "Login"],
    ["/register", "Register"],
    ["/admin", "Admin"],
    ["/candidate", "Candidate"],
    ["/recruiter", "Recruiter"],
    ["/jobs", "Jobs"],
    ["/applications", "Applications"],
    ["/interviews", "Interviews"],
    ["/ai/resume-analysis", "AI Resume"],
    ["/ai/job-matching", "AI Match"],
    ["/ai/candidate-ranking", "AI Rank"]
  ] as const;

  return (
    <div>
      <header>
        <strong>TalentAI Recruitment</strong>
        <div>{location.pathname}</div>
      </header>
      <nav>
        {links.map(([to, label]) => (
          <Link key={to} to={to}>{label}</Link>
        ))}
      </nav>
      <main>
        <h1>{title}</h1>
        {description ? <p>{description}</p> : null}
        {children}
      </main>
    </div>
  );
}

function Section({ title, children }: { title: string; children?: React.ReactNode }) {
  return (
    <section>
      <h2>{title}</h2>
      {children}
    </section>
  );
}

function FormField({ label, type = "text", textarea = false }: { label: string; type?: string; textarea?: boolean }) {
  return (
    <label>
      {label}
      <br />
      {textarea ? <textarea rows={4} /> : <input type={type} />}
    </label>
  );
}

function DataTable({ columns, rows }: { columns: string[]; rows: Row[] }) {
  return (
    <table>
      <thead>
        <tr>
          {columns.map((column) => (
            <th key={column}>{column}</th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rows.length > 0 ? (
          rows.map((row, index) => (
            <tr key={index}>
              {columns.map((column) => (
                <td key={column}>{row[column] ?? "-"}</td>
              ))}
            </tr>
          ))
        ) : (
          <tr>
            <td colSpan={columns.length}>No rows yet</td>
          </tr>
        )}
      </tbody>
    </table>
  );
}

function LoginPage() {
  return (
    <Shell title="Login" description="Sign in to the recruitment system.">
      <form>
        <FormField label="Email" type="email" />
        <FormField label="Password" type="password" />
        <label>
          <input type="checkbox" /> Remember me
        </label>
        <button type="submit">Login</button>
      </form>
      <p>
        <Link to="/forgot-password">Forgot password</Link> | <Link to="/register">Register</Link>
      </p>
    </Shell>
  );
}

function RegisterPage() {
  return (
    <Shell title="Register" description="Create a candidate or recruiter account.">
      <form>
        <FormField label="Full name" />
        <FormField label="Email" type="email" />
        <FormField label="Password" type="password" />
        <label>
          Role
          <br />
          <select>
            <option>Candidate</option>
            <option>Recruiter</option>
          </select>
        </label>
        <button type="submit">Register</button>
      </form>
    </Shell>
  );
}

function ForgotPasswordPage() {
  return (
    <Shell title="Forgot Password" description="Request a reset email.">
      <form>
        <FormField label="Email" type="email" />
        <button type="submit">Send reset link</button>
      </form>
    </Shell>
  );
}

function ResetPasswordPage() {
  return (
    <Shell title="Reset Password" description="Set a new password using the reset token.">
      <form>
        <FormField label="Reset token" />
        <FormField label="New password" type="password" />
        <button type="submit">Update password</button>
      </form>
    </Shell>
  );
}

function AdminDashboardPage() {
  return (
    <Shell title="Admin Dashboard" description="Overview of the recruitment platform.">
      <DataTable
        columns={["Metric", "Value"]}
        rows={[
          { Metric: "Users", Value: "Load from API" },
          { Metric: "Jobs", Value: "Load from API" },
          { Metric: "Applications", Value: "Load from API" },
          { Metric: "Interviews", Value: "Load from API" }
        ]}
      />
    </Shell>
  );
}

function UserManagementPage() {
  return (
    <Shell title="User Management" description="Search, view, and manage users.">
      <Section title="Search">
        <FormField label="Search users" />
        <button type="button">Search</button>
      </Section>
      <Section title="Users">
        <DataTable columns={["Name", "Email", "Role", "Status"]} rows={[]} />
      </Section>
    </Shell>
  );
}

function ReportsPage() {
  return (
    <Shell title="Reports" description="Generate summary reports for the hiring process.">
      <button type="button">Generate report</button>
      <DataTable columns={["Report", "Generated", "Action"]} rows={[]} />
    </Shell>
  );
}

function CandidateDashboardPage() {
  return (
    <Shell title="Candidate Dashboard" description="Track profile completion, resume score, and applications.">
      <DataTable
        columns={["Item", "Status"]}
        rows={[
          { Item: "Profile", Status: "Load from API" },
          { Item: "Resume", Status: "Load from API" },
          { Item: "Applications", Status: "Load from API" },
          { Item: "Interviews", Status: "Load from API" }
        ]}
      />
    </Shell>
  );
}

function CandidateProfilePage() {
  return (
    <Shell title="Candidate Profile" description="Basic profile details for the candidate.">
      <form>
        <FormField label="First name" />
        <FormField label="Last name" />
        <FormField label="Phone" />
        <FormField label="Address" textarea />
        <FormField label="Bio" textarea />
        <FormField label="Experience" textarea />
        <FormField label="Education" textarea />
        <button type="submit">Save profile</button>
      </form>
    </Shell>
  );
}

function ResumeUploadPage() {
  return (
    <Shell title="Resume Upload" description="Upload a CV and view parsed text and ATS score.">
      <form encType="multipart/form-data">
        <label>
          Resume file
          <br />
          <input type="file" accept=".pdf,.doc,.docx,.txt" />
        </label>
        <button type="submit">Upload resume</button>
      </form>
      <Section title="Latest Resume">
        <DataTable columns={["File", "Score", "Version", "Status"]} rows={[]} />
      </Section>
      <Section title="Parsed Text">
        <textarea rows={8} readOnly />
      </Section>
    </Shell>
  );
}

function SkillsPage() {
  return (
    <Shell title="Skills" description="Maintain candidate skills for filtering and matching.">
      <form>
        <FormField label="Skill name" />
        <button type="submit">Add skill</button>
      </form>
      <DataTable columns={["Skill", "Level", "Action"]} rows={[]} />
    </Shell>
  );
}

function ApplicationsPage() {
  return (
    <Shell title="Applications" description="Candidate application history and statuses.">
      <DataTable columns={["Job", "Status", "Applied", "Action"]} rows={[]} />
    </Shell>
  );
}

function RecruiterDashboardPage() {
  return (
    <Shell title="Recruiter Dashboard" description="Monitor jobs, applicants, and hiring activity.">
      <DataTable
        columns={["Metric", "Value"]}
        rows={[
          { Metric: "Open jobs", Value: "Load from API" },
          { Metric: "Applicants", Value: "Load from API" },
          { Metric: "Interviews", Value: "Load from API" },
          { Metric: "Offers", Value: "Load from API" }
        ]}
      />
    </Shell>
  );
}

function ManageJobsPage() {
  return (
    <Shell title="Manage Jobs" description="List, search, edit, and close job posts.">
      <Section title="Search">
        <FormField label="Search jobs" />
        <button type="button">Search</button>
      </Section>
      <Section title="Jobs">
        <DataTable columns={["Title", "Department", "Status", "Action"]} rows={[]} />
      </Section>
    </Shell>
  );
}

function CreateJobPage() {
  return (
    <Shell title="Create Job" description="Create a new role for recruitment.">
      <form>
        <FormField label="Title" />
        <FormField label="Department" />
        <FormField label="Location" />
        <FormField label="Requirements" textarea />
        <FormField label="Description" textarea />
        <button type="submit">Save job</button>
      </form>
    </Shell>
  );
}

function CompanyProfilePage() {
  return (
    <Shell title="Company Profile" description="Company information shown on job posts.">
      <form>
        <FormField label="Company name" />
        <FormField label="Website" />
        <FormField label="Description" textarea />
        <FormField label="Address" textarea />
        <button type="submit">Save company</button>
      </form>
    </Shell>
  );
}

function CandidateEvaluationPage() {
  return (
    <Shell title="Candidate Evaluation" description="Review candidate ratings and notes.">
      <DataTable columns={["Candidate", "Score", "Recommendation", "Action"]} rows={[]} />
    </Shell>
  );
}

function ApplicantsPage() {
  return (
    <Shell title="Applicants" description="Candidates for a selected job.">
      <DataTable columns={["Candidate", "Job", "Status", "Action"]} rows={[]} />
    </Shell>
  );
}

function SearchJobsPage() {
  return (
    <Shell title="Search Jobs" description="Find jobs that match a candidate profile.">
      <form>
        <FormField label="Keyword" />
        <FormField label="Location" />
        <button type="submit">Search</button>
      </form>
    </Shell>
  );
}

function JobListPage() {
  return (
    <Shell title="Jobs" description="All open and closed jobs.">
      <DataTable columns={["Title", "Department", "Location", "Status"]} rows={[]} />
    </Shell>
  );
}

function JobDetailsPage() {
  const { id } = useParams();
  return (
    <Shell title="Job Details" description={`Job ID: ${id ?? "unknown"}`}>
      <Section title="Overview">
        <p>Job details load here.</p>
      </Section>
      <Section title="Applicants">
        <DataTable columns={["Candidate", "Score", "Status"]} rows={[]} />
      </Section>
    </Shell>
  );
}

function InterviewListPage() {
  return (
    <Shell title="Interviews" description="Upcoming and completed interviews.">
      <DataTable columns={["Candidate", "Job", "Date", "Status"]} rows={[]} />
    </Shell>
  );
}

function ScheduleInterviewPage() {
  return (
    <Shell title="Schedule Interview" description="Create a new interview and connect it to calendar/email.">
      <form>
        <FormField label="Candidate ID" />
        <FormField label="Job ID" />
        <FormField label="Date and time" type="datetime-local" />
        <FormField label="Location" />
        <FormField label="Notes" textarea />
        <button type="submit">Schedule</button>
      </form>
    </Shell>
  );
}

function InterviewDetailsPage() {
  const { id } = useParams();
  return (
    <Shell title="Interview Details" description={`Interview ID: ${id ?? "unknown"}`}>
      <DataTable columns={["Field", "Value"]} rows={[]} />
    </Shell>
  );
}

function ResumeAnalysisPage() {
  return (
    <Shell title="AI Resume Analysis" description="Analyze a CV against ATS and job criteria.">
      <form>
        <FormField label="Resume text" textarea />
        <FormField label="Job description" textarea />
        <FormField label="Candidate name" />
        <button type="submit">Analyze</button>
      </form>
      <Section title="Results">
        <DataTable columns={["Field", "Value"]} rows={[]} />
      </Section>
    </Shell>
  );
}

function JobMatchingPage() {
  return (
    <Shell title="AI Job Matching" description="Compare a resume to a job description.">
      <form>
        <FormField label="Resume text" textarea />
        <FormField label="Job description" textarea />
        <FormField label="Job title" />
        <button type="submit">Match</button>
      </form>
    </Shell>
  );
}

function CandidateRankingPage() {
  return (
    <Shell title="AI Candidate Ranking" description="Rank applicants for a role.">
      <form>
        <FormField label="Job description" textarea />
        <FormField label="Job title" />
        <button type="submit">Rank candidates</button>
      </form>
      <DataTable columns={["Rank", "Candidate", "Score", "Recommendation"]} rows={[]} />
    </Shell>
  );
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} />
      <Route path="/admin" element={<AdminDashboardPage />} />
      <Route path="/admin/users" element={<UserManagementPage />} />
      <Route path="/admin/reports" element={<ReportsPage />} />
      <Route path="/candidate" element={<CandidateDashboardPage />} />
      <Route path="/candidate/profile" element={<CandidateProfilePage />} />
      <Route path="/candidate/resume" element={<ResumeUploadPage />} />
      <Route path="/candidate/skills" element={<SkillsPage />} />
      <Route path="/candidate/applications" element={<ApplicationsPage />} />
      <Route path="/recruiter" element={<RecruiterDashboardPage />} />
      <Route path="/recruiter/jobs" element={<ManageJobsPage />} />
      <Route path="/recruiter/jobs/create" element={<CreateJobPage />} />
      <Route path="/recruiter/company" element={<CompanyProfilePage />} />
      <Route path="/recruiter/evaluation" element={<CandidateEvaluationPage />} />
      <Route path="/recruiter/applicants" element={<ApplicantsPage />} />
      <Route path="/jobs/search" element={<SearchJobsPage />} />
      <Route path="/jobs" element={<JobListPage />} />
      <Route path="/jobs/:id" element={<JobDetailsPage />} />
      <Route path="/interviews" element={<InterviewListPage />} />
      <Route path="/interviews/schedule" element={<ScheduleInterviewPage />} />
      <Route path="/interviews/:id" element={<InterviewDetailsPage />} />
      <Route path="/ai/resume-analysis" element={<ResumeAnalysisPage />} />
      <Route path="/ai/job-matching" element={<JobMatchingPage />} />
      <Route path="/ai/candidate-ranking" element={<CandidateRankingPage />} />
    </Routes>
  );
}
