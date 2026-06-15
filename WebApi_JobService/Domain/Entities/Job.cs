namespace JobService.Domain.Entities;

public class Job
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? TelegramId { get; set; }        // часто удобно держать здесь

    public string? JobNumber { get; set; }
    public string? JobLink { get; set; }
    public string? Token { get; set; }
    public string? UpdateRequestStatus { get; set; }
    public string Status { get; set; } = "Pending";

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Навигационные свойства в Domain-стиле (упрощённые)
    public JobDescriptionAndNote? Description { get; set; }
    //public UserInfo? User { get; set; }

    //public List<JobDocInfo> Documents { get; set; } = new();
    //public List<JobStockInfo> Stocks { get; set; } = new();
    //public List<ModelNumberInfo> ModelNumbers { get; set; } = new();

    // Бизнес-методы
    public bool IsPending() => Status == "Pending";
    public bool IsCompleted() => Status == "Completed" || Status == "Done";
    public bool CanBeModified() => Status is "Pending" or "InProgress";

    public void UpdateStatus(string newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}