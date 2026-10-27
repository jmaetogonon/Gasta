using Gasta.Maui.Data;
using Gasta.Maui.Services;
using Gasta.Shared.Abstractions;
using Gasta.Shared.Data.Repositories;
using Gasta.Shared.Services;
using Microsoft.Extensions.Logging;

namespace Gasta.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();

        // --- Platform services -------------------------------------------------------
        // Data: SQLite file in the app's private storage (Android Auto Backup includes it).
        // Settings: Android SharedPreferences. Only these registrations differ from the web host.
        builder.Services.AddSingleton(_ => new SqliteDatabase(Path.Combine(FileSystem.AppDataDirectory, "gasta.db3")));
        builder.Services.AddSingleton<IAppDatabase>(sp => sp.GetRequiredService<SqliteDatabase>());
        builder.Services.AddScoped<IExpenseRepository, SqliteExpenseRepository>();
        builder.Services.AddScoped<ICategoryRepository, SqliteCategoryRepository>();
        builder.Services.AddScoped<IPaymentMethodRepository, SqlitePaymentMethodRepository>();
        builder.Services.AddScoped<IBudgetRepository, SqliteBudgetRepository>();
        builder.Services.AddScoped<ICategoryBudgetRepository, SqliteCategoryBudgetRepository>();
        builder.Services.AddScoped<IKeyValueStore, PreferencesKeyValueStore>();

        // A WebView can't download a blob or reload itself the way a browser tab can.
        builder.Services.AddScoped<IFileExportService, MauiFileExportService>();
        builder.Services.AddScoped<IAppReloader, MauiAppReloader>();
#if ANDROID
        builder.Services.AddSingleton<ISystemBars, AndroidSystemBars>();
#else
        builder.Services.AddSingleton<ISystemBars, NoOpSystemBars>();
#endif

        // --- App services (identical to the web host's Program.cs) -------------------
        builder.Services.AddScoped<ExpenseSummaryService>();
        builder.Services.AddScoped<SeedService>();
        builder.Services.AddScoped<ThemeService>();
        builder.Services.AddScoped<AccentColorService>();
        builder.Services.AddScoped<AddExpenseUiService>();
        builder.Services.AddSingleton<AlertService>();
        builder.Services.AddScoped<UserProfileService>();
        builder.Services.AddScoped<AppInitializer>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
