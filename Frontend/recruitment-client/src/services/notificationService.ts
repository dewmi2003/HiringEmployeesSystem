import type { Notification } from "../models/notification";
import apiClient from "./apiClient";

export async function getUserNotifications(
  userId: string,
): Promise<Notification[]> {
  const { data } = await apiClient.get<Notification[]>(
    `/notifications/${userId}`,
  );
  return data;
}

export async function markNotificationRead(
  notificationId: string,
): Promise<void> {
  await apiClient.put(`/notifications/${notificationId}/read`);
}

export async function getAllNotifications(): Promise<Notification[]> {
  const { data } = await apiClient.get<Notification[]>("/notifications");
  return data;
}
