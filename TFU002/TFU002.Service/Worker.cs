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
        private readonly IServiceProvider serviceProvider;
        private readonly IGatewayService gatewayService;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IGatewayService gatewayService)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.gatewayService = gatewayService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Observe Beckhoff for run mode
            // // Read symbols attributes
            // // Create a subscription for every attribute Beckhoff => S7 and S7 => Beckhoff

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //Load Settings and
            //Try Connect with all PLCs
            await InitializeAllServices(serviceProvider).ConfigureAwait(false);

            //Start gateway
            await gatewayService.StartGateway();
            
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            //Dispose all subscriptions

            await base.StopAsync(cancellationToken);
        }

        private async Task InitializeAllServices(IServiceProvider serviceProvider)
        {
            var toInit = GetExtendedInterfacesOf<IInitializable>(typeof(ServiceBase));
            foreach (var type in toInit)
            {
                foreach (var o in serviceProvider.GetServices(type))
                {
                    logger.LogInformation($"Staring initialization for {type.Name}...");
                    try
                    {
                        var service = (IInitializable) o;
                        var result = await service.Initialize();
                        logger.LogInformation($"Initialization of {type.Name} completed with result: {result}");
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Error while initializing service {type.Name}");
                    }
                }
            }
        }

        public static Type[] GetExtendedInterfacesOf<T>(params Type[] exclude)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t))
                .SelectMany(t => t.GetInterfaces())
                .Except(exclude)
                .Except(new []{typeof(IDisposable), typeof(T)})
                .ToArray();
        }


    }
}
