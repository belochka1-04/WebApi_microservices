using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KameraData.Data.Models;

[Table("Masks")]
public partial class Mask
{
    [Column("id")] public int Id { get; set; }

    [Column("user_id")][Required] public int UserId { get; set; }

    [Column("mask")][Required] public string mask { get; set; }

    //public virtual User User { get; set; } = null!;
}
