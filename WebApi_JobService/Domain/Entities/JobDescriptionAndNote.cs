// JobService/Domain/Entities/JobDescriptionAndNote.cs
namespace JobService.Domain.Entities;

public class JobDescriptionAndNote
{
    public int Id { get; set; }
    public int JobId { get; set; }

    public string? JobDescription { get; set; }
    public string? JobNotes { get; set; }
    public string? FullText { get; set; }

    public int? PicRecon { get; set; }
    public int? PicCounter { get; set; }

    public string? Status { get; set; }

    public string? OriginalBrand { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? StickerLink { get; set; }
    public string? OcrText { get; set; }

    public int? GoogleConfirmed { get; set; }
    public int? IsRatingPlatePercentage { get; set; }
    public decimal? CompletionCost { get; set; }
    public int? ImageTokens { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Бизнес-методы
    public void UpdateDescription(string newDescription)
    {
        JobDescription = newDescription;
        FullText = newDescription; // или объединять с JobNotes
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkGoogleConfirmed()
    {
        GoogleConfirmed = 1;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsConfirmedByGoogle() => GoogleConfirmed == 1;
}