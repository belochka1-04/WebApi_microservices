using System.ComponentModel.DataAnnotations;

namespace WebApi_ModelService.Services;

public sealed class JobDocModelBatchRequest
{
    [Range(1, int.MaxValue)] public int JobId { get; set; }
    [Required, MinLength(1), MaxLength(32)] public List<JobDocModelSearch> Searches { get; set; } = new();
}

public sealed class JobDocModelSearch
{
    [Required, StringLength(255, MinimumLength = 1)] public string Prefix { get; set; } = string.Empty;
    [Range(0, int.MaxValue)] public int MinLen { get; set; }
    [Range(0, int.MaxValue)] public int MaxLen { get; set; }
}

public sealed record JobDocModelBatchResult(int SearchCount, int InsertedRows);
