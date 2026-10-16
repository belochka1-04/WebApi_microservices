using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebApi_JobStockService.Controllers;
using Xunit;

namespace WebApi_JobStockService.Tests;

public sealed class BatchCleanupDatabaseTests
{
    [SqlServerFact]
    public async Task SqlServerDeletesOnlySelectedRows_InsideRolledBackFixture()
    {
        await using var connection = new SqlConnection(Environment.GetEnvironmentVariable("STOCK_CLEANUP_TEST_SQL"));
        await connection.OpenAsync();
        await using var db = new KameraDbContext(new DbContextOptionsBuilder<KameraDbContext>().UseSqlServer(connection).Options);
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            await using var command = connection.CreateCommand();
            command.Transaction = (SqlTransaction)transaction.GetDbTransaction();
            command.CommandText = "INSERT dbo.jobs(user_id,job_number,update_request_status) OUTPUT inserted.id VALUES(1036,@marker,'9')";
            command.Parameters.AddWithValue("@marker", "QA-CLEANUP-" + Guid.NewGuid().ToString("N"));
            var firstJob = Convert.ToInt32(await command.ExecuteScalarAsync());
            command.Parameters["@marker"].Value = "QA-CLEANUP-" + Guid.NewGuid().ToString("N");
            var secondJob = Convert.ToInt32(await command.ExecuteScalarAsync());
            command.Parameters.Clear();
            command.CommandText = "INSERT dbo.job_stocks(job_id,user_stock_id,match_part_number) VALUES(@first,1,'TEST'),(@first,1,'TEST'),(@first,2,'TEST'),(@second,1,'TEST')";
            command.Parameters.AddWithValue("@first", firstJob);
            command.Parameters.AddWithValue("@second", secondJob);
            await command.ExecuteNonQueryAsync();
            var service = new WebApi_JobStockService.Services.DatabaseService(db);
            Assert.Equal(2, await service.DeleteJobStocksBatchAsync(firstJob, new[] { 1, 1 }));
            Assert.Equal(0, await service.DeleteJobStocksBatchAsync(firstJob, new[] { 1 }));
            Assert.Equal(1, await db.JobStocks.CountAsync(x => x.JobId == firstJob && x.UserStockId == 2));
            Assert.Equal(1, await db.JobStocks.CountAsync(x => x.JobId == secondJob && x.UserStockId == 1));
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task DeletesOnlySelectedPositionsOfSelectedJob_AndRetryIsIdempotent()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = new CleanupContext(new DbContextOptionsBuilder<KameraDbContext>().UseSqlite(connection).Options);
        await db.Database.EnsureCreatedAsync();
        db.JobStocks.AddRange(Row(42, 1), Row(42, 1), Row(42, 2), Row(43, 1));
        await db.SaveChangesAsync();
        var service = new WebApi_JobStockService.Services.DatabaseService(db);
        Assert.Equal(2, await service.DeleteJobStocksBatchAsync(42, new[] { 1, 1 }));
        Assert.Equal(0, await service.DeleteJobStocksBatchAsync(42, new[] { 1 }));
        Assert.Equal(new[] { "42/2", "43/1" }, await db.JobStocks.OrderBy(x => x.JobId)
            .Select(x => x.JobId + "/" + x.UserStockId).ToArrayAsync());

        var controller = new JobStocksController(service);
        Assert.IsType<BadRequestObjectResult>(await controller.DeleteJobStocksBatch(42, new[] { -1 }, default));
        Assert.IsType<BadRequestObjectResult>(await controller.DeleteJobStocksBatch(42, new int[501], default));
        Assert.IsType<NoContentResult>(await controller.DeleteJobStocksBatch(42, Array.Empty<int>(), default));
    }

    [Fact]
    public async Task SqlFailurePropagatesInsteadOfReportingSuccess()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = new CleanupContext(new DbContextOptionsBuilder<KameraDbContext>().UseSqlite(connection).Options);
        var service = new WebApi_JobStockService.Services.DatabaseService(db);
        await Assert.ThrowsAsync<SqliteException>(() => service.DeleteJobStocksBatchAsync(42, new[] { 1 }));
    }

    private static JobStock Row(int job, int stock) => new() { JobId = job, UserStockId = stock, MatchPartNumber = "TEST" };

    private sealed class CleanupContext(DbContextOptions<KameraDbContext> options) : KameraDbContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes().Select(x => x.ClrType).ToArray())
                modelBuilder.Ignore(type);
            modelBuilder.Entity<JobStock>(entity =>
            {
                entity.ToTable("job_stocks");
                entity.HasKey(x => x.Id);
                entity.Ignore(x => x.Job);
                entity.Ignore(x => x.UserStock);
            });
        }
    }
}

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("STOCK_CLEANUP_TEST_SQL")))
            Skip = "Set STOCK_CLEANUP_TEST_SQL to run the rollback-only SQL Server integration fixture.";
    }
}
