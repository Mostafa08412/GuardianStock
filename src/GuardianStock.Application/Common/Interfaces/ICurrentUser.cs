namespace GuardianStock.Application.Common.Interfaces
{
    public interface ICurrentUser
    {

        public Guid? UserId { get; }
        public string UserEmail { get; }
    }
}
