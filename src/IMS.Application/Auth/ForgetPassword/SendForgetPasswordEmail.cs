using MediatR;

namespace IMS.Application.Auth.ForgetPassword
{
    public record SendForgetPasswordEmail : IRequest
    {
        public string To { get; init; }

        public string Name { get; init; }

        public string EmailAddress { get; init; }

        public string Otp { get; init; }
    }
}
