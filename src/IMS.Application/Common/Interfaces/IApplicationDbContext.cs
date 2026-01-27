using IMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<User> BusinessUsers { get; }

    }
}
