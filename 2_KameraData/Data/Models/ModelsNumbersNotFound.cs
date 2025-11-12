using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class ModelsNumbersNotFound
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public string ModelNumber { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;
}
