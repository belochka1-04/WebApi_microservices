using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class ModelNumber
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public string ModelNumber1 { get; set; } = null!;
    public string? Brand { get; set; } = null!;

    public string MN_request { get; set; }

    public string Confirmed { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public int? BrandId { get; set; }

    public DateTime? GotForWorkAt { get; set; }

    public int? DocCounter { get; set; }

    public int? jdConfirmed { get; set; }

    public string? CleanedModel { get; set; } = null!;

    public string? WorkerId { get; set; }

    public DateTime? LeaseUntil { get; set; }

    public int AttemptCount { get; set; }

    public string? LastError { get; set; }

    public virtual Job Job { get; set; } = null!;


}
