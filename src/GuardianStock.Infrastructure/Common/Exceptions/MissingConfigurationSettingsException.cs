namespace GuardianStock.Infrastructure.Common.Exceptions
{
    internal class MissingConfigurationSettingsException : Exception
    {
        public string Message { get; private set; }
        public MissingConfigurationSettingsException(string key) : base($"Missing configuration settings: Value for {key} is missing.")
        {

            Message = $"Missing configuration settings: Value for {key} is missing.";


        }


    }
}
