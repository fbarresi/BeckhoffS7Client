using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using TFU002.Interfaces.Services;
using TFU002.Logic;
using TFU002.Logic.Services;

namespace TFU002.Service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File( Path.Combine(Directory.Exists(Constants.DefaultDirectory) ? Constants.DefaultDirectory : "", "Service.log"), 
                    rollOnFileSizeLimit: true, 
                    retainedFileCountLimit: 10, 
                    fileSizeLimitBytes: 102400)
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
                    services.AddSingleton<IGatewayService, GatewayService>();
                    services.AddSingleton<IBeckhoffService, BeckhoffService>();
                    services.AddSingleton<IPlcProvider, PlcProvider>();
                    services.AddSingleton<ISettingsProvider, SettingsProvider>();
                    services.AddSingleton<IDirectoryProvider, DirectoryProvider>();
                    services.AddHostedService<Worker>();
                })
        ;
    }
}
