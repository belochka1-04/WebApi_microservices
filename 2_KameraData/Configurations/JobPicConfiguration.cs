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

    public class JobPicConfiguration : IEntityTypeConfiguration<JobPic>
    {
        public void Configure(EntityTypeBuilder<JobPic> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_job_pics_test_100");

            entity.ToTable("job_pics_test_100");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                .HasColumnName("id");

            entity.Property(e => e.FolderName)
                .HasMaxLength(255)
                .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                .HasColumnName("folder_name");

            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                .HasColumnName("file_name");

            entity.Property(e => e.OcrText)
                .HasMaxLength(255)
                .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                .HasColumnName("ocr_text");

            entity.Property(e => e.IsRating)
                .HasColumnType("int")
                .HasColumnName("is_rating_plate_percentage");

            entity.Property(e => e.CompletionCosts)
                .HasColumnType("decimal(18,6)")
                .HasColumnName("completion_cost");

            entity.Property(e => e.ImageToken)
                .HasColumnType("int")
                .HasColumnName("image_tokens")
                .HasDefaultValue(0); // Устанавливаем значение по умолчанию как 0

            entity.Property(e => e.Brand)
                .HasMaxLength(255)
                .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                .HasColumnName("brand");

            entity.Property(e => e.Model)
                .HasMaxLength(255)
                .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                .HasColumnName("model");

            entity.Property(e => e.SerialNumber)
                .HasMaxLength(255)
                .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                .HasColumnName("serial_number");
        }
    }
}
