using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class JobDocInfo
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public int ModelsNumberId { get; set; }
    public int SourceId { get; set; }

    public string? LocalPath { get; set; }

    public int ModelId { get; set; }
}
