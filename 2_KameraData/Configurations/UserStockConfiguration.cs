using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class UserStockConfiguration : IEntityTypeConfiguration<UserStock>
    {
        public void Configure(EntityTypeBuilder<UserStock> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_user_stocks_id");
            entity.ToTable("user_stocks", tb => tb.UseSqlOutputClause(false));

            entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
            entity.HasIndex(e => e.StockId, "stock_id");
            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.PartNumber)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("part_number");

            entity.Property(e => e.PartsAndReplacesId)
                .HasColumnName("parts_and_replaces_id");

            entity.Property(e => e.Comment)
                .HasColumnName("Comment");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .HasColumnName("created_at");

            entity.Property(e => e.PhotoPath)
                .HasColumnName("photo_path");

            entity.Property(e => e.Qty)
                .HasDefaultValue(1)
                .HasColumnName("qty");

            entity.Property(e => e.PriceRaw)
                .HasMaxLength(100)
                .HasColumnName("price_raw");

            entity.Property(e => e.PriceValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price_value");

            entity.Property(e => e.UserPartName)
                .HasMaxLength(200)
                .HasColumnName("user_part_name");

            entity.Property(e => e.LinkUrl)
                .HasMaxLength(500)
                .HasColumnName("link_url");

            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");

            entity.Property(e => e.OwnerComment)
                .HasMaxLength(1000)
                .HasColumnName("owner_comment");

            entity.Property(e => e.Custom1)
                .HasMaxLength(500)
                .HasColumnName("custom_1");

            entity.Property(e => e.Custom2)
                .HasMaxLength(500)
                .HasColumnName("custom_2");

            entity.Property(e => e.Custom3)
                .HasMaxLength(500)
                .HasColumnName("custom_3");

            entity.Property(e => e.StockId)
                .IsRequired()
                .HasColumnName("stock_id");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            entity.HasOne(d => d.PartsAndReplaces)
                .WithMany(p => p.UserStocks)
                .HasForeignKey(d => d.PartsAndReplacesId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_stocks_ibfk_3");

            entity.HasOne(d => d.Stock)
                .WithMany(p => p.UserStocks)
                .HasForeignKey(d => d.StockId)
                .HasConstraintName("user_stocks_ibfk_2");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserStocks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_stocks_ibfk_1");
        }
    }
}
