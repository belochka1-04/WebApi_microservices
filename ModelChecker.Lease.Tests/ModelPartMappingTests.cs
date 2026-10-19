using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ModelChecker.Lease.Tests;

public sealed class ModelPartMappingTests
{
    [Fact]
    public void ModelPartUsesExistingModelIdAndModelTable()
    {
        using var db = new KameraDbContext(new DbContextOptionsBuilder<KameraDbContext>()
            .UseSqlServer("Server=localhost;Database=metadata_only;Integrated Security=true;TrustServerCertificate=true").Options);
        var entity = db.Model.FindEntityType(typeof(ModelPart))!;
        Assert.DoesNotContain(entity.GetProperties(), p => p.IsShadowProperty());
        var fk = Assert.Single(entity.GetForeignKeys(), f => f.PrincipalEntityType.ClrType == typeof(ModelTb));
        Assert.Equal(nameof(ModelPart.ModelId), Assert.Single(fk.Properties).Name);
        Assert.Equal("Model", fk.PrincipalEntityType.GetTableName());
        Assert.Equal(nameof(ModelTb.ModelParts), fk.PrincipalToDependent!.Name);
        Assert.DoesNotContain("ModelTbId", db.ModelParts.Include(p => p.Part).ToQueryString());
    }

    [PartsSqlFact]
    public async Task ReadsPartsForPreviouslyFailingModels()
    {
        using var db = new KameraDbContext(new DbContextOptionsBuilder<KameraDbContext>()
            .UseSqlServer(Environment.GetEnvironmentVariable("PARTS_GRABBER_TEST_CONNECTION")).Options);
        foreach (var modelId in new[] { 4572034, 4294072 })
        {
            var rows = await db.ModelParts.Include(p => p.Part).Where(p => p.ModelId == modelId).ToListAsync();
            Assert.Equal(await db.ModelParts.CountAsync(p => p.ModelId == modelId), rows.Count);
            Assert.All(rows, row => Assert.NotNull(row.Part));
        }
    }
}
