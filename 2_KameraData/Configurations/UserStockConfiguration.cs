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