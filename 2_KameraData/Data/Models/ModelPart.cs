using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;
[Table("ModelPart")]
public class ModelPart
{
    // Идентификатор записи
    public int Id { get; set; }

    // Идентификатор модели
    public int ModelId { get; set; }

    // Идентификатор детали
    public int PartId { get; set; }

    // Навигационное свойство для связи с моделью (если необходимо)
    public virtual Model Model { get; set; }

    // Навигационное свойство для связи с деталью (если необходимо)
    public virtual Parts Part { get; set; }
}
