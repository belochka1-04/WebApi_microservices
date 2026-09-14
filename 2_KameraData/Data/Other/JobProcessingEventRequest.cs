using System;

namespace KameraData.Data.Other
{
    public class JobProcessingEventRequest
    {
        public Guid? CorrelationId { get; set; }
        public int? JobId { get; set; }
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public string Component { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? ModelNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? WorkerId { get; set; }
        public string? PodName { get; set; }
        public int? DurationMs { get; set; }
        public string? ErrorMessage { get; set; }
        public string? DetailsJson { get; set; }
        public DateTime? CreatedAtUtc { get; set; }
    }
}
