export interface AuditLog {
  id: string;
  userId: string | null;
  action: string;
  entityName: string;
  ipAddress: string;
  createdAt: string;
}
