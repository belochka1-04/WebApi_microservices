using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class JobStock
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public int UserStockId { get; set; }

    public string MatchPartNumber { get; set; } = null!;

    public virtual Job? Job { get; set; } = null!;

    public virtual UserStock? UserStock { get; set; } = null!;
}
