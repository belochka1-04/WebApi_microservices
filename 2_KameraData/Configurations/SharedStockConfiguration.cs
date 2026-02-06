using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class SharedStockConfiguration : IEntityTypeConfiguration<SharedStock>
    {
        public void Configure(EntityTypeBuilder<SharedStock> entity)
        {
            entity.ToTable("shared_stocks", tb => tb.UseSqlOutputClause(false));
            entity.HasKey(e => new { e.UserId, e.StockId });

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.StockId)
                .HasColumnName("stock_id");

            entity.Property(e => e.GrantedByUserId)
                .HasColumnName("granted_by_user_id")
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");
        }
    }
}