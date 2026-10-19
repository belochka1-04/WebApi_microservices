using Dapper;
using Microsoft.Data.SqlClient;
using WebApi_PartsService.Services;
using Xunit;

namespace ModelChecker.Lease.Tests;

public sealed class GrabberCacheTests
{
    [Theory]
    [InlineData(" wp-w123 ", "WPW123", "W123")]
    [InlineData("w123", "W123", "WPW123")]
    public void PartNumberAliasesMatchExistingLookup(string input, string first, string second) =>
        Assert.Equal(new[] { first, second }, GrabberCacheKeys.PartNumbers(input));

    [Theory]
    [InlineData("HTTPS://WWW.EXAMPLE.COM/path", "www.example.com", "example.com")]
    [InlineData("Example.COM/", "example.com", "www.example.com")]
    public void SourceAliasesMatchExistingLookup(string input, string first, string second) =>
        Assert.Equal(new[] { first, second }, GrabberCacheKeys.Sources(input));

    [PartsSqlFact]
    public async Task ReadsActualCachedPartWithAllReplacements()
    {
        var connection = Environment.GetEnvironmentVariable("PARTS_GRABBER_TEST_CONNECTION")!;
        await using var db = new SqlConnection(connection);
        var seed = await db.QueryFirstAsync("SELECT TOP(1) id AS Id,source_site AS SourceSite,main_part_number_norm AS MainPartNumberNorm FROM dbo.grabber_parts WHERE main_part_number_norm IS NOT NULL AND main_part_number_norm<>'' ORDER BY id");
        var result = await new GrabberCache(connection).ReadAsync((string)seed.MainPartNumberNorm, (string)seed.SourceSite);
        Assert.NotNull(result);
        var expected = await db.QueryAsync<string>("SELECT replace_number FROM dbo.grabber_part_replaces WHERE grabber_part_id=@id ORDER BY id",new { id = result.PartSourceId });
        Assert.Equal(expected.ToArray(), result.Replaces);
        var expectedName = await db.ExecuteScalarAsync<string>("SELECT part_name FROM dbo.grabber_parts WHERE id=@id", new { id = result.PartSourceId });
        Assert.Equal(expectedName, result.Name);
        Assert.Null(await new GrabberCache(connection).ReadAsync("QA_MISSING_"+Guid.NewGuid().ToString("N"), (string)seed.SourceSite));
    }
}
