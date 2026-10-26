using Gasta;
using Gasta.Shared.Abstractions;
using Gasta.Shared.Data.IndexedDb;
using Gasta.Shared.Data.Repositories;
using Gasta.Shared.Platform.Web;
using Gasta.Shared.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// --- Platform services (web). The MAUI host registers its own versions of these. ---
builder.Services.AddScoped<IKeyValueStore, LocalStorageKeyValueStore>();
builder.Services.AddScoped<IFileExportService, BlobDownloadExportService>();
builder.Services.AddScoped<IAppReloader, BrowserAppReloader>();
builder.Services.AddScoped<IndexedDbService>();
builder.Services.AddScoped<IAppDatabase, IndexedDbAppDatabase>();
builder.Services.AddScoped<IExpenseRepository, IndexedDbExpenseRepository>();
builder.Services.AddScoped<ICategoryRepository, IndexedDbCategoryRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, IndexedDbPaymentMethodRepository>();
builder.Services.AddScoped<IBudgetRepository, IndexedDbBudgetRepository>();
builder.Services.AddScoped<ICategoryBudgetRepository, IndexedDbCategoryBudgetRepository>();

// --- App services (shared by every host) ---
builder.Services.AddScoped<ExpenseSummaryService>();
builder.Services.AddScoped<SeedService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<AccentColorService>();
builder.Services.AddScoped<AddExpenseUiService>();
builder.Services.AddSingleton<AlertService>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<AppInitializer>();

var host = builder.Build();

// Everything AppInitializer does (open DB, seed, apply saved theme) MUST finish before
// RunAsync() so no page's OnInitializedAsync can ever read the database mid-seed.
//
// Resolve straight from host.Services (the root provider) rather than a manually
// created child scope. Blazor WASM only really has one long-lived scope for the app's
// whole session, so a manually created-and-disposed scope tears down its own separate
// IndexedDbService instance (and the JS module reference it holds) right after this
// block, which is what threw the AsyncDisposableServiceDispose error. Resolving from
// the root keeps the same IndexedDbService instance alive for the rest of the app.
await host.Services.GetRequiredService<AppInitializer>().InitializeAsync();

// Run THIS host. The repo previously ended with `await builder.Build().RunAsync();`,
// which builds a second host and runs that one, so the services initialised above
// (e.g. ThemeService.IsDark) were thrown away.
await host.RunAsync();
