export interface ChatMessageReceiveDto {
  id: number;
  userId: number;
  firstName: string;
  lastName: string;
  eventId: number;
  message: string;
  dateTime: Date;
}
