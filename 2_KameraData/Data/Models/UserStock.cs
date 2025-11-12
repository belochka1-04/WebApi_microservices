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

    public virtual ICollection<JobStock>? JobStocks { get; set; } = new List<JobStock>();

    public virtual PartsAndReplace? PartsAndReplaces { get; set; }

    public virtual StockCred? Stock { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
