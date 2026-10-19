
using LocationTracker.Services;

namespace LocationTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Configure .NET MAUI and Maps.
        builder
            .UseMauiApp<App>()
            .UseMauiMaps();

        // Register SQLite database service.
        builder.Services.AddSingleton(_ => new LocationDatabase(
            Path.Combine(FileSystem.AppDataDirectory, "locations.db3")));

        // Register the main application page.
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
