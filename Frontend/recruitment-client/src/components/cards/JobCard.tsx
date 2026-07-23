import { Building2, MapPin, WalletCards } from "lucide-react";
import { Link } from "react-router-dom";
import type { Job } from "../../models/job";
import { formatCurrency } from "../../utils/formatters";
import AppBadge from "../common/AppBadge";

interface JobCardProps {
  job: Job;
  detailsPath: string;
}

export default function JobCard({ job, detailsPath }: JobCardProps) {
  return (
    <article className="job-card">
      <div className="job-card-header">
        <h2>{job.title}</h2>
        <AppBadge>{job.status}</AppBadge>
      </div>
      <p>{job.companyName}</p>
      <div className="job-card-meta">
        <span>
          <MapPin size={14} aria-hidden="true" />
          {job.location || "Flexible location"}
        </span>
        <span>
          <WalletCards size={14} aria-hidden="true" />
          {formatCurrency(job.salary)}
        </span>
        <span>
          <Building2 size={14} aria-hidden="true" />
          {job.companyName}
        </span>
      </div>
      <div className="card-actions">
        <Link className="app-button app-button--secondary" to={detailsPath}>
          View details
        </Link>
      </div>
    </article>
  );
}
