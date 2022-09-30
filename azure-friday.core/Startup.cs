using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using azure_friday.core.services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Polly;
using Microsoft.AspNetCore.Rewrite;

namespace azure_friday.core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddApplicationInsightsTelemetry();

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddControllers();
            services.AddRazorPages();

            // Register LazyCache
            services.AddLazyCache();

            services.AddHttpClient<AzureFridayClient>()
                .AddTransientHttpErrorPolicy(PolicyBuilder => PolicyBuilder.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)))
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromMinutes(1)
                )
            );

            services.AddSingleton<IAzureFridayDB, AzureFridayDB>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            //app.UseMvc();
            app.UseEndpoints(endpoints => {
                //endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

        }
    }
}
