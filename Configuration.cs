using Microsoft.Extensions.Configuration;

namespace SpecFlowBookingAPI
{
    public static class Configuration
    {
        private static IConfiguration _config;

        static Configuration()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static string BaseUrl => _config["ApiSettings:BaseUrl"];
        public static int Timeout => int.Parse(_config["ApiSettings:Timeout"]);
        public static string Username => _config["Credentials:Username"];
        public static string Password => _config["Credentials:Password"];
    }
}