using Journal.Authentication;
using Journal.Endpoints;
using Journal.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Nocturne.Database;
using Serilog;
using Serilog.Sinks.SpectreConsole;

namespace Journal;

public class JournalApp
{
    public static readonly string DATA_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
    public static readonly string DATABASE_PATH = Path.Combine(DATA_PATH, "database.nocturne");
    public static readonly string REALM_EXPORTS_PATH = Path.Combine(DATA_PATH, "import/realm_export.json");
    public static readonly string PERSISTENT_KEYS_PATH = Path.Combine(DATA_PATH, "keys");

    public static readonly NocturneDatabase NOCTURNE_DATABASE = new NocturneDatabase
    {
        FilePath = DATABASE_PATH,
        AutomaticallyCompact = true,
        CompactOnLaunch = true
    };

    public static void Main(string[] args)
    {
        Directory.CreateDirectory(DATA_PATH);

        SettingsManager.Load();
        NOCTURNE_DATABASE.Open();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.SpectreConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSerilog();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(PERSISTENT_KEYS_PATH));
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddDiscordAuthentication();

        var app = builder.Build();

        app.MapAuthEndpoints();
        app.MapGoogleHealthEndpoints();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapRazorComponents<Components.App>().AddInteractiveServerRenderMode();

        app.Run();
    }
}
