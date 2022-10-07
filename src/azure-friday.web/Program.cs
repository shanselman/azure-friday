using azure_friday.web.services;
using Microsoft.AspNetCore.Rewrite;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddLazyCache();


builder.Services.AddHttpClient<AzureFridayClient>()
    .AddPolicyHandler(Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).RetryAsync(3))
    .AddPolicyHandler(Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).CircuitBreakerAsync(2, TimeSpan.FromMinutes(1)));

builder.Services.AddSingleton<IAzureFridayDB, AzureFridayDB>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy(new CookiePolicyOptions
{
    CheckConsentNeeded = context => true,
    MinimumSameSitePolicy = SameSiteMode.None
});
app.UseRouting();
app.UseStatusCodePagesWithReExecute("/{0}");
var options = new RewriteOptions()
    .AddRedirect("rssaudio", builder.Configuration["AZURE_FRIDAY_AUDIO_RSS"])
    .AddRedirect("rss", builder.Configuration["AZURE_FRIDAY_RSS"]);
app.UseRewriter(options);
app.UseAuthorization();
app.MapRazorPages();

app.Run();
