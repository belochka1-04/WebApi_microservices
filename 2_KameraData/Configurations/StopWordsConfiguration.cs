using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class StopWordsConfiguration : IEntityTypeConfiguration<StopWords>
    {
        public void Configure(EntityTypeBuilder<StopWords> entity)
        {
            entity.ToTable("Stop_words");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.Word)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("word");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id");
        }
    }
}