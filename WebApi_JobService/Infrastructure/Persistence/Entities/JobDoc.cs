using System;
using System.Collections.Generic;

namespace WebApi_JobService.Infrastructure.Persistence.Entities;

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

    public virtual Job? Job { get; set; } = null!;

    
}
