namespace GuardianStock.Application.Contracts.Identity
{
    public class IdentityUserDto
    {

        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string UserName { get; set; }

        public IEnumerable<string> Roles { get; set; }

    }
}
