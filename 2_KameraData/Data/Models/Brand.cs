using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;
[Table("Brand")]
public partial class Brand
{
    public int Id { get; set; }

    [Column("Title")]
    public string Title { get; set; } = string.Empty;
   
}
