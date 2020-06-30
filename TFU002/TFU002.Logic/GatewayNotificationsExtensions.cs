using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.TypeSystem;

namespace TFU002.Logic
{
    public static class GatewayNotificationsExtensions
    {
        public static IDisposable GetTypedS7Notification(this IPlc plc, Type type, string address, AdsClient beckhoff, ISymbol symbol, TimeSpan notificationCycle, TimeSpan regularTransmissionCycle)
        {
            if (type == typeof(byte[]))
            {
                return plc.CreateNotification<byte[]>(address, TransmissionMode.Cyclic, regularTransmissionCycle)
                    .Do(value => Log.Logger.Debug($"Writing {address} to {symbol.InstancePath} {ByteToString(value)}"))
                    .SelectMany(value => beckhoff.Write(symbol, value))
                    .Subscribe();
            }

            var typecode = Type.GetTypeCode(type);
            switch (typecode)
            {
                case TypeCode.Boolean:
                    return plc.CreateNotification<bool>(address, TransmissionMode.OnChange, notificationCycle)
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Char:
                    return plc.CreateNotification<byte>(address, TransmissionMode.OnChange, notificationCycle)
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return plc.CreateNotification<short>(address, TransmissionMode.OnChange, notificationCycle)
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return plc.CreateNotification<int>(address, TransmissionMode.OnChange, notificationCycle)
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return plc.CreateNotification<long>(address, TransmissionMode.OnChange, notificationCycle)
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Single:
                    return plc.CreateNotification<float>(address, TransmissionMode.OnChange, notificationCycle)
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                default:
                    throw new ArgumentException($"Unsupported Type {type.Name}");
            }
        }

        public static IDisposable GetTypedBeckhoffNotification(this AdsClient beckhoff, ISymbol symbol, Type type, IPlc plc, string address, TimeSpan regularTransmissionCycle)
        {
            if (type == typeof(byte[]))
            {
                return Observable.Timer(TimeSpan.FromMilliseconds(100), regularTransmissionCycle)
                    .Select(_ => beckhoff.ReadVariable<byte[]>(symbol))
                    .Do(value => Log.Logger.Debug($"Writing {symbol.InstancePath} to {address}: {ByteToString(value)}"))
                    .SelectMany(value => plc.Write(address, value))
                    .Retry()
                    .Subscribe();
            }

            var typecode = Type.GetTypeCode(type);
            switch (typecode)
            {
                case TypeCode.Boolean:
                    return beckhoff.WhenNotification<bool>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(value => plc.Write(address, value))
                        .Subscribe();
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return beckhoff.WhenNotification<byte>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(value => plc.Write(address, value))
                        .Subscribe();
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return beckhoff.WhenNotification<short>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(value => plc.Write(address, value))
                        .Subscribe();
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return beckhoff.WhenNotification<int>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(value => plc.Write(address, value))
                        .Subscribe();
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return beckhoff.WhenNotification<long>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(value => plc.Write(address, value))
                        .Subscribe();
                case TypeCode.Single:
                    return beckhoff.WhenNotification<float>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(value => plc.Write(address, value))
                        .Subscribe();
                default:
                    throw new ArgumentException($"Unsupported Type {type.Name}");
            }
        }

        private static string ByteToString(byte[] value)
        {
            return string.Join(" ", value.Select(b => b.ToString("X2")));
        }

        private static async Task<Unit> Write<T>(this AdsClient beckhoff, ISymbol symbol, T value)
        {
            try
            {
                await beckhoff.WriteValueAsync(symbol, value, CancellationToken.None);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error while writing Beckhoff value {value} into {symbol.InstancePath}");
            }
            return Unit.Default;
        }
        
        private static async Task<Unit> Write<T>(this IPlc plc, string address, T value)
        {
            try
            {
                await plc.SetValue(address, value);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error while writing plc value {value} into {address}");
            }
            return Unit.Default;
        }

        private static T ReadVariable<T>(this AdsClient beckhoff, ISymbol symbol)
        {
            try
            {
                return (T) beckhoff.ReadValue(symbol);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error while reading symbol {symbol.InstancePath}");
                throw;
            }
        }
    }
}
