export interface ChatMessageSendDto {
  id: number;
  userId: number;
  eventId: number;
  message: string;
  dateTime: Date;
}
