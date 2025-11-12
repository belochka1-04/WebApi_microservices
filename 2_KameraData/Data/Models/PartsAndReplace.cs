using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KameraData.Data.Models;

public partial class PartsAndReplace
{
    public int Id { get; set; }

    public string? Replaces { get; set; }

    [StringLength(70, ErrorMessage = "PartName cannot be longer than 70 characters.")]
    public string? PartName { get; set; }

    [StringLength(20, ErrorMessage = "MainPartNumber cannot be longer than 20 characters.")]
    public string? MainPartNumber { get; set; }

    [StringLength(2, ErrorMessage = "Status cannot be longer than 2 characters.")]
    public string? Status { get; set; }

    public DateTime? GotForWorkAt { get; set; } =null;

    public string? PicBase64 { get; set; }

    [StringLength(255, ErrorMessage = "Status cannot be longer than 255 characters.")]
    public string? Brand { get; set; }
    public string? Pic { get; set; }
    public string? PicLink { get; set; }
    public int? PhotoStatus { get; set; }

    public DateTime DateUpdate { get; set; }

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
