using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class JobDoc
{
    public int Id { get; set; }

    public int? JobId { get; set; }

    public int ModelsId { get; set; }

    public string? CleanedModel { get; set; }

    //public string? CategoryName { get; set; }

    //public DateOnly? Data { get; set; }

    //public string? Version { get; set; }
    public string? DocumentType { get; set; }

    public int? Site { get; set; }
    public int? Part_counter { get; set; }

    public string? Status { get; set; }
    public int? siteState { get; set; }
    public int? docState { get; set; }
    public int? partCountState { get; set; }

    public int? Document_Id { get; set; }

    public int? CPСountState { get; set; }

    public int? clicked { get; set; }

    public int? QaState { get; set; }

    public int? VideoState { get; set; }

    public int? ArticleState { get; set; }

    public int? QaCnt { get; set; }

    public int? VideoCnt { get; set; }

    public int? ArticleCnt { get; set; }

    public virtual Job? Job { get; set; } = null!;

    public virtual KameraData.Data.Models.Model? Models { get; set; } = null!;
}
