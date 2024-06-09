using System.IO;
using Microsoft.Extensions.Configuration;

public class Startup
{
    public AppConfig _appConfig { get; }

    public Startup()
    {
        // Создаем конфигурацию из файла appsettings.json
        string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configFilePath, optional: false, reloadOnChange: true)
            .Build();
        
        IConfigurationSection appSettingsSection = configuration.GetSection("AppConfig");

        // Создаем объект AppSettings и заполняем его значениями из раздела AppConifg в конфигурации
        _appConfig = new AppConfig();
        _appConfig.LoadFilePath = appSettingsSection.GetValue<string?>("LoadFilePath");
        _appConfig.BoardHeight = appSettingsSection.GetValue("BoardHeight", 50);
        _appConfig.BoardWidth = appSettingsSection.GetValue("BoardWidth", 20);
        _appConfig.BoardCellSize = appSettingsSection.GetValue("BoardCellSize", 1);
        _appConfig.BoardLiveDensity = appSettingsSection.GetValue("BoardLiveDensity", 0.5);
    }
}
