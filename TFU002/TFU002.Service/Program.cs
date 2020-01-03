using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using TFU002.Interfaces.Services;
using TFU002.Logic.Services;

namespace TFU002.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File("Service.log", rollOnFileSizeLimit: true, retainedFileCountLimit: 10, fileSizeLimitBytes: 102400)
                .CreateLogger();
            
            CreateHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureLogging(logging => logging.AddSerilog())
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ISettingsProvider, SettingsProvider>();
                    services.AddSingleton<IDirectoryProvider, DirectoryProvider>();
                    services.AddHostedService<Worker>();
                });
    }
}
