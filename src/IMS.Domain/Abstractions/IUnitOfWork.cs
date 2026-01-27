namespace IMS.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        public Task<int> Complete(CancellationToken cancellationToken);
    }
}
