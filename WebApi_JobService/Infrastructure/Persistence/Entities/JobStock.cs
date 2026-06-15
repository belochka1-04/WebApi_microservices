using System;
using System.Collections.Generic;

namespace WebApi_JobService.Infrastructure.Persistence.Entities;

public partial class JobStock
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public int UserStockId { get; set; }

    public string MatchPartNumber { get; set; } = null!;

    public virtual Job? Job { get; set; } = null!;

  
}
