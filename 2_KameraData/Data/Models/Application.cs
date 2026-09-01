using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class Application
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? FileName { get; set; }

    public string? ProcessName { get; set; }
    public string Path { get; set; } = null!;

    public string Caption { get; set; } = null!;

    public int? Status { get; set; }

    public int Order { get; set; }

    public int? CanStartInStarter { get; set; }
    public DateTime? LastRunDate { get; set; }

    public virtual ICollection<ApplicationLog> ApplicationLogs { get; set; } = new List<ApplicationLog>();

    public DateTime? GetValidLastRunDate()
    {
        if (LastRunDate.HasValue && LastRunDate.Value.Year > 1900)
        {
            return LastRunDate; // Возвращаем только если дата валидная
        }
        return null; // Возвращаем null для невалидных дат
    }
}
