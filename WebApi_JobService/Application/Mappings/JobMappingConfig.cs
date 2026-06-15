using Google.Protobuf.WellKnownTypes;
using Mapster;
using EFModels = KameraData.Data.Models;
using Protos = WebApi_JobService.Protos;

namespace JobService.Application.Mappings;

public static class JobMappingConfig
{
    public static void Apply()
    {
        // =============================================
        // 1. EF Model → Domain Entity
        // =============================================

        TypeAdapterConfig<EFModels.Job, Domain.Entities.Job>
           .NewConfig()
           .Map(dest => dest.Id, src => src.Id)
           .Map(dest => dest.UserId, src => src.UserId)
           // В EF Job.TelegramId помечен [NotMapped] → защитимся
           //.Map(dest => dest.TelegramId, src => (int?)src.TelegramId)
           .Map(dest => dest.JobNumber, src => src.JobNumber)
           .Map(dest => dest.JobLink, src => src.JobLink)
           .Map(dest => dest.Token, src => src.Token)
           .Map(dest => dest.UpdateRequestStatus, src => src.UpdateRequestStatus)
           // В EF Job.Status тоже [NotMapped] → если он всегда "Pending", можно так:
           .Map(dest => dest.Status, src => string.IsNullOrEmpty(src.Status)
                                                       ? "Pending"
                                                       : src.Status)
           .Map(dest => dest.CreatedAt, src => src.CreatedAt)
           // UpdatedAt в старой модели нет → пусть будет null
           .Map(dest => dest.UpdatedAt, src => (DateTime?)null);


        TypeAdapterConfig<EFModels.JobDescriptionAndNote, Domain.Entities.JobDescriptionAndNote>
          .NewConfig()
          .Map(dest => dest.Id, src => src.Id)
          .Map(dest => dest.JobId, src => src.JobId)

          .Map(dest => dest.JobDescription, src => src.JobDescription)
          .Map(dest => dest.JobNotes, src => src.JobNotes)

          // Если в домене FullText = объединённый текст, можно так:
          .Map(dest => dest.FullText,
               src => (src.JobDescription ?? "") + (string.IsNullOrEmpty(src.JobNotes) ? "" : "\n" + src.JobNotes))

          .Map(dest => dest.PicRecon, src => src.PicRecon)
          // В БД PicCounter нет → доменное поле = 0 по умолчанию
          .Map(dest => dest.PicCounter, src => 0)

          .Map(dest => dest.Status, src => src.Status)

          .Map(dest => dest.OriginalBrand, src => src.OriginalBrand)
          // В EF-модели отдельного Brand может не быть → оставляем null
          .Map(dest => dest.Brand, src => (string?)null)
          .Map(dest => dest.Model, src => src.Model)
          .Map(dest => dest.SerialNumber, src => src.SerialNumber)
          .Map(dest => dest.StickerLink, src => src.StickerLink)

          // В БД OCR-текста нет → доменное поле пока не заполняем
          .Map(dest => dest.OcrText, src => (string?)null)

          .Map(dest => dest.GoogleConfirmed, src => src.GoogleConfirmed)
          .Map(dest => dest.IsRatingPlatePercentage, src => src.IsRatingPlatePercentage)
          .Map(dest => dest.CompletionCost, src => src.CompletionCost)
          .Map(dest => dest.ImageTokens, src => src.ImageTokens)

          .Map(dest => dest.UpdatedAt, src => (DateTime?)null);

        // =============================================
        // 2. Domain → Protos (gRPC)
        // =============================================

        // Domain → Protos.Job
        TypeAdapterConfig<Domain.Entities.Job, Protos.Job>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.JobNumber, src => src.JobNumber ?? "")
            .Map(dest => dest.JobLink, src => src.JobLink ?? "")
            .Map(dest => dest.Token, src => src.Token ?? "")
            .Map(dest => dest.UpdateRequestStatus,
                                           src => src.UpdateRequestStatus ?? "")
            .Map(dest => dest.Status, src => string.IsNullOrEmpty(src.Status)
                                                        ? "Pending"
                                                        : src.Status)
            .Map(dest => dest.CreatedAt,
                 src => src.CreatedAt.HasValue
                        ? Timestamp.FromDateTime(src.CreatedAt.Value.ToUniversalTime())
                        : null)
            .Map(dest => dest.TelegramId, src => src.TelegramId ?? 0); // proto int32, домен int?

        // Domain → Protos.JobDescriptionAndNote
        TypeAdapterConfig<Domain.Entities.JobDescriptionAndNote, Protos.JobDescriptionAndNote>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.JobDescription, src => src.JobDescription ?? "")
            .Map(dest => dest.JobNotes, src => src.JobNotes ?? "")
            .Map(dest => dest.Status, src => src.Status ?? "")
            .Map(dest => dest.PicRecon, src => src.PicRecon ?? 0)
            // В proto google_confirmed bool, в домене int? 0/1
            .Map(dest => dest.GoogleConfirmed, src => src.GoogleConfirmed == 1)
            .Map(dest => dest.OriginalBrand, src => src.OriginalBrand ?? "")
            .Map(dest => dest.Model, src => src.Model ?? "")
            .Map(dest => dest.StickerLink, src => src.StickerLink ?? "")
            .Map(dest => dest.SerialNumber, src => src.SerialNumber ?? "");

        // =============================================
        // 3. Domain ↔ JobDto (если нужно)
        // =============================================

        // если JobDto — это твой DTO для /api/Job/GetJobByIdAsync и т.п.
        // string? или string, как у тебя сейчас в DTO

        /* пример, если JobDto в KameraData.Data.Dtos.JobDto:
        TypeAdapterConfig<Domain.Job, EFModels.JobDto>
            .NewConfig()
            .Map(dest => dest.Id,            src => src.Id)
            .Map(dest => dest.UserId,        src => src.UserId)
            .Map(dest => dest.TelegramId,    src => src.TelegramId)
            .Map(dest => dest.JobNumber,     src => src.JobNumber)
            .Map(dest => dest.JobLink,       src => src.JobLink)
            .Map(dest => dest.Token,         src => src.Token)
            .Map(dest => dest.UpdateRequestStatus, src => src.UpdateRequestStatus)
            .Map(dest => dest.Status,        src => src.Status);
         */
    }
}