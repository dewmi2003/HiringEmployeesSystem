import { useEffect, useState } from "react";
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
