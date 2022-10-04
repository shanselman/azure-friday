using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureFridayDocstoJSON
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // #if DEBUG
            //     Debugger.Launch();
            // #endif
            //<docsnippet_startup>
            var host = new HostBuilder()
                //<docsnippet_configure_defaults>
                // .ConfigureFunctionsWorkerDefaults(builder =>
                // {
                //     //builder
                //         // .AddApplicationInsights()
                //         // .AddApplicationInsightsLogger()
                //         //.AddTimers()
                //         //.AddAzureStorage();
                // })
                //</docsnippet_configure_defaults>
                //<docsnippet_dependency_injection>
                // .ConfigureServices(s =>
                // {
                //     s.AddSingleton<IHttpResponderService, DefaultHttpResponderService>();
                // })
                //</docsnippet_dependency_injection>
                .Build();
            //</docsnippet_startup>

            //<docsnippet_host_run>
            await host.RunAsync();
            //</docsnippet_host_run>
        }
    }
}