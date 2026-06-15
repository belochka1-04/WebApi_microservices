using Grpc.Core;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using WebApi_JobService.Protos;
using WebApi_JobService.Services;
using ProtoJobService = WebApi_JobService.Protos.JobService;

namespace WebApi_JobService;

[Authorize]
public class JobServiceImpl : ProtoJobService.JobServiceBase
{
    private readonly IDatabaseService _db;

    public JobServiceImpl(IDatabaseService db)
    {
        _db = db;
    }

    #region Job

    public override async Task<GetJobByIdResponse> GetJobById(
    GetJobByIdRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Job ID"));

        var job = await _db.GetJobByIdAsync(request.Id); // Domain.Job

        if (job == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Job with ID {request.Id} not found"));

        // доменная модель → gRPC-прото через Mapster
        var protoJob = job.Adapt<Job>(); // WebApi_JobService.Protos.Job

        return new GetJobByIdResponse
        {
            Job = protoJob
        };
    }

    public override async Task<GetJobsResponse> GetJobs(
    GetJobsRequest request, ServerCallContext context)
    {
        var jobs = await _db.GetJobsAsync(); // IEnumerable<Domain.JobDescriptionAndNote>

        var response = new GetJobsResponse();

        // Domain → Protos через Mapster (есть TypeAdapterConfig<Domain.JobDescriptionAndNote, Protos.JobDescriptionAndNote>)
        response.Jobs.AddRange(jobs.Select(j => j.Adapt<JobDescriptionAndNote>()));

        return response;
    }

    public override async Task<Job> AddJobByTg(
    AddJobByTgRequest request, ServerCallContext context)
    {
        if (request.TelegramId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "TelegramId must be greater than 0"));

        var job = await _db.AddJobByTgAsync(request.TelegramId); // Domain.Job

        // Domain → Protos.Job
        return job.Adapt<Job>();
    }

    #endregion

    #region JobDescriptionAndNote

    public override async Task<UpdateStatusResponse> UpdateStatus(
        UpdateStatusRequest request, ServerCallContext context)
    {
        if (request.JobId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid JobId"));

        await _db.UpdateDescriptionAndNotesStatusAsync(request.JobId, request.Status);

        return new UpdateStatusResponse { UpdatedRows = 1 }; // можно улучшить, возвращая реальное кол-во
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdateDescription(
        UpdateDescriptionRequest request, ServerCallContext context)
    {
        if (request.JobId <= 0 || string.IsNullOrWhiteSpace(request.Fulltext))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "JobId and Fulltext are required"));

        await _db.UpdateDescriptionAndNotesDescriptionAsync(request.JobId, request.Fulltext);

        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> UpdatePicCount(
        UpdatePicCountRequest request, ServerCallContext context)
    {
        if (request.JobId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid JobId"));

        await _db.UpdateDescriptionAndNotesPicCountAsync(request.JobId, request.PicCount);

        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> InsertDescription(
        InsertDescriptionRequest request, ServerCallContext context)
    {
        if (request.JobId <= 0 || string.IsNullOrWhiteSpace(request.Fulltext))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "JobId and Fulltext are required"));

        await _db.InsertDescriptionAndNotesDescriptionAsync(request.JobId, request.Fulltext);

        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    public override async Task<Google.Protobuf.WellKnownTypes.Empty> ConfirmGoogle(
        ConfirmGoogleRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.RequestWord))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "RequestWord is required"));

        await _db.MarkJobDescriptionsGoogleConfirmedAsync(request.RequestWord);

        return new Google.Protobuf.WellKnownTypes.Empty();
    }

    #endregion
}