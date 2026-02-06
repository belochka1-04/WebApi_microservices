using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class PartsAndReplaceConfiguration : IEntityTypeConfiguration<PartsAndReplace>
    {
        public void Configure(EntityTypeBuilder<PartsAndReplace> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_partsandreplacesid");
            entity.ToTable("partsandreplaces");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.GotForWorkAt)
                .HasColumnType("datetime")
                .HasColumnName("gotforworkat");

            entity.Property(e => e.MainPartNumber)
                .HasMaxLength(20)
                .HasColumnName("mainpartnumber");

            entity.Property(e => e.PartName)
                .HasMaxLength(70)
                .HasColumnName("partname");

            entity.Property(e => e.PicBase64)
                .HasColumnName("picbase64");

            entity.Property(e => e.Replaces)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("replaces");

            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");

            entity.Property(e => e.Brand)
                .HasColumnName("brand");

            entity.Property(e => e.Pic)
                .HasColumnName("pic");

            entity.Property(e => e.PicLink)
                .HasColumnName("piclink");

            entity.Property(e => e.PhotoStatus)
                .HasColumnName("Photostatus");

            entity.Property(e => e.DateUpdate)
                .HasColumnName("dateupdate");
        }
    }
}