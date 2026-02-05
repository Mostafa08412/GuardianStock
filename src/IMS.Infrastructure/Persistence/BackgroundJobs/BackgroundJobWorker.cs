using Hangfire;
using IMS.Application.Common.Interfaces;
using IMS.Application.LowStockAlerts.Commands.SendLowStockEmail;
using MediatR;

namespace IMS.Infrastructure.Persistence.BackgroundJobs
{

    internal class BackgroundJobBridge
    {
        private readonly IMediator _mediator;

        public BackgroundJobBridge(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendRequestAsync<TRequest>(TRequest command) where TRequest : IRequest
        {
            await _mediator.Send(command);
        }


    }

    internal class BackgroundJobWorker : IBackgroundJobWorker
    {

        public void EnqueueSendLowStockEmailJob(SendLowStockEmailCommand request)
        {
            var jobId = BackgroundJob.Schedule<BackgroundJobBridge>(bridge => bridge.SendRequestAsync(request), TimeSpan.FromSeconds(10));

        }
    }
}
