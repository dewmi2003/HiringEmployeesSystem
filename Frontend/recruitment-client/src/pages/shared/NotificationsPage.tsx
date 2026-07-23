import { useCallback, useEffect, useState } from "react";
import { Bell, Check } from "lucide-react";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppLoader from "../../components/common/AppLoader";
import EmptyState from "../../components/common/EmptyState";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import { useAuth } from "../../hooks/useAuth";
import type { Notification } from "../../models/notification";
import {
  getUserNotifications,
  markNotificationRead,
} from "../../services/notificationService";
import { getErrorMessage } from "../../services/apiClient";
import { formatDateTime } from "../../utils/formatters";

export default function NotificationsPage() {
  const { user } = useAuth();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadNotifications = useCallback(async () => {
    if (!user?.userId) {
      setError("Your session does not include a user identifier.");
      setLoading(false);
      return;
    }

    setLoading(true);
    setError("");
    try {
      setNotifications(await getUserNotifications(user.userId));
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, [user?.userId]);

  useEffect(() => {
    void loadNotifications();
  }, [loadNotifications]);

  const markRead = async (notificationId: string) => {
    try {
      await markNotificationRead(notificationId);
      setNotifications((items) =>
        items.map((item) =>
          item.id === notificationId ? { ...item, isRead: true } : item,
        ),
      );
    } catch (markError) {
      setError(getErrorMessage(markError));
    }
  };

  return (
    <div className="animate-in">
      <PageHeader
        title="Notifications"
        description="Review important updates from your recruitment workflow."
      />
      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Loading notifications" />
          </div>
        ) : error ? (
          <ErrorState message={error} onRetry={loadNotifications} />
        ) : notifications.length === 0 ? (
          <EmptyState
            title="No notifications"
            description="New application, interview, and status updates will appear here."
            icon={Bell}
          />
        ) : (
          <div className="stack">
            {notifications.map((notification) => (
              <article className="list-row" key={notification.id}>
                <div>
                  <div className="inline-list">
                    <strong>{notification.title}</strong>
                    {!notification.isRead ? (
                      <AppBadge tone="primary">New</AppBadge>
                    ) : null}
                  </div>
                  <p>{notification.message}</p>
                  <span className="field-hint">
                    {formatDateTime(notification.createdAt)}
                  </span>
                </div>
                {!notification.isRead ? (
                  <AppButton
                    variant="ghost"
                    size="small"
                    icon={<Check size={16} aria-hidden="true" />}
                    onClick={() => void markRead(notification.id)}
                  >
                    Mark read
                  </AppButton>
                ) : null}
              </article>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
