using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class JobStocksView
{
    public int? Id { get; set; }

    public int? JobId { get; set; }

    public string? MatchPartNumber { get; set; }

    public string? PartNumber { get; set; }

    public int? PartsAndReplacesId { get; set; }

    public string? StockName { get; set; }

    public string? Replaces { get; set; }

    public string? PartName { get; set; }

    public string? MainPartNumber { get; set; }

    public int? MnJobId { get; set; }

    public string? ModelNumber { get; set; }

    public string? NotFountModel { get; set; }

    public string? JobDescription { get; set; }

    public string? JobNotes { get; set; }

    public string? Token { get; set; }

    public string? ResponseText { get; set; }

    public int? PartId { get; set; }

    public int? UserStockId { get; set; }
}
