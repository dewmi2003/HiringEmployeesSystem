import { useCallback, useEffect, useState } from "react";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type { AuditLog } from "../../models/audit";
import { getErrorMessage } from "../../services/apiClient";
import { getAuditLogs } from "../../services/auditService";
import { formatDateTime } from "../../utils/formatters";

export default function AuditLogsPage() {
  const [logs, setLogs] = useState<AuditLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadLogs = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setLogs(await getAuditLogs());
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadLogs();
  }, [loadLogs]);

  const columns: TableColumn<AuditLog>[] = [
    {
      key: "action",
      header: "Action",
      render: (log) => <span className="table-primary">{log.action}</span>,
    },
    {
      key: "entity",
      header: "Entity",
      render: (log) => log.entityName,
    },
    {
      key: "user",
      header: "User ID",
      render: (log) => log.userId || "System",
    },
    {
      key: "ip",
      header: "IP address",
      render: (log) => log.ipAddress || "Not recorded",
    },
    {
      key: "date",
      header: "Created",
      render: (log) => formatDateTime(log.createdAt),
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="Audit logs"
        description="Review recorded platform activity for operational oversight."
      />
      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Loading audit activity" />
          </div>
        ) : error ? (
          <ErrorState message={error} onRetry={loadLogs} />
        ) : (
          <DataTable
            columns={columns}
            data={logs}
            getRowKey={(log) => log.id}
            emptyTitle="No audit activity"
            emptyDescription="Recorded system activity will appear here."
          />
        )}
      </section>
    </div>
  );
}
