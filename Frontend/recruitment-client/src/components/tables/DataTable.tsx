import type { ReactNode } from "react";
import EmptyState from "../common/EmptyState";

export interface TableColumn<T> {
  key: string;
  header: string;
  render: (item: T) => ReactNode;
  align?: "left" | "right";
}

interface DataTableProps<T> {
  columns: TableColumn<T>[];
  data: T[];
  getRowKey: (item: T) => string;
  emptyTitle?: string;
  emptyDescription?: string;
}

export default function DataTable<T>({
  columns,
  data,
  getRowKey,
  emptyTitle = "No records found",
  emptyDescription = "Records will appear here when they become available.",
}: DataTableProps<T>) {
  if (data.length === 0) {
    return (
      <EmptyState
        title={emptyTitle}
        description={emptyDescription}
      />
    );
  }

  return (
    <div className="data-table-wrap">
      <table className="data-table">
        <thead>
          <tr>
            {columns.map((column) => (
              <th
                key={column.key}
                className={column.align === "right" ? "text-right" : ""}
                scope="col"
              >
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data.map((item) => (
            <tr key={getRowKey(item)}>
              {columns.map((column) => (
                <td
                  key={column.key}
                  className={column.align === "right" ? "text-right" : ""}
                >
                  {column.render(item)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
