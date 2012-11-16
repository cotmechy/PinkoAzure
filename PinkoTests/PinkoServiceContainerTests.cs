using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkDao;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.ExceptionTypes;
using PinkoCommon.Interface;
using PinkoExpressionCommon;
using PinkoExpressionEngine;
using PinkoWorkerCommon.RoleHandlers;
using PinkoWorkerCommon.Utility;

namespace PinkoTests
{
    [TestClass]
    public class PinkoServiceContainerTests
    {
        /// <summary>
        /// Assure all objects still registered
        /// </summary>
        [TestMethod]
        public void TestPinkoServiceContainerRegisterCalcEngineExtra()
        {
            var pinkoServiceContainer = new PinkoServiceContainer();
            var pinkoContainer = pinkoServiceContainer.BuildContainer();
            pinkoServiceContainer.RegisterCalcEngineExtra(pinkoContainer);

            Assert.IsNotNull(pinkoContainer.Resolve<IPinkoExpressionEngine>());
            Assert.IsNotNull(pinkoContainer.Resolve<IRxMemoryBus<PinkoExceptionDataNotSubscribed>>());

            Assert.IsNotNull(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>());
            Assert.IsNotNull(pinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgPing>>());

            Assert.IsNotNull(pinkoContainer.Resolve<RoleBusListenerPinkoPingMessageHandle>());
            Assert.IsNotNull(pinkoContainer.Resolve<RoleUserSubscriptionHandler>());
            Assert.IsNotNull(pinkoContainer.Resolve<RoleCalculateExpressionSnapshotHandler>());
            Assert.IsNotNull(pinkoContainer.Resolve<RoleCalculateSubsCalcExpressionHandler>());

            var msgHandler = pinkoContainer.Resolve<IIncominBusMessageHandlerManager>() as IncominBusMessageHandlerManager;
            Assert.IsNotNull(msgHandler);

            Assert.IsTrue(msgHandler.CachedBusPublisher.Values.Count(x => x is InboundTypedPublisher<IBusMessageInbound, PinkoMsgClientTimeout>) == 1);
            Assert.IsTrue(msgHandler.CachedBusPublisher.Values.Count(x => x is InboundTypedPublisher<IBusMessageInbound, PinkoMsgCalculateExpression>) == 1);
            Assert.IsTrue(msgHandler.CachedBusPublisher.Values.Count(x => x is InboundTypedPublisher<IBusMessageInbound, PinkoMsgClientConnect>) == 1);
            Assert.IsTrue(msgHandler.CachedBusPublisher.Values.Count(x => x is InboundTypedPublisher<IBusMessageInbound, PinkoMsgPing>) == 1);
            //Assert.IsTrue(msgHandler.CachedBusPublisher.Values.Count(x => x is InboundTypedPublisher<IBusMessageInbound, string>) == 1);
            Assert.IsTrue(msgHandler.CachedBusPublisher.Values.Count(x => x is InboundTypedPublisher<IBusMessageInbound, PinkoMsgRoleHeartbeat>) == 1);
            Assert.IsTrue(msgHandler.CachedBusPublisher.Values.Count(x => x is InboundTypedPublisher<IBusMessageInbound, PinkoMsgCalculateExpressionResult>) == 1);
        }
    }
}
