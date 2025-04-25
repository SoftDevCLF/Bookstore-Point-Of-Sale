using Microsoft.Extensions.Logging;
using BookstorePointOfSale.Services;

namespace BookstorePointOfSale;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddScoped<ValidationService>();//Dependency injection for customer validation quick access. It is scoped so it will only creates one instance per request
        builder.Services.AddScoped<NavigationService>();
        builder.Services.AddScoped<AlertService>();



#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
