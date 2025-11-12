using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KameraData.Data.Models;

[Table("job_description_and_notes")]

public partial class JobDescriptionAndNote
{
    [Column("id")] public int Id { get; set; }

    [Column("job_id")] public int JobId { get; set; }

    [Column("job_description")] public string? JobDescription { get; set; }

    [Column("job_notes")] public string? JobNotes { get; set; }

    [Column("pic_counter")] public int? PicCounter { get; set; }
    [Column("pic_recon")] public int? PicRecon { get; set; }

    [Column("status")] public string? Status { get; set; }

    [Column("brand")] public string? Brand { get; set; }
    [Column("model")] public string? Model { get; set; }
    [Column("serial_number")] public string? SerialNumber { get; set; }
    [Column("ocr_text")] public string? OcrText { get; set; }
    [Column("is_rating_plate_percentage")] public int? IsRatingPlatePercentage { get; set; }
    [Column("completion_cost")] public decimal? CompletionCost { get; set; }
    [Column("image_tokens")] public int? ImageTokens { get; set; }
    [Column("sticker_link")] public string? StickerLink { get; set; }

    public virtual Job Job { get; set; } = null!;
}
