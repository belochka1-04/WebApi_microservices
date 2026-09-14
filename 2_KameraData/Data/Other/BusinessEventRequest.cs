using System;

namespace KameraData.Data.Other
{
    public class BusinessEventRequest
    {
        public Guid? EventId { get; set; }
        public Guid? SessionId { get; set; }
        public int? UserId { get; set; }
        public long? TelegramUserId { get; set; }
        public int? JobId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public string? Source { get; set; }
        public string? Feature { get; set; }
        public string? Status { get; set; }
        public decimal? NumericValue { get; set; }
        public int? DurationMs { get; set; }
        public string? MetadataJson { get; set; }
        public DateTime? OccurredAtUtc { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
