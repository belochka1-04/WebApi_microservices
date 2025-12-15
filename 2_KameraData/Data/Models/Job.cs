using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class Job
{
    public int Id { get; set; }

    public int? UserId { get; set; }  // Может быть null- изначально нельзя
    public int TelegramId { get; set; }  // Новый обязательный FK

    public string? JobNumber { get; set; }

    public string? JobLink { get; set; }

    public string? Token { get; set; }

    public string? UpdateRequestStatus { get; set; }
    public string Status { get; set; } = "Pending";  //new Pending/Linked/Failed

    public virtual JobDescriptionAndNote? JobDescriptionAndNote { get; set; }

    public virtual ICollection<JobDoc> JobDocs { get; set; } = new List<JobDoc>();

    public virtual ICollection<JobStock> JobStocks { get; set; } = new List<JobStock>();

    public virtual ICollection<ModelNumber> ModelNumbers { get; set; } = new List<ModelNumber>();

    public virtual ICollection<ModelsNumbersNotFound> ModelsNumbersNotFounds { get; set; } = new List<ModelsNumbersNotFound>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual User User { get; set; } = null!;
}
