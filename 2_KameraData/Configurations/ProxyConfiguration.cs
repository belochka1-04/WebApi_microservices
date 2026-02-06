using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class ProxyConfiguration : IEntityTypeConfiguration<Proxy>
    {
        public void Configure(EntityTypeBuilder<Proxy> entity)
        {
            entity.HasKey(e => e.Id).HasName("PK_proxy_table_ID");
            entity.ToTable("proxy_table");

            entity.Property(e => e.Id)
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");

            entity.Property(e => e.Type)
                .HasColumnType("nvarchar(max)")
                .HasDefaultValue("")
                .HasColumnName("Type");

            entity.Property(e => e.IP)
                .HasColumnType("nvarchar(max)")
                .HasDefaultValue("")
                .HasColumnName("IP");

            entity.Property(e => e.Port)
                .HasColumnType("int")
                .HasColumnName("Port")
                .HasDefaultValue(0);

            entity.Property(e => e.Login)
                .HasColumnType("nvarchar(max)")
                .HasDefaultValue("")
                .HasColumnName("Login");

            entity.Property(e => e.Password)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Password");

            entity.Property(e => e.IsActive)
                .HasColumnType("TINYINT")
                .HasColumnName("IsActive")
                .HasDefaultValue(1);
        }
    }
}