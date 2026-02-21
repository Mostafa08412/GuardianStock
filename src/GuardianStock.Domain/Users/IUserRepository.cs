using GuardianStock.Domain.Abstractions;

namespace GuardianStock.Domain.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
