using System.ComponentModel.DataAnnotations;
namespace WebApi_JobService.Services;
public sealed class ModelCheckerLeaseRequest
{
    [Required, StringLength(128)] public string WorkerId { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int AttemptCount { get; set; }
    public int LeaseSeconds { get; set; } = 3600;
    [StringLength(2000)] public string? Error { get; set; }
}
public sealed class ModelCheckerRetryRequest
{
    [Required, StringLength(1000, MinimumLength = 1)] public string Reason { get; set; } = string.Empty;
}
