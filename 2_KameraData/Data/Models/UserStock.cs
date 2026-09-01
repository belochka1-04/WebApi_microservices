using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class UserStock
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int StockId { get; set; }

    public string PartNumber { get; set; } = null!;

    public int? PartsAndReplacesId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? PhotoPath { get; set; }

    public int Qty { get; set; }

    public string? PriceRaw { get; set; }

    public decimal? PriceValue { get; set; }

    public string? UserPartName { get; set; }

    public string? LinkUrl { get; set; }

    public string? Description { get; set; }

    public string? OwnerComment { get; set; }

    public string? Custom1 { get; set; }

    public string? Custom2 { get; set; }

    public string? Custom3 { get; set; }

    public virtual ICollection<JobStock>? JobStocks { get; set; } = new List<JobStock>();

    public virtual PartsAndReplace? PartsAndReplaces { get; set; }

    public virtual StockCred? Stock { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
