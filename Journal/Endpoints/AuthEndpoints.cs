// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Journal.Endpoints;

public static class AuthEndpoints
{
    private const string content_type = "text/html";
    private const string reload_page_html = @"<!DOCTYPE html><html style='background: black'><script>window.location.href = '/'; </script></html>";

    public const string ROUTE_LOGIN = "/login";
    public const string ROUTE_LOGOUT = "/logout";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(ROUTE_LOGIN, async context =>
        {
            if (!(context.User.Identity?.IsAuthenticated ?? false))
            {
                await context.ChallengeAsync(DiscordAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            }
            else
            {
                context.Response.Redirect("/");
            }
        });

        app.MapGet(ROUTE_LOGOUT, async context =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ugly hack but blazor REFUSES to actually refresh a page normally
            context.Response.ContentType = content_type;
            await context.Response.WriteAsync(reload_page_html);
        });
    }
}
