using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class UniqModelConfiguration : IEntityTypeConfiguration<UniqModel>
    {
        public void Configure(EntityTypeBuilder<UniqModel> entity)
        {
            entity.ToTable("UniqModel", tb => tb.UseSqlOutputClause(false));

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.Title)
                .HasColumnName("Title");

            entity.Property(e => e.DocCounter)
                .HasColumnName("doc_counter");
        }
    }
}