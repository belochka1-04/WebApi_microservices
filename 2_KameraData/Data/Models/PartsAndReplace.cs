using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KameraData.Data.Models;

public partial class PartsAndReplace
{
    public int Id { get; set; }

    public string? Replaces { get; set; }

    [StringLength(140, ErrorMessage = "PartName cannot be longer than 140 characters.")]
    public string? PartName { get; set; }

    [StringLength(40, ErrorMessage = "MainPartNumber cannot be longer than 40 characters.")]
    public string? MainPartNumber { get; set; }

    [StringLength(2, ErrorMessage = "Status cannot be longer than 2 characters.")]
    public string? Status { get; set; }

    public DateTime? GotForWorkAt { get; set; } =null;

    public string? PicBase64 { get; set; }

    [StringLength(510, ErrorMessage = "Brand cannot be longer than 510 characters.")]
    public string? Brand { get; set; }
    public string? Pic { get; set; }
    public string? PicLink { get; set; }
    public int? PhotoStatus { get; set; }

    public DateTime DateUpdate { get; set; }

    public string? UserCreated { get; set; }

    public DateTime? DateCreated { get; set; }

    public int? RecordId { get; set; }

    [StringLength(40, ErrorMessage = "MainPartNumberNorm cannot be longer than 40 characters.")]
    public string? MainPartNumberNorm { get; set; }

    public string? PicUrl { get; set; }

    public int? OrderFind { get; set; }

    public string? PartsWorkerId { get; set; }

    public DateTime? PartsLeaseUntilUtc { get; set; }

    public int PartsAttemptCount { get; set; }

    public DateTime? PartsProcessingStartedAtUtc { get; set; }

    public DateTime? PartsProcessingCompletedAtUtc { get; set; }

    public string? PartsProcessingLastError { get; set; }

    public virtual ICollection<UserStock>? UserStocks { get; set; } = new List<UserStock>();
}
public enum Status
{
    Added = 0,
    Skip = 1,
    InWork = -1,
    Ready = 2,
    Error = 3
}
