using KameraData.Data.Events;
using KameraData.Events;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Logging;
using WebApi_UserService.Services;

namespace UserService.Consumers
{
    public class JobCreatedConsumer : IConsumer<JobCreatedEvent>
    {
        private readonly IDatabaseService _userService;
        private readonly ILogger<JobCreatedConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public JobCreatedConsumer(IDatabaseService userService,
    IPublishEndpoint publishEndpoint,  // ← НОВОЕ
    ILogger<JobCreatedConsumer> logger)
        {
            _userService = userService;
            _logger = logger;
            _publishEndpoint = publishEndpoint;  // ← НОВОЕ
        }

        public async Task Consume(ConsumeContext<JobCreatedEvent> context)
        {
            var user = await _userService.GetUserByTgAsync(context.Message.TelegramId);
            if (user == null) return;

            // ПУБЛИКУЕМ НОВОЕ СОБЫТИЕ
            await _publishEndpoint.Publish(new JobLinkedEvent
            {
                JobId = context.Message.JobId,
                UserId = user.Id,
                TelegramId = context.Message.TelegramId
            });
        }
    }
}
