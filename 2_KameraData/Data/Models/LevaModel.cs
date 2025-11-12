using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class LevaModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int? BrandModelId { get; set; }
    public int? DocCounter { get; set; }

}
