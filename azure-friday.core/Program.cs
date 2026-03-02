using azure_friday.core.services;
using Microsoft.AspNetCore.Rewrite;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddLazyCache();

builder.Services.AddHttpClient<AzureFridayClient>()
    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)))
    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 2,
        durationOfBreak: TimeSpan.FromMinutes(1)
    ));

builder.Services.AddSingleton<IAzureFridayDB, AzureFridayDB>();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseResponseCompression();

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self'; " +
        "img-src 'self' https: data:; " +
        "font-src 'self'; " +
        "connect-src 'self'; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'; " +
        "upgrade-insecure-requests;";
    await next();
});

// Domain restriction middleware
app.Use(async (context, next) =>
{
    var host = context.Request.Host.Host;

    if (host.Equals("azurefriday.com", StringComparison.OrdinalIgnoreCase) ||
        host.Equals("www.azurefriday.com", StringComparison.OrdinalIgnoreCase))
    {
        await next();
    }
    else if (host.Equals("its-azure-friday.azurewebsites.net", StringComparison.OrdinalIgnoreCase))
    {
        var scheme = context.Request.Scheme;
        var pathAndQuery = context.Request.Path + context.Request.QueryString;
        context.Response.Redirect($"{scheme}://azurefriday.com{pathAndQuery}", permanent: true);
    }
    else
    {
        await next();
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 hour, but revalidate with the server
        ctx.Context.Response.Headers.CacheControl = "public, max-age=3600, must-revalidate";
    }
});
app.UseRouting();
app.UseStatusCodePagesWithReExecute("/{0}");
app.UseHttpsRedirection();

var rewriteOptions = new RewriteOptions()
    .AddRedirect("rssaudio", "https://hanselstorage.blob.core.windows.net/output/azurefridayaudio.rss")
    .AddRedirect("rss", "https://hanselstorage.blob.core.windows.net/output/azurefriday.rss");
app.UseRewriter(rewriteOptions);

app.MapControllers();
app.MapRazorPages();

app.Run();

// Make the implicit Program class public so test projects can reference it
public partial class Program { }
