using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Pages;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;
using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using CommunityToolkit.Maui;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        // Load configuration from appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.appsettings.json");
        
        if (stream != null)
        {
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
                
            builder.Configuration.AddConfiguration(config);
        }
        
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // Adds CommunityToolkit.Maui library for additional controls and helpers, like expander
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services - Singleton for API services, one instance for entire app
        builder.Services.AddSingleton<ITeamService, TeamsService>();
        builder.Services.AddSingleton<IPlayerService, PlayerService>();
        
        // Register OpenAI service with API key from configuration
        builder.Services.AddSingleton<IOpenAIService>(_ => 
            new OpenAIService(builder.Configuration["AppSettings:ApiKey"] ?? string.Empty));

        // ViewModels - Singleton used to preserve state between tab switches
        builder.Services.AddSingleton<TeamsViewModel>();
        builder.Services.AddSingleton<PlayersViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<TeamsReportViewModel>();

        // Pages - Transient for UI components, new instance each time a page is navigated to
        builder.Services.AddTransient<TeamsPage>();
        builder.Services.AddTransient<PlayersPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<TeamsReportPage>();

#if DEBUG // Logging - Adds debug logging for development
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}