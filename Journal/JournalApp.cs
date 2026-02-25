using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Journal.API;
using Journal.Database;
using Journal.Database.Models;
using Journal.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Realms;

namespace Journal;

public class JournalApp
{
    // List of changes:
    // 2 - Added `SleepStart` and `SleepEnd` so journal entries can be edited
    // 3 - Added `MoodRating` and `ProductivityRating` to `MentalHealthTrackerEntry`
    // 4 - Added `Sunrise`, `Sunset` and `Weather` to `JournalEntry`
    // 5 - Changed `Weather` to an enum
    public const ulong SCHEMA_VERSION = 5;

    public static readonly string DATA_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

    public static RealmConfiguration RealmConfig = null!;

    public static void Main(string[] args)
    {
        Directory.CreateDirectory(DATA_PATH);

        SettingsManager.Load();

        RealmConfig = new RealmConfiguration(Path.Combine(DATA_PATH, "database.realm"))
        {
            SchemaVersion = SCHEMA_VERSION,
            MigrationCallback = RealmAccess.Migrate
        };

        Realm.Compact(RealmConfig);


        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton(RealmConfig);
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/app/data/keys/"));
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => { options.LoginPath = "/login"; })
            .AddDiscord(options =>
            {
                options.ClientId = SettingsManager.Config.DiscordAuthSettings.Value!.ClientId.ToString();
                options.ClientSecret = SettingsManager.Config.DiscordAuthSettings.Value!.Secret;

                options.SaveTokens = true;
                options.Scope.Add("identify");
                options.ClaimActions.MapJsonKey("urn:discord:id", "id");

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                options.Events.OnCreatingTicket = async context =>
                {
                    var data = await DiscordApi.GetData(context.AccessToken!);
                    if (data == null) throw new InvalidDataException("data is null");

                    if (context.Principal?.Identity is not ClaimsIdentity identity) return;

                    var realmConfiguration = context.HttpContext.RequestServices.GetRequiredService<RealmConfiguration>();
                    var realm = await Realm.GetInstanceAsync(realmConfiguration);
                    await realm.WriteAsync(() =>
                    {
                        var user = realm.Find<RealmUser>(data.DiscordId);
                        if (user == null)
                        {
                            realm.Add(data!);
                        }
                        else
                        {
                            user.DisplayName = data.DisplayName;
                            user.ProfileImageUrl = data.ProfileImageUrl;
                            user.Username = data.Username;
                        }

                        identity.AddClaim(new Claim("urn:discord:id", data.DiscordId.ToString()));
                    });
                };
            });

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        var app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.MapGet("/login", async context =>
        {
            if (!(context.User?.Identity?.IsAuthenticated ?? false))
            {
                await context.ChallengeAsync(DiscordAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            }
            else
            {
                context.Response.Redirect("/");
            }
        });

        app.MapGet("/logout", async context =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // god why cant blazor just fucking fully refresh the page when I want it to
            // why do I have to do these fucking hacks
            // (do love blazor otherwise tho)
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(@"<!DOCTYPE html><html style='background: black'><script>window.location.href = '/'; </script></html>");
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
