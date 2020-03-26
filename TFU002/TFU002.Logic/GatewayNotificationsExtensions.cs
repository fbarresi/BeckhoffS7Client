using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;
using TFU002.Interfaces.Extensions;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.TypeSystem;

namespace TFU002.Logic
{
    public static class GatewayNotificationsExtensions
    {
        public static IDisposable GetTypedS7Notification(this IPlc plc, Type type, string address, AdsClient beckhoff, ISymbol symbol)
        {
            if (type == typeof(byte[]))
                return plc.CreateNotification<byte[]>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                    .SelectMany(value => beckhoff.Write(symbol, value))
                    .Subscribe();

            var typecode = Type.GetTypeCode(type);
            switch (typecode)
            {
                case TypeCode.Boolean:
                    return plc.CreateNotification<bool>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Char:
                    return plc.CreateNotification<byte>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return plc.CreateNotification<short>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return plc.CreateNotification<int>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return plc.CreateNotification<long>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                case TypeCode.Single:
                    return plc.CreateNotification<float>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.Write(symbol, value))
                        .Subscribe();
                default:
                    throw new ArgumentException($"Unsupported Type {type.Name}");
            }
        }

        public static IDisposable GetTypedBeckhoffNotification(this AdsClient beckhoff, ISymbol symbol, Type type, IPlc plc, string address)
        {
            if (type == typeof(byte[]))
            {
                return beckhoff.WhenNotification<byte[]>(symbol.InstancePath, NotificationSettings.Default)
                    .SelectMany(value => plc.Write(address, value))
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
    }
}
