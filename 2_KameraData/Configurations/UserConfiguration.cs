using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_users_id");
            entity.ToTable("users", tb => tb.UseSqlOutputClause(false));

            entity.HasIndex(e => e.CrmId, "CRM_id");
            entity.HasIndex(e => e.TelegramId).IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.Login)
                .HasMaxLength(30)
                .HasColumnName("login")
                .HasDefaultValue(null);

            entity.Property(e => e.Password)
                .HasMaxLength(30)
                .HasColumnName("password")
                .HasDefaultValue(null);

            entity.Property(e => e.CrmId)
                .HasColumnName("CRM_id")
                .HasDefaultValue(null);

            entity.Property(e => e.CrmLogin)
                .HasMaxLength(30)
                .HasColumnName("CRM_login")
                .HasDefaultValue(null);

            entity.Property(e => e.CrmPassword)
                .HasMaxLength(30)
                .HasColumnName("CRM_password")
                .HasDefaultValue(null);

            entity.Property(e => e.Proxy)
                .HasMaxLength(60)
                .HasColumnName("proxy")
                .HasDefaultValue(null);

            entity.Property(e => e.SyncFreq)
                .HasMaxLength(5)
                .HasColumnName("sync_freq")
                .HasDefaultValue("10");

            entity.Property(e => e.SyncSwitch)
                .HasMaxLength(3)
                .HasColumnName("sync_switch")
                .HasDefaultValue("ON");

            entity.Property(e => e.UpdateStatus)
                .HasMaxLength(20)
                .HasColumnName("update_status")
                .HasDefaultValue(null);

            entity.Property(e => e.UpdateTime)
                .HasColumnName("update_time")
                .HasDefaultValue(null);

            entity.Property(e => e.ChatId)
                .HasColumnName("chat_id")
                .HasDefaultValue(null);

            entity.Property(e => e.Code)
                .HasColumnName("code")
                .HasDefaultValue(123);

            entity.Property(e => e.PrefersCrm)
                .HasMaxLength(1)
                .HasColumnName("prefers_crm")
                .HasDefaultValue("0");

            entity.Property(e => e.PrefersWhatsapp)
                .HasMaxLength(1)
                .HasColumnName("prefers_whatsapp")
                .HasDefaultValue("0");

            entity.Property(e => e.PrefersTelegram)
                .HasMaxLength(1)
                .HasColumnName("prefers_telegram")
                .HasDefaultValue("0");

            entity.Property(e => e.AllHistory)
                .HasMaxLength(255)
                .HasColumnName("all_history")
                .HasDefaultValue(null);

            entity.Property(e => e.TelegramId)
                .HasColumnName("telegram_id")
                .HasDefaultValue(null);

            entity.Property(e => e.TelegramState)
                .HasMaxLength(20)
                .HasColumnName("telegram_state")
                .HasDefaultValue(null);

            entity.Property(e => e.LastUpdatedDateTime)
                .HasColumnName("lastUpdatedDateTime")
                .HasColumnType("datetime")
                .HasDefaultValue(null);

            entity.Property(e => e.DiagramProbability)
                .HasColumnName("diagram_probability")
                .HasDefaultValue(50);

            entity.Property(e => e.ManualProbability)
                .HasColumnName("manual_probability")
                .HasDefaultValue(25);

            entity.Property(e => e.TgState)
                .HasColumnName("tg_state");

            entity.Property(e => e.ProUntil)
                .HasColumnName("pro_until")
                .HasColumnType("datetime")
                .HasDefaultValue(null);

            entity.Property(e => e.LastRepairVideoId)
                .HasColumnName("last_repair_video_id")
                .HasDefaultValue(null);

            entity.Property(e => e.LastWarehouseVideoId)
                .HasColumnName("last_warehouse_video_id")
                .HasDefaultValue(null);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .HasDefaultValue(null);

            entity.Property(e => e.LastVideoTipId)
                .HasColumnName("last_video_tip_id")
                .HasDefaultValue(null);

            entity.Property(e => e.LastLinkTipId)
                .HasColumnName("last_link_tip_id")
                .HasDefaultValue(null);

            entity.Property(e => e.LimitedUntil)
                .HasColumnName("limited_until")
                .HasDefaultValue(0);

            entity.Property(e => e.RequestsLimit)
                .HasColumnName("requests_limit")
                .HasDefaultValue(100);

            entity.Property(e => e.SupportThreadId)
                .HasColumnName("support_thread_id")
                .HasDefaultValue(null);

            entity.Property(e => e.Access)
                .HasColumnName("access")
                .HasDefaultValue(0);
        }
    }
}