using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

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

    public int? TelegramId { get; set; }

    public string? TelegramState { get; set; }

    public int? ChatId { get; set; }
    public int? DiagramProbability { get; set; }
    public int? ManualProbability { get; set; }

    public byte? TgState { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual ICollection<Mask> Masks { get; set; } = new List<Mask>();

    public virtual ICollection<StockCred> StockCreds { get; set; } = new List<StockCred>();

    public virtual ICollection<UserStock> UserStocks { get; set; } = new List<UserStock>();
}
