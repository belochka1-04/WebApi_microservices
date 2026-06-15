using System;
using System.Collections.Generic;

namespace WebApi_JobService.Infrastructure.Persistence.Entities;

public partial class User
{
    public int Id { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public int? CrmId { get; set; }

    public string? CrmLogin { get; set; }

    public string? CrmPassword { get; set; }

    public string? Proxy { get; set; }

    public string SyncFreq { get; set; } = null!;

    public string SyncSwitch { get; set; } = null!;

    public string? UpdateStatus { get; set; }

    public long? UpdateTime { get; set; }

    public int Code { get; set; }

    public string PrefersCrm { get; set; } = null!;

    public string PrefersWhatsapp { get; set; } = null!;

    public string PrefersTelegram { get; set; } = null!;

    public string? AllHistory { get; set; }

    public long? TelegramId { get; set; }

    public string? TelegramState { get; set; }

    public int? ChatId { get; set; }
    public int? DiagramProbability { get; set; }
    public int? ManualProbability { get; set; }

    public byte? TgState { get; set; }

    public DateTime? ProUntil { get; set; }
    public DateTime? LastUpdatedDateTime { get; set; }

    public int? LastVideoTipId { get; set; }
    public int? LastLinkTipId { get; set; }
    public int? LastRepairVideoId { get; set; }
    public int? LastWarehouseVideoId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int LimitedUntil { get; set; }       // NOT NULL
    public int RequestsLimit { get; set; }      // NOT NULL

    public long? SupportThreadId { get; set; }  // bigint -> long
    public byte Access { get; set; }            // tinyint -> byte
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

  
}
