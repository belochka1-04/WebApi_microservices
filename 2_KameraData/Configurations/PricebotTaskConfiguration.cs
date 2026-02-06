using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class PricebotTaskConfiguration : IEntityTypeConfiguration<PricebotTask>
    {
        public void Configure(EntityTypeBuilder<PricebotTask> entity)
        {
            entity.ToTable("pricebot_tasks");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.Property(e => e.PartNumber)
                .HasColumnName("part_number");

            entity.Property(e => e.ShopId)
                .HasColumnName("shop_id");

            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.Property(e => e.Price)
                .HasColumnName("price");

            entity.Property(e => e.ErrorMessage)
                .HasColumnName("error_message");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.Property(e => e.SitePartNumber)
                .HasColumnName("Site_part_number");

            entity.Property(e => e.PageLink)
                .HasColumnName("page_link");

            entity.Property(e => e.FoundParts)
                .HasColumnName("found_parts");

            entity.Property(e => e.Availability)
                .HasColumnName("availability");

            entity.Property(e => e.PartName)
                .HasColumnName("part_name");

            entity.Property(e => e.YourPrice)
                .HasColumnName("your_price");

            entity.Property(e => e.RegularPrice)
                .HasColumnName("regular_price");

            entity.Property(e => e.SpecialField1)
                .HasColumnName("special_field_1");

            entity.Property(e => e.SpecialField2)
                .HasColumnName("special_field_2");

            entity.Property(e => e.SpecialField3)
                .HasColumnName("special_field_3");
        }
    }
}
