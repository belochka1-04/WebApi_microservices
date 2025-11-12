using System;
using System.Collections.Generic;

namespace KameraData.Data.Models;

public partial class JobDocsModelInfo
{
    public int? Id { get; set; }

    public int? JobDocModelId { get; set; }

    public string? Jdmodel { get; set; }

    public int? JdsourceId { get; set; }

    public string? Jdbrand { get; set; }

    public string? JdmodelConfidence { get; set; }

    public int? JdcategoriesId { get; set; }

    public string? JdfileTitle { get; set; }

    public string? JdfileName { get; set; }

    public string? JdwebLink { get; set; }

    public string? JdlocalPath { get; set; }

    public DateOnly? JddateModel { get; set; }

    public string? JdmodelVersion { get; set; }

    public int? JobId { get; set; }

    public string? ModelNumber { get; set; }

    public int? ModelsNumberId { get; set; }

    public int? ModelId { get; set; }

    public string? Model { get; set; }

    public int? SourceId { get; set; }

    public string? Brand { get; set; }

    public string? ModelConfidence { get; set; }

    public int? CategoriesId { get; set; }

    public string? FileTitle { get; set; }

    public string? FileName { get; set; }

    public string? WebLink { get; set; }

    public string? LocalPath { get; set; }

    public DateOnly? DateModel { get; set; }

    public string? ModelVersion { get; set; }

    public string? NotFountModel { get; set; }

    public string? JobDescription { get; set; }

    public string? JobNotes { get; set; }

    public string? Token { get; set; }

    public string? ResponseText { get; set; }
}
