using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KameraData.Data.Models;
[Table("Responses")]
public partial class Response
{
    [Column("id")] public int Id { get; set; }

    [Column("job_id")] public int JobId { get; set; }

    [Column("job_link")] public string? JobLink { get; set; }

    [Column("response_text")] public string ResponseText { get; set; } = null!;

    [Column("user_id")] public int? UserId { get; set; }

    [Column("status")] public string Status { get; set; } = null!;

    [Column("TG-status")] public string TgStatus { get; set; } = null!;

    [Column("Whatsapp-status")] public string WhatsappStatus { get; set; } = null!;

    public int? AttemptCount { get; set; }

    [Column("CRM_id")] public int? CrmId { get; set; }

    [JsonIgnore]
    public virtual Job? Job { get; set; } = null!;
}
