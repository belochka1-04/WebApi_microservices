using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;
[Table("ModelDocument")]
public class ModelDocument
{
    // Идентификатор записи
    public int Id { get; set; }

    // Идентификатор модели
    public int? ModelId { get; set; } // Nullable, так как в таблице допускаются NULL

    // Идентификатор документа
    public int? DocumentId { get; set; } // Nullable, так как в таблице допускаются NULL

    // Навигационное свойство для связи с моделью (если необходимо)
    public virtual NModel Model { get; set; }

    // Навигационное свойство для связи с документом (если необходимо)
    public virtual Document Document { get; set; }
}

