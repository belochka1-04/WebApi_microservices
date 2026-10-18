namespace WebApi_StockCredsService.Services;

public sealed class StockSyncLeaseRequest
{
    public string WorkerId { get; set; } = "";
    public Guid LockId { get; set; }
    public int LeaseSeconds { get; set; } = 900;
    public bool Success { get; set; }
    public string? Error { get; set; }
    public int RetryDelaySeconds { get; set; } = 30;
}

public sealed class StockSyncStateRequest
{
    public DateTime? UpdateTime { get; set; }
    public int? RecordsCount { get; set; }
    public string? SyncSwitch { get; set; }
}
