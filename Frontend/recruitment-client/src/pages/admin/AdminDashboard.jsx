import { useEffect, useState } from "react";
import api from "../../services/api";

function AdminDashboard() {
  const [summary, setSummary] = useState(null);

  useEffect(() => {
    Promise.allSettled([
      api.get("/recruiter-dashboard/statistics"),
      api.get("/analytics/hiring-trends"),
      api.get("/reports/recruitment")
    ]).then((results) => {
      setSummary(results.map((result) => (result.status === "fulfilled" ? result.value.data : null)));
    });
  }, []);

  return (
    <div>
      <h1>
        Admin Dashboard
      </h1>

      <pre>{JSON.stringify(summary, null, 2)}</pre>
    </div>
  );
}

export default AdminDashboard;
