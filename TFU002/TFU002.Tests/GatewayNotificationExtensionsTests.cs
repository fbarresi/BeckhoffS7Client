using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reactive.Linq;
using Moq;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;
using TFU002.Logic;
using Xunit;

namespace TFU002.Tests
{
    public class GatewayNotificationExtensionsTests
    {
        public static Mock<IPlc> CreatePlcMock<T>()
        {
            var mock = new Mock<IPlc>();
            mock.Setup(plc => plc.CreateNotification<T>(It.IsAny<string>(), It.IsAny<TransmissionMode>(), It.IsAny<TimeSpan>()))
                .Returns(Observable.Never<T>());
            return mock;
        }
        
        [Theory]
        [ClassData(typeof(TestTypeData))]
        public void CreateTypedPlcNotification(Type type)
        {
            var plcMock = (Mock<IPlc>)typeof(GatewayNotificationExtensionsTests).GetMethod("CreatePlcMock")?.MakeGenericMethod(type).Invoke(null, null);

            plcMock.Object
                .GetTypedS7Notification(type, "address", null, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
            
         
            if (type == typeof(byte[]))
            {
                plcMock.Verify(plc => plc.CreateNotification<byte[]>("address", TransmissionMode.Cyclic, TimeSpan.FromMilliseconds(100)));
                return;
            }

            var typecode = Type.GetTypeCode(type);
            switch (typecode)
            {
                case TypeCode.Boolean:
                    plcMock.Verify(plc => plc.CreateNotification<bool>("address", TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100)));
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Char:
                    plcMock.Verify(plc => plc.CreateNotification<byte>("address", TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100)));
                    break;
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    plcMock.Verify(plc => plc.CreateNotification<short>("address", TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100)));
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    plcMock.Verify(plc => plc.CreateNotification<int>("address", TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100)));
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    plcMock.Verify(plc => plc.CreateNotification<long>("address", TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100)));
                    break;
                case TypeCode.Single:
                    plcMock.Verify(plc => plc.CreateNotification<float>("address", TransmissionMode.OnChange, TimeSpan.FromMilliseconds(100)));
                    break;
                default:
                    throw new ArgumentException($"Unsupported Type {type.Name}");
            }
        }
        
    }
    
    public class TestTypeData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { typeof(byte[]) };
            yield return new object[] { typeof(byte) };
            yield return new object[] { typeof(bool) };
            yield return new object[] { typeof(int) };
            yield return new object[] { typeof(short) };
            yield return new object[] { typeof(float) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
