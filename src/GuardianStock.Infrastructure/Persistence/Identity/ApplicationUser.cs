using AutoMapper;
using GuardianStock.Application.Contracts.Identity;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Infrastructure.Tokens;
using Microsoft.AspNetCore.Identity;

namespace GuardianStock.Infrastructure.Persistence.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, IUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }

    public class ApplicationUserMappingProfile : Profile
    {
        public ApplicationUserMappingProfile()
        {
            CreateMap<ApplicationUser, IdentityUserDto>();

        }
    }
}
