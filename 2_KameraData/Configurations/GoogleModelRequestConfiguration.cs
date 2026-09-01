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
    public class GoogleModelRequestConfiguration : IEntityTypeConfiguration<GoogleModelRequest>
    {
        public void Configure(EntityTypeBuilder<GoogleModelRequest> builder)
        {
            builder.ToTable("GoogleModelRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("Id");

            builder.Property(x => x.Request)
                .HasColumnName("Request")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("sysutcdatetime()")
                .IsRequired();

            builder.Property(x => x.LastCheckedAt)
                .HasColumnName("LastCheckedAt")
                .HasColumnType("datetime2(0)");

            builder.Property(x => x.Status)
                .HasColumnName("Status")
                .HasDefaultValue((byte)0)
                .IsRequired();

            builder.Property(x => x.WorkerId)
                .HasColumnName("WorkerId")
                .HasMaxLength(128);

            builder.Property(x => x.LeaseUntil)
                .HasColumnName("LeaseUntil")
                .HasColumnType("datetime2(3)");

            builder.Property(x => x.AttemptCount)
                .HasColumnName("AttemptCount")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.LastError)
                .HasColumnName("LastError")
                .HasMaxLength(2000);
        }
    }
}
