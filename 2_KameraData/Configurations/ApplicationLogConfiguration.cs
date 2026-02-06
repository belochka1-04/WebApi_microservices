using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Configurations
{

    public class ApplicationLogConfiguration : IEntityTypeConfiguration<ApplicationLog>
    {
        public void Configure(EntityTypeBuilder<ApplicationLog> entity)
        {
            entity.ToTable("ApplicationLog");

            entity.Property(e => e.Id)
                .HasColumnName("Id");

            entity.Property(e => e.ApplicationId)
                .HasColumnName("ApplicationId");

            entity.Property(e => e.RecDate)
                .HasColumnType("datetime")
                .HasColumnName("RecDate");

            entity.Property(e => e.RecType)
                .HasColumnName("RecType");

            entity.Property(e => e.Text)
                .HasColumnName("Text");

            entity.Property(e => e.Description)
                .HasColumnName("Description");
        }
    }
}
