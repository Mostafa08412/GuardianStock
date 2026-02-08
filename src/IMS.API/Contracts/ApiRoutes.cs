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

        public static class Users
        {
            public const string Base = "users";
            public const string GetById = "{userId}";
            public const string Update = "{userId}";
            public const string Activate = "{userId}/activate";
            public const string Deactivate = "{userId}/deactivate";
        }

    }
}
