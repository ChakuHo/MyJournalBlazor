using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using MyJournalBlazor.Services; 

namespace MyJournalBlazor
{
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

            // 1. Blazor Setup
            builder.Services.AddMauiBlazorWebView();

            // 2. MudBlazor UI Library
            builder.Services.AddMudServices();

            // 3. Rich Text Editor (Quill)
            //builder.Services.AddBlazoredTextEditor(); 

            // 4. DATABASE SERVICE
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

            // 5. USER / SECURITY SERVICE
            builder.Services.AddSingleton<UserService>();

            // 6. PDF EXPORT SERVICE
            builder.Services.AddSingleton<PdfService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}