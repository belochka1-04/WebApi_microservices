using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KameraData.Data.Models;

public partial class StockCred
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? StockName { get; set; }

    public string? Email { get; set; }

    public int? DocTypesId { get; set; }

    public string? StockLink { get; set; }

    public string? FileName { get; set; }

    public string? ExcelColumn { get; set; }

    public string? Partmanager { get; set; }

    public int ContactTypesId { get; set; }

    public string? ContactAddress { get; set; }

    public string? SyncFreq { get; set; }

    public string? SyncSwitch { get; set; }

    public string? UpdateStatus { get; set; }

    public DateTime? UpdateTime { get; set; }

    public int? RecordsCount { get; set; }

    public int? ReplaceRecords { get; set; }

    public int? MaxRowsPerUpload { get; set; }

    public int? IsDefault { get; set; }

    public Guid? SyncLockId { get; set; }

    public string? SyncLockedBy { get; set; }

    public DateTime? SyncLeaseUntil { get; set; }

    public int SyncAttemptCount { get; set; }

    public string? SyncLastError { get; set; }

    public virtual User? User { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<UserStock> UserStocks { get; set; } = new List<UserStock>();
}
