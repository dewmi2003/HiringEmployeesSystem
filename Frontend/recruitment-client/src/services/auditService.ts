import type { AuditLog } from "../models/audit";
import apiClient from "./apiClient";

export async function getAuditLogs(): Promise<AuditLog[]> {
  const { data } = await apiClient.get<AuditLog[]>("/auditlogs");
  return data;
}
