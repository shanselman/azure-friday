using azure_friday.core.services;
using Microsoft.AspNetCore.Rewrite;
using Polly;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

services.AddControllers();
services.AddRazorPages();

// Register LazyCache
services.AddLazyCache();

// Register Http Client with Polly circuit breaker pattern
services.AddHttpClient<AzureFridayClient>()
    .AddTransientHttpErrorPolicy(PolicyBuilder => PolicyBuilder.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)))
    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 2,
        durationOfBreak: TimeSpan.FromMinutes(1)
    )
);

services.AddSingleton<IAzureFridayDB, AzureFridayDB>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseStatusCodePagesWithReExecute("/{0}");
app.UseHttpsRedirection();
app.UseCookiePolicy();

var options = new RewriteOptions()
    .AddRedirect("rssaudio", "https://hanselstorage.blob.core.windows.net/output/azurefridayaudio.rss")
    .AddRedirect("rss", "https://hanselstorage.blob.core.windows.net/output/azurefriday.rss");
app.UseRewriter(options);

app.MapControllers();
app.MapRazorPages();

app.Run();