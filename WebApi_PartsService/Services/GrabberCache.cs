using Dapper;
using Microsoft.Data.SqlClient;

namespace WebApi_PartsService.Services;

public sealed class GrabberCachedPart
{
    public int PartSourceId { get; set; }
    public string SourceName { get; set; } = "";
    public string? Name { get; set; }
    public List<string> Replaces { get; set; } = new();
    public List<object> Pictures { get; set; } = new();
    public string? SitePartNumber { get; set; }
    public double RegularPrice { get; set; }
    public int AttempsCount { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public static class GrabberCacheKeys
{
    public static string[] PartNumbers(string value)
    {
        var number = new string(value.Trim().ToUpperInvariant().Where(char.IsLetterOrDigit).ToArray());
        if (number.Length == 0) return Array.Empty<string>();
        if (number.StartsWith("WPW") && number.Length > 3) return new[] { number, "W" + number[3..] };
        if (number.StartsWith('W') && !number.StartsWith("WPW") && number.Length > 1)
            return new[] { number, "WPW" + number[1..] };
        return new[] { number };
    }

    public static string[] Sources(string value)
    {
        var source = value.Trim();
        if (Uri.TryCreate(source, UriKind.Absolute, out var uri)) source = uri.Host;
        source = source.TrimEnd('/').ToLowerInvariant();
        if (source.Length == 0) return Array.Empty<string>();
        return new[] { source, source.StartsWith("www.") ? source[4..] : "www." + source };
    }
}

public sealed class GrabberCache(string connectionString)
{
    private sealed class Row
    {
        public int Id { get; set; }
        public string SourceSite { get; set; } = "";
        public string? MainPartNumber { get; set; }
        public string? PartName { get; set; }
        public DateTime? FirstSeenAt { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }

    public async Task<GrabberCachedPart?> ReadAsync(string partNumber, string sourceSite, CancellationToken ct = default)
    {
        var numbers = GrabberCacheKeys.PartNumbers(partNumber);
        var sources = GrabberCacheKeys.Sources(sourceSite);
        if (numbers.Length == 0 || sources.Length == 0) return null;
        await using var db = new SqlConnection(connectionString);
        var part = await db.QueryFirstOrDefaultAsync<Row>(new CommandDefinition("""
SELECT TOP (1) id AS Id, source_site AS SourceSite, main_part_number AS MainPartNumber,
part_name AS PartName, first_seen_at AS FirstSeenAt, last_seen_at AS LastSeenAt
FROM dbo.grabber_parts
WHERE source_site IN @sources
AND (main_part_number_norm IN @numbers
 OR ((main_part_number_norm IS NULL OR LTRIM(RTRIM(main_part_number_norm))='') AND main_part_number IN @numbers))
ORDER BY last_seen_at DESC, id DESC;
""", new { numbers, sources }, cancellationToken: ct));
        if (part is null) return null;
        var replacements = await db.QueryAsync<string>(new CommandDefinition("""
SELECT replace_number FROM dbo.grabber_part_replaces WHERE grabber_part_id=@id ORDER BY id;
""", new { id = part.Id }, cancellationToken: ct));
        return new GrabberCachedPart
        {
            PartSourceId = part.Id, SourceName = part.SourceSite, SitePartNumber = part.MainPartNumber,
            Name = part.PartName, Replaces = replacements.ToList(), CreatedAt = part.FirstSeenAt ?? DateTime.UtcNow,
            ExpiresAt = part.LastSeenAt?.AddDays(30)
        };
    }
}
