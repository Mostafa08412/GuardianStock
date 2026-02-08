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
}
