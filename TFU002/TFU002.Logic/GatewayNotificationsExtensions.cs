using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;
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
                    .SelectMany(value => beckhoff.WriteValueAsync(symbol, value, CancellationToken.None))
                    .Subscribe();

            var typecode = Type.GetTypeCode(type);
            switch (typecode)
            {
                case TypeCode.Boolean:
                    return plc.CreateNotification<bool>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.WriteValueAsync(symbol, value, CancellationToken.None))
                        .Subscribe();
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Char:
                    return plc.CreateNotification<byte>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.WriteValueAsync(symbol, value, CancellationToken.None))
                        .Subscribe();
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return plc.CreateNotification<short>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.WriteValueAsync(symbol, value, CancellationToken.None))
                        .Subscribe();
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return plc.CreateNotification<int>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.WriteValueAsync(symbol, value, CancellationToken.None))
                        .Subscribe();
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return plc.CreateNotification<long>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.WriteValueAsync(symbol, value, CancellationToken.None))
                        .Subscribe();
                case TypeCode.Single:
                    return plc.CreateNotification<float>(address, TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100))
                        .SelectMany(value => beckhoff.WriteValueAsync(symbol, value, CancellationToken.None))
                        .Subscribe();
                default:
                    throw new ArgumentException($"Unsupported Type {type.Name}");
            }
        }

        public static IDisposable GetTypedBeckhoffNotification(this AdsClient beckhoff, ISymbol symbol, Type type, IPlc plc, string address)
        {
            if (type == typeof(byte[]))
                return beckhoff.WhenNotification<byte[]>(symbol.InstancePath, NotificationSettings.Default)
                    .SelectMany(async value =>
                    {
                        await plc.SetValue(address, value);
                        return Unit.Default;
                    })
                    .Subscribe();

            var typecode = Type.GetTypeCode(type);
            switch (typecode)
            {
                case TypeCode.Boolean:
                    return beckhoff.WhenNotification<bool>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(async value =>
                        {
                            await plc.SetValue(address, value);
                            return Unit.Default;
                        })
                        .Subscribe();
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return beckhoff.WhenNotification<byte>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(async value =>
                        {
                            await plc.SetValue(address, value);
                            return Unit.Default;
                        })
                        .Subscribe();
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return beckhoff.WhenNotification<short>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(async value =>
                        {
                            await plc.SetValue(address, value);
                            return Unit.Default;
                        })
                        .Subscribe();
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return beckhoff.WhenNotification<int>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(async value =>
                        {
                            await plc.SetValue(address, value);
                            return Unit.Default;
                        })
                        .Subscribe();
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return beckhoff.WhenNotification<long>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(async value =>
                        {
                            await plc.SetValue(address, value);
                            return Unit.Default;
                        })
                        .Subscribe();
                case TypeCode.Single:
                    return beckhoff.WhenNotification<float>(symbol.InstancePath, NotificationSettings.Default)
                        .SelectMany(async value =>
                        {
                            await plc.SetValue(address, value);
                            return Unit.Default;
                        })
                        .Subscribe();
                default:
                    throw new ArgumentException($"Unsupported Type {type.Name}");
            }
        }
    }
}
