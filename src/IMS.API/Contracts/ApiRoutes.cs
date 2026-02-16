namespace IMS.API.Contracts
{
    public static class ApiRoutes
    {

        public const string Root = "api/v{version:apiVersion}";
        public static class Authentication
        {
            public const string Base = Root + $"/auth";

            public const string Register = $"register";
            public const string Login = $"login";
            public const string Logout = $"logout";
            public const string RefreshToken = $"refresh-token";
            public const string ChangePassword = $"change-password";
            public const string GoogleLogin = $"google-login";
            public const string UpdateProfile = $"update-profile";
            public const string ForgetPassword = $"forget-password";
            public const string VerifyResetPasswordOtp = $"verify-reset-password-otp";
            public const string ResetPassword = $"reset-password";
        }

        public static class Users
        {
            public const string Base = Root + $"/users";
            public const string GetById = "{userId}";
            public const string Create = "";
            public const string Update = "{userId}";
            public const string Activate = "{userId}/activate";
            public const string Deactivate = "{userId}/deactivate";
        }


        public static class Categories
        {
            public const string Base = Root + $"/categories";
            public const string GetAll = "";
            public const string Create = "";
            public const string GetById = "{categoryId}";
            public const string Update = "{categoryId}";
            public const string Delete = "{categoryId}";
        }
        public static class Products
        {
            public const string Base = Root + $"/products";
            public const string GetAll = "";
            public const string Create = "";
            public const string GetById = "{productId}";
            public const string GetBySku = "{productSku}";
            public const string Update = "{productId}";
            public const string Delete = "{productId}";
            public const string Upload = "import-preview";
            public const string ConfirmImport = "confirm-import";
        }

        public static class Dashboard
        {
            public const string Base = Root + $"/dashboard";
            public const string GetStats = "";
        }

        public static class Transactions
        {
            public const string Base = Root + $"/transactions";
            public const string GetAll = "";
            public const string GetById = "{transactionId}";
            public const string RecordSale = "sale";
            public const string RecordPurchase = "purchase";
        }

        public static class Inventories
        {
            public const string Base = Root + $"/inventories";
            public const string GetAll = "";
            public const string GetById = "{inventoryId}";
            public const string GetSummary = "summary";
            public const string AdjustThreshold = "{inventoryId}/adjust-threshold";
        }

        public static class Alerts
        {
            public const string Base = Root + $"/alerts";
            public const string GetAll = "";
            public const string Dismiss = "{inventoryId}/dismiss";
        }
    }
}
