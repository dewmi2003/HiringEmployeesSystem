import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getJobs } from "../../services/jobService";

function RecruiterDashboard() {
  const [jobs, setJobs] = useState([]);

  useEffect(() => {
    getJobs()
      .then((items) => setJobs(items || []))
      .catch(() => {});
  }, []);

  return (
    <div>
      <h1>
        Recruiter Dashboard
      </h1>

      <div className="mt-4 flex flex-wrap gap-3">
        <Link to="/recruiter/jobs" className="bg-teal-700 text-white px-4 py-2 rounded-xl">
          Manage Jobs
        </Link>
        <Link to="/recruiter/jobs/create" className="bg-teal-100 text-teal-800 px-4 py-2 rounded-xl">
          Create Job
        </Link>
        <Link to="/recruiter/applicants" className="bg-gray-100 text-gray-800 px-4 py-2 rounded-xl">
          Applicants
        </Link>
      </div>

      <div className="mt-6">
        {jobs.length > 0 ? (
          <ul className="space-y-2">
            {jobs.slice(0, 5).map((job) => (
              <li key={job.id} className="border rounded p-3">
                <strong>{job.title}</strong>
                <div>{job.location || "Location not set"}</div>
              </li>
            ))}
          </ul>
        ) : (
          <p>No jobs loaded yet.</p>
        )}
      </div>
    </div>
  );
}

export default RecruiterDashboard;
