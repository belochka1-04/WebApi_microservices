using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models;

public partial class StopWords
{
    public int UserId { get; set; }

    public int Id { get; set; }

    public string? Word { get; set; }
}
