export interface NotificationResponseDto {
  id: number;
  userId: number;
  description: string;
  type: string;
  isRead: boolean;
  eventId: number;
}
