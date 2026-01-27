namespace IMS.API.Contracts
{
    public static class ApiRoutes
    {
        public static class Authentication
        {
            public const string Base = $"auth";

            public const string Register = $"register";
            public const string Login = $"login";
            public const string Logout = $"logout";
            public const string RefreshToken = $"refresh-token";

            public const string ChangePassword = $"change-password";
        }

    }
}
