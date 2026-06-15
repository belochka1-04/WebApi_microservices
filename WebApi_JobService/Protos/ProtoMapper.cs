using Google.Protobuf.WellKnownTypes;
using KameraData.Data.Models;

namespace WebApi_JobService;

public static class ProtoMapper
{
    /// <summary>
    /// Entity Job → Proto Job
    /// </summary>
    public static Protos.Job MapToProto(Job job)
    {
        if (job == null) return new Protos.Job();

        return new Protos.Job
        {
            Id = job.Id,
            UserId = job.UserId,
            JobNumber = job.JobNumber ?? "",
            JobLink = job.JobLink ?? "",
            Token = job.Token ?? "",
            UpdateRequestStatus = job.UpdateRequestStatus ?? "",
            Status = job.Status ?? "",
            TelegramId = job.TelegramId,

            // Правильная обработка DateTime?
            CreatedAt = job.CreatedAt.HasValue
                ? Timestamp.FromDateTime(job.CreatedAt.Value.ToUniversalTime())
                : null
        };
    }

    /// <summary>
    /// Entity JobDescriptionAndNote → Proto JobDescriptionAndNote
    /// </summary>
    public static Protos.JobDescriptionAndNote MapToProtoNote(JobDescriptionAndNote note)
    {
        if (note == null) return new Protos.JobDescriptionAndNote();

        return new Protos.JobDescriptionAndNote
        {
            Id = note.Id,
            JobId = note.JobId,
            JobDescription = note.JobDescription ?? "",
            JobNotes = note.JobNotes ?? "",
            Status = note.Status ?? "",
            PicRecon = note.PicRecon ?? 0,

            // GoogleConfirmed скорее всего int? в БД (0/1)
            GoogleConfirmed = note.GoogleConfirmed == 1,

            OriginalBrand = note.OriginalBrand ?? "",
            Model = note.Model ?? "",
            StickerLink = note.StickerLink ?? "",
            SerialNumber = note.SerialNumber ?? ""
        };
    }

    /// <summary>
    /// Proto Job → Entity Job (если понадобится)
    /// </summary>
    public static Job MapToEntity(Protos.Job proto)
    {
        return new Job
        {
            Id = proto.Id,
            UserId = proto.UserId,
            JobNumber = string.IsNullOrEmpty(proto.JobNumber) ? null : proto.JobNumber,
            JobLink = string.IsNullOrEmpty(proto.JobLink) ? null : proto.JobLink,
            Token = string.IsNullOrEmpty(proto.Token) ? null : proto.Token,
            UpdateRequestStatus = string.IsNullOrEmpty(proto.UpdateRequestStatus) ? null : proto.UpdateRequestStatus,
            Status = string.IsNullOrEmpty(proto.Status) ? null : proto.Status,
            TelegramId = proto.TelegramId,
            CreatedAt = proto.CreatedAt?.ToDateTime()
        };
    }
}