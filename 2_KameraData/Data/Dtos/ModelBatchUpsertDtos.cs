namespace KameraData.Data.Dtos;

public sealed class ModelBatchUpsertRequest
{
    public int GoogleModelRequestId { get; set; }
    public int ChunkIndex { get; set; }
    public List<ModelBatchCandidate> Models { get; set; } = new();
}

public sealed class ModelBatchCandidate
{
    public int SiteId { get; set; }
    public string BrandTitle { get; set; } = string.Empty;
    public string CleanedModel { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}

public sealed class ModelBatchUpsertResult
{
    public int Received { get; set; }
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public int Unchanged { get; set; }
    public int MissingBrandModel { get; set; }
    public int Duplicates { get; set; }
}
