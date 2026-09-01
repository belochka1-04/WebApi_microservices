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
        public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
        {
            public void Configure(EntityTypeBuilder<Application> entity)
            {
            entity.HasKey(e => e.Id).HasName("PK_applications_id");

            entity.ToTable("applications");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .HasColumnName("file_name")
                .IsRequired(false); // Указывает, что поле может быть NULL

            entity.Property(e => e.LastRunDate)
                .HasDefaultValueSql("NULL") // Устанавливает значение по умолчанию как NULL
                .HasColumnType("datetime") // Используем datetime2 для MSSQL
                .HasColumnName("last_run_date")
                .IsRequired(false); // Указывает, что поле может быть NULL

            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .HasColumnName("name")
                .IsRequired(false); // Указывает, что поле может быть NULL

            entity.Property(e => e.ProcessName)
                .HasMaxLength(500)
                .HasColumnName("process_name")
                .IsRequired(false); // Указывает, что поле может быть NULL

            entity.Property(e => e.Order)
                .HasColumnType("int")
                .HasColumnName("order")
                .IsRequired(true); // Указывает, что поле может быть NULL

            entity.Property(e => e.Status)
                .HasDefaultValue(0) // Устанавливает значение по умолчанию как 0
                .HasColumnType("int")
                .HasColumnName("status")
                .IsRequired(false); // Указывает, что поле может быть NULL

            entity.Property(e => e.Path)
                  .HasColumnName("path")
                  .IsRequired(); // Указывает, что поле не может быть NULL

            entity.Property(e => e.Caption)
               .HasColumnName("caption")
               .IsRequired(); // Указывает, что поле не может быть NULL

            entity.Property(e => e.CanStartInStarter)
              .HasColumnType("int")
              .HasColumnName("CanStartInStarter")
              .IsRequired(false); // Указывает, что поле может быть NULL
        }

        }
}
