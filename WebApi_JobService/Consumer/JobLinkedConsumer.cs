namespace WebApi_JobService.Consumer
{
    using KameraData.Data;
    using KameraData.Data.Events;
    using KameraData.Events;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    namespace JobsService.Consumers
    {
        public class JobLinkedConsumer : IConsumer<JobLinkedEvent>
        {
            private readonly KameraDbContext _dbContext;
            private readonly ILogger<JobLinkedConsumer> _logger;

            public JobLinkedConsumer(KameraDbContext dbContext, ILogger<JobLinkedConsumer> logger)
            {
                _dbContext = dbContext;
                _logger = logger;
            }

            public async Task Consume(ConsumeContext<JobLinkedEvent> context)
            {
                _logger.LogInformation("🔗 JobLinkedEvent получен: JobId={JobId} → UserId={UserId}",
                    context.Message.JobId, context.Message.UserId);

                // Находим Job и обновляем
                var job = await _dbContext.Jobs.FindAsync(context.Message.JobId);
                if (job == null)
                {
                    _logger.LogWarning("❌ Job {JobId} не найден", context.Message.JobId);
                    return;
                }

                job.UserId = context.Message.UserId;     //  Устанавливаем UserId
                job.Status = "Linked";                   //  Меняем статус

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("✅ Job {JobId} обновлён: UserId={UserId}, Status=Linked",
                    context.Message.JobId, context.Message.UserId);
            }
        }
    }

}
