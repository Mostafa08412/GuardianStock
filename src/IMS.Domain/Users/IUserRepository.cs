using IMS.Domain.Abstractions;

namespace IMS.Domain.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
