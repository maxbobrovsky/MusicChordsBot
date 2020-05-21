using System.Configuration;

namespace AccordsBot
{
    class AppSettings
    {
        public static string TelegramToken = ConfigurationManager.AppSettings.Get("TelegramToken");
        
        public static string MSSQLConnectionString = $"Data Source={ConfigurationManager.AppSettings.Get("ServerName")};" +
                                                    $"Initial Catalog={ConfigurationManager.AppSettings.Get("DataBaseName")};" +
                                                    $"User Id={ConfigurationManager.AppSettings.Get("UserName")};" +
                                                    $"Password={ConfigurationManager.AppSettings.Get("Password")};";

        public static string TelegraphToken = ConfigurationManager.AppSettings.Get("TelegraphToken");

    }
}
