using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;
using TFU002.Interfaces.Extensions;
using TFU002.Interfaces.Services;
using TFU002.Logic.Basics;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace TFU002.Logic.Services
{
    public class GatewayService : DisposableBase, IGatewayService
    {
        private readonly ILogger logger;
        private readonly IPlcProvider plcProvider;
        private readonly IBeckhoffService beckhoffService;
        private readonly SerialDisposable observerSubscriptions = new SerialDisposable();
        public GatewayService(ILogger<GatewayService> logger, 
                                IPlcProvider plcProvider, 
                                IBeckhoffService beckhoffService)
        {
            this.logger = logger;
            this.plcProvider = plcProvider;
            this.beckhoffService = beckhoffService;
            
            observerSubscriptions.AddDisposableTo(Disposables);
        }

        public Task StartGateway()
        {
            beckhoffService.Symbols
                .Do(_ => observerSubscriptions.Disposable = null)
                .Where(symbols => symbols != null)
                .SelectMany(GetS7Variables)
                .Select(GenerateObservers)
                .Do(subscriptionsDisposable => observerSubscriptions.Disposable = subscriptionsDisposable)
                .LogAndRetryAfterDelay(logger, TimeSpan.FromMilliseconds(100), "Error while generating gateway")
                .Subscribe()
                .AddDisposableTo(Disposables);
            return Task.FromResult(Unit.Default);
        }

        private IDisposable GenerateObservers(List<GatewayVariable> variables)
        {
            var subscriptions = new CompositeDisposable();
            
            foreach (var gatewayVariable in variables)
            {
                var plc = string.IsNullOrEmpty(gatewayVariable.PlcName) ? plcProvider.GetPlc() : plcProvider.GetPlc(gatewayVariable.PlcName);
                
                if(gatewayVariable.Direction == Direction.Input)
                {
                    plc.GetTypedS7Notification(gatewayVariable.TargetType, gatewayVariable.S7Address, beckhoffService.Client, gatewayVariable.Symbol)
                        .AddDisposableTo(subscriptions);
                }

                if(gatewayVariable.Direction == Direction.Output)
                {
                    beckhoffService.Client.GetTypedBeckhoffNotification(gatewayVariable.Symbol, gatewayVariable.TargetType, plc, gatewayVariable.S7Address)
                        .AddDisposableTo(subscriptions);
                }
            }

            return subscriptions;
        }

        private Task<List<GatewayVariable>> GetS7Variables(ISymbolCollection<ISymbol> symbols)
        {
            var iterator = new SymbolIterator(symbols, 
                s => s.InstancePath.Split('.').Length == 2 && s.Attributes.Any(attribute => attribute.Name.StartsWith("S7")));
            return Task.FromResult(iterator.Select(ConvertToGatewayVariale).Where(variable => variable.IsValid).ToList());
        }

        private GatewayVariable ConvertToGatewayVariale(ISymbol symbol)
        {
            var variable = new GatewayVariable 
            { 
                Symbol = symbol,
                TargetType = symbol.IsPrimitiveType ? ((DataType)symbol.DataType).ManagedType : null
            };
            if (symbol.Attributes.Any(attribute => attribute.Name.Equals("S7.In", StringComparison.InvariantCultureIgnoreCase)))
            {
                variable.Direction = Direction.Input;
            }

            if (symbol.Attributes.Any(attribute => attribute.Name.Equals("S7.Out", StringComparison.InvariantCultureIgnoreCase)))
            {
                variable.Direction = Direction.Output;
            }

            if (symbol.Attributes.Any(attribute => attribute.Name.Equals("S7.Address", StringComparison.InvariantCultureIgnoreCase)))
            {
                variable.S7Address = symbol.Attributes.First(attribute => attribute.Name.Equals("S7.Address", StringComparison.InvariantCultureIgnoreCase)).Value;
            }

            if (symbol.Attributes.Any(attribute => attribute.Name.Equals("S7.Plc", StringComparison.InvariantCultureIgnoreCase)))
            {
                variable.PlcName = symbol.Attributes.First(attribute => attribute.Name.Equals("S7.Plc", StringComparison.InvariantCultureIgnoreCase)).Value;
            }
            
            logger.LogInformation($"Converted Symbol to Gateway {variable}");
            return variable;
        }
    }

    internal class GatewayVariable
    {
        public Direction Direction { get; set; }
        public string S7Address { get; set; }
        public string PlcName { get; set; }
        public ISymbol Symbol { get; set; }
        public Type TargetType { get; set; }
        public bool IsValid => (Direction == Direction.Input || Direction == Direction.Output) 
                                && TargetType != null
                                && !string.IsNullOrEmpty(S7Address);

        public override string ToString()
        {
            return $"{Symbol.InstancePath} to {Direction} {PlcName}@{S7Address} of Type {TargetType} (isValid = {IsValid})";
        }
    }

    internal enum Direction
    {
        Input,
        Output
    }
}