export interface NotificationDto {
  id: number;
  userId: number;
  description: string;
  type: string;
  isRead: boolean;
  eventId: number;
  eventTitle: string;
}
