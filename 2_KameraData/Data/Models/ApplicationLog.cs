using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KameraData.Data.Models;

public partial class ApplicationLog
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public DateTime RecDate { get; set; }

    public string? RecType { get; set; } 
    public string? Text { get; set; }

    public string? Description { get; set; }

    [JsonIgnore]
    public virtual Application? Application { get; set; } = null!;
}
