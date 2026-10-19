namespace WebApi_PartsService.Services;
internal static class PartsGrabberSql {
public const string Claim = @"
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @claimed TABLE
(
    Id int,
    Replaces nvarchar(max),
    PartName nvarchar(max),
    MainPartNumber nvarchar(max),
    Status nvarchar(max),
    GotForWorkAt datetime2(7),
    PartsWorkerId nvarchar(128),
    PartsLeaseUntilUtc datetime2(3),
    PartsAttemptCount int,
    PartsProcessingStartedAtUtc datetime2(3),
    PartsProcessingCompletedAtUtc datetime2(3),
    PartsProcessingLastError nvarchar(2000),
    PicBase64 nvarchar(max),
    Brand nvarchar(max),
    Pic nvarchar(max),
    PicLink nvarchar(max),
    PhotoStatus nvarchar(max),
    DateUpdate datetime2(7)
);

;WITH parts_to_claim AS
(
    SELECT TOP (@BatchSize) *
    FROM dbo.parts_and_replaces WITH (UPDLOCK, READPAST, ROWLOCK)
    WHERE (@OnlyId IS NULL OR id=@OnlyId) AND ISNULL(parts_attempt_count,0)<@MaxAttempts AND (status='0' OR (status='1' AND parts_lease_until_utc<SYSUTCDATETIME()))
    ORDER BY id
)
UPDATE parts_to_claim
SET status = '1',
    got_for_work_at = COALESCE(got_for_work_at, SYSUTCDATETIME()),
    parts_worker_id = @WorkerId,
    parts_lease_until_utc = DATEADD(SECOND, @LeaseSeconds, SYSUTCDATETIME()),
    parts_attempt_count = ISNULL(parts_attempt_count, 0) + 1,
    parts_processing_started_at_utc = COALESCE(parts_processing_started_at_utc, SYSUTCDATETIME()),
    parts_processing_completed_at_utc = NULL,
    parts_processing_last_error = NULL,
    date_update = SYSUTCDATETIME()
OUTPUT
    inserted.id AS Id,
    inserted.replaces AS Replaces,
    inserted.part_name AS PartName,
    inserted.main_part_number AS MainPartNumber,
    inserted.status AS Status,
    inserted.got_for_work_at AS GotForWorkAt,
    inserted.parts_worker_id AS PartsWorkerId,
    inserted.parts_lease_until_utc AS PartsLeaseUntilUtc,
    inserted.parts_attempt_count AS PartsAttemptCount,
    inserted.parts_processing_started_at_utc AS PartsProcessingStartedAtUtc,
    inserted.parts_processing_completed_at_utc AS PartsProcessingCompletedAtUtc,
    inserted.parts_processing_last_error AS PartsProcessingLastError,
    inserted.pic_base64 AS PicBase64,
    inserted.brand AS Brand,
    inserted.pic AS Pic,
    inserted.pic_link AS PicLink,
    inserted.Photo_status AS PhotoStatus,
    inserted.date_update AS DateUpdate
INTO @claimed;

SELECT
    Id,
    Replaces,
    PartName,
    MainPartNumber,
    Status,
    GotForWorkAt,
    PartsWorkerId,
    PartsLeaseUntilUtc,
    PartsAttemptCount,
    PartsProcessingStartedAtUtc,
    PartsProcessingCompletedAtUtc,
    PartsProcessingLastError,
    PicBase64,
    Brand,
    Pic,
    PicLink,
    PhotoStatus,
    DateUpdate
FROM @claimed;";
public const string Name = @"
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.parts_names_archive WITH (UPDLOCK, HOLDLOCK)
    WHERE parts_and_replaces_id = @PartsAndReplacesId
      AND parts_sources_id = @PartsSourcesId
      AND partname = @PartName
)
BEGIN
    INSERT INTO dbo.parts_names_archive
        (parts_and_replaces_id, partname, parts_sources_id, attempt_counter, created_at, updated_at)
    VALUES
        (@PartsAndReplacesId, @PartName, @PartsSourcesId, @AttemptCounter, @CreatedAt, @UpdatedAt);
END";
public const string Replace = @"
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.replaces_archive WITH (UPDLOCK, HOLDLOCK)
    WHERE parts_and_replaces_id = @PartsAndReplacesId
      AND parts_sources_id = @PartsSourcesId
      AND replace_number = @ReplaceNumber
)
BEGIN
    INSERT INTO dbo.replaces_archive
        (parts_and_replaces_id, replace_number, parts_sources_id, attempt_counter, created_at, updated_at)
    VALUES
        (@PartsAndReplacesId, @ReplaceNumber, @PartsSourcesId, @AttemptCounter, @CreatedAt, @UpdatedAt);
END";
public const string Picture = @"
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.parts_pic_archive WITH (UPDLOCK, HOLDLOCK)
    WHERE parts_and_replaces_id = @PartsAndReplacesId
      AND parts_sources_id = @PartsSourcesId
      AND link = @Link
)
BEGIN
    INSERT INTO dbo.parts_pic_archive
        (parts_and_replaces_id, local_path, link, parts_sources_id, attempt_counter, created_at, updated_at)
    VALUES
        (@PartsAndReplacesId, @LocalPath, @Link, @PartsSourcesId, @AttemptCounter, @CreatedAt, @UpdatedAt);
END";
}