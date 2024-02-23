﻿namespace PartyCreatorWebApi.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
