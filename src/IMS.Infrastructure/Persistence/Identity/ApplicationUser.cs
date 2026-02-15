using AutoMapper;
using IMS.Application.Contracts.Identity;
using IMS.Domain.Abstractions;
using IMS.Infrastructure.Tokens;
using Microsoft.AspNetCore.Identity;

namespace IMS.Infrastructure.Persistence.Identity
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
