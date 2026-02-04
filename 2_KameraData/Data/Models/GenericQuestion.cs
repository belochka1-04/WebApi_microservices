using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models
{
    [Table("generic_questions")]
    public partial class GenericQuestion
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("Question")]
        public string Question { get; set; } = null!;

        [Column("CreatedAt", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [InverseProperty(nameof(DocumentGenericQa.Question))]
        public virtual ICollection<DocumentGenericQa> DocumentGenericQas { get; set; } = [];
    }
}
