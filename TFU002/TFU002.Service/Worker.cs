using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TFU002.Interfaces.Basics;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;

namespace TFU002.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly ISettingsProvider settingsProvider;
        private readonly IServiceProvider serviceProvider;

        public Worker(ILogger<Worker> logger, ISettingsProvider settingsProvider, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.settingsProvider = settingsProvider;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Observe Beckhoff for run mode
            // // Read symbols attributes
            // // Create a subsriprion for evcery attribute Beckhoff => S7 and S7 => Beckhoff

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            //Load Settings
            // var initializables = serviceProvider.GetServices<ISettingsProvider>();
            await InitializeAllServices(serviceProvider);
            var settings = settingsProvider.Settings;
            //Try Connect with all PLCs

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            //Dispose all subscriptions

            await base.StopAsync(cancellationToken);
        }

        private async Task InitializeAllServices(IServiceProvider serviceProvider)
        {
            var assemblyLocation = Assembly.GetAssembly(this.GetType()).Location;
            var directory = Directory.GetParent(assemblyLocation);
            logger.LogInformation($"Loading interfaces from path: {directory}");
            
            var toInit = GetInterfacesThatImplements<IInitializable>(typeof(IInitializable), typeof(ServiceBase));
            foreach (var type in toInit)
            {
                logger.LogInformation($"Staring initialization for {type.Name}...");
                try
                {
                    var service = (IInitializable)serviceProvider.GetService(type);
                    var result = await service.Initialize();
                    logger.LogInformation($"Initialization of {type.Name} completed with result: {result}");
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error while initializing service {type.Name}");
                }
                
            }
        }

        public Type[] GetInterfacesThatImplements<T>(params Type[] exclude)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t))
                .SelectMany(t => t.GetInterfaces())
                .Except(exclude)
                .Except(new []{typeof(IDisposable)})
                .ToArray();
        }


    }
}
