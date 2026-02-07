using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourNamespace.Data.Configurations
{
    public class ApiClientConfiguration : IEntityTypeConfiguration<ApiClient>
    {
        public void Configure(EntityTypeBuilder<ApiClient> entity)
        {
            entity.ToTable("ApiClients");

            // PK
            entity.HasKey(e => e.Id)
                  .HasName("PK_ApiClients_Id");

            // Уникальный индекс по ClientId
            entity.HasIndex(e => e.ClientId)
                  .IsUnique()
                  .HasDatabaseName("IX_ApiClients_ClientId");

            entity.Property(e => e.Id)
                  .HasColumnName("Id")
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.ClientId)
                  .HasColumnName("ClientId")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.ClientSecretHash)
                  .HasColumnName("ClientSecretHash")
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.Name)
                  .HasColumnName("Name")
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.AllowedScopes)
                  .HasColumnName("AllowedScopes")
                  .HasMaxLength(400);

            entity.Property(e => e.IsActive)
                  .HasColumnName("IsActive")
                  .HasDefaultValue(true);

            entity.Property(e => e.AllowedIps)
                  .HasColumnName("AllowedIps")
                  .HasMaxLength(400);

            entity.Property(e => e.AllowedOrigins)
                  .HasColumnName("AllowedOrigins")
                  .HasMaxLength(400);

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("CreatedAt")
                  .HasColumnType("datetime2")
                  .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.CreatedBy)
                  .HasColumnName("CreatedBy")
                  .HasMaxLength(100);

            entity.Property(e => e.LastUpdatedAt)
                  .HasColumnName("LastUpdatedAt")
                  .HasColumnType("datetime2");

            entity.Property(e => e.LastUpdatedBy)
                  .HasColumnName("LastUpdatedBy")
                  .HasMaxLength(100);
        }
    }
}
