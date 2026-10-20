using KameraData.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebApi_JobDocumentService.Controllers;
using WebApi_JobDocumentService.Services;
using Xunit;

namespace WebApi_JobStockService.Tests;

public class OptionalDocumentErrorsTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DatabaseFailureIs500AndNeverDisguisedAsAbsentDocument(bool pdf)
    {
        await using var connection=new SqliteConnection("Data Source=:memory:");await connection.OpenAsync();
        await using var db=new KameraDbContext(new DbContextOptionsBuilder<KameraDbContext>().UseSqlite(connection).Options);
        var controller=new JobDocController(new DatabaseService(db));
        // The missing schema deliberately causes a database exception, not an empty result set.
        var result=pdf?await controller.GetPdfDoc(1):await controller.GetMainDoc(1);
        Assert.Equal(500,Assert.IsType<ObjectResult>(result.Result).StatusCode);
    }
}
