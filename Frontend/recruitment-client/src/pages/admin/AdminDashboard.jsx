import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
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

      <div className="mt-4 flex flex-wrap gap-3">
        <Link to="/admin/users" className="bg-teal-700 text-white px-4 py-2 rounded-xl">
          Users
        </Link>
        <Link to="/admin/reports" className="bg-teal-100 text-teal-800 px-4 py-2 rounded-xl">
          Reports
        </Link>
        <Link to="/manager/dashboard" className="bg-gray-100 text-gray-800 px-4 py-2 rounded-xl">
          Manager View
        </Link>
      </div>

      <pre>{JSON.stringify(summary, null, 2)}</pre>
    </div>
  );
}

export default AdminDashboard;
