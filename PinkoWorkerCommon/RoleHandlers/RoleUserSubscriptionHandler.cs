using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Extension;
using PinkoCommon.Interface;
using PinkoCommon.Interface.Storage;
using PinkoCommon.Utility;

namespace PinkoWorkerCommon.RoleHandlers
{
    /// <summary>
    /// Role Agent
    /// </summary>
    public class RoleUserSubscriptionHandler
    {
        /// <summary>
        /// Initialize and register role handler
        /// </summary>
        /// <returns></returns>
        public RoleUserSubscriptionHandler Register()
        {
            Debug.Assert(PinkoContainer.Registrations.Count(x => x.RegisteredType == typeof(InboundMessageReactiveListener<PinkoMsgCalculateExpression>)) == 1);
            Debug.Assert(PinkoContainer.Registrations.Count(x => x.RegisteredType == typeof(InboundMessageReactiveListener<PinkoMsgClientConnect>)) == 1);
            Debug.Assert(PinkoContainer.Registrations.Count(x => x.RegisteredType == typeof(InboundMessageReactiveListener<PinkoMsgClientPing>)) == 1);

            var handler = PinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgCalculateExpression>>();

            HandlerPartitionId = "RoleUserSubscriptionHandler_" + handler.MsgHandlerId;

            //
            // Handle: PinkoMsgCalculateExpression
            //
            handler
                .Subscribers.Add(new Handlesubscriber<PinkoMsgCalculateExpression>()
                {
                    FilterCallback = (m, p) => p.MsgAction == PinkoMessageAction.UserSubscription,
                    Callback = (m, e) => ProcessSubscribe(m, e)
                });

            //
            // Handle: PinkoMsgClientConnect 
            //
            PinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientConnect>>()
                .Subscribers.Add(new Handlesubscriber<PinkoMsgClientConnect>()
                {
                    Callback = (m, e) => ProcessConnectionChange(m, e)
                });

            //
            // Handle: PinkoMsgClientPing 
            //
            PinkoContainer.Resolve<InboundMessageReactiveListener<PinkoMsgClientPing>>()
                .Subscribers.Add(new Handlesubscriber<PinkoMsgClientPing>()
                {
                    Callback = (m, e) => ProcessPinkoMsgClientPing(m, e)
                });

            
            // start monitoring client timeouts.  Client must send a ping message on specified interval
            _timeoutCheck = PinkoApplication.RunActionInTimer(PinkoConfiguration.ClientTimeoutThresholdIntervalMs, () => CheckClientTimeout(DateTime.Now));

            return this;
        }



        /// <summary>
        /// Process PinkoMsgClientPing
        /// </summary>
        public IBusMessageOutbound ProcessPinkoMsgClientPing(IBusMessageInbound envelop, PinkoMsgClientPing pinkoMsgClientPing)
        {
            var subs = ClientSubscriptions[pinkoMsgClientPing.DataFeedIdentifier.SubscribtionId];

            // Reset date to latest
            if (subs.IsNotNull() && subs.LastPing < pinkoMsgClientPing.PingTime)
                subs.LastPing = pinkoMsgClientPing.PingTime;

            return null;
        }



        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public IBusMessageOutbound ProcessSubscribe(IBusMessageInbound envelop, PinkoMsgCalculateExpression expression)
        {
            var response = (IBusMessageOutbound)envelop;
            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);

            response.Message = resultMsg;
            response.WebRoleId = expression.DataFeedIdentifier.WebRoleId;

            PinkoMsgCalculateExpression updatedExpMsg = null;
            var isNew = false;

            //  - Do check for current existence
            //  - look for Id changes.
            var clientSubscription =
                ClientSubscriptions.ReplaceCondition(expression.DataFeedIdentifier.SubscribtionId,
                                                     // only replace if null, or expressions are not equal.
                                                     x => (isNew = (null == x)) || !x.CalcExpression.IsEqual(expression),
                                                     x =>
                                                     {
                                                         // create new Runtime ids if formula was changed
                                                         if (null == x || x.CalcExpression.IsFormulaChanged(expression))
                                                             foreach (var formula in expression.ExpressionFormulas)
                                                                 formula.RuntimeId = Interlocked.Increment(ref _runtimeIdSequenceStart);

                                                         return new ClientSubscription() {CalcExpression = expression};
                                                     });

            if (clientSubscription.IsNotNull())
                updatedExpMsg = clientSubscription.CalcExpression;

            // Expression was either replaced, or added
            if (updatedExpMsg.IsNotNull())
            {
                // Update Runtime storage
                PinkoStorage.SaveExpression(PinkoStorageCode.SubsribeBucketName, HandlerPartitionId, expression.DataFeedIdentifier.SubscribtionId, expression);

                // Broadcast to Calc engines that a change happened. They need to update their ids.
                expression.MsgAction = PinkoMessageAction.ManagerSubscription;
                var outboundEnvelop = new PinkoServiceMessageEnvelop()
                {
                    WebRoleId = updatedExpMsg.DataFeedIdentifier.WebRoleId,
                    Message = updatedExpMsg,
                    ReplyTo = envelop.ReplyTo, // string.Empty,  //PinkoConfiguration.PinkoMessageBusToWebRoleCalcResultTopic,
                    //                               New Calc Engine                                             Subscription update to specific role
                    QueueName = isNew ? PinkoConfiguration.PinkoMessageBusToCalcEngineQueue : PinkoConfiguration.PinkoMessageBusToWorkerCalcEngineTopic
                };

                // publish
                TopicPublisher.Publish(outboundEnvelop);

                // do not reply.  The Calc engine will repond to client.
                return null;
            }

            //
            // Unsubscribe
            //  -   remove from storage
            //  -   Tell the calc engines to unsubscribe

            // Calc Engine monitor
            //  -   Monitor calc engines heart beat
            //  -   If failed, the re-subscribe the client

            // Monitor siblings
            //  -   If sibling missing, the take over its subscriptions
            //  -   Load from storage and re-subscribe the client formulas

            return response;
        }



        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public void CheckClientTimeout(DateTime now)
        {
            ClientSubscriptions
                .GetEnumerator()
                //.ForEach(x => Debug.WriteLine(now.Subtract(x.LastPing).TotalMilliseconds))
                .Where(x => now.Subtract(x.LastPing).TotalMilliseconds > PinkoConfiguration.ClientTimeoutThresholdMs)
                .Select(x => x.CalcExpression)
                .ForEach(expression =>
                    {
                        Trace.TraceInformation("Expression Timed out: {0}", expression.Verbose());

                        // Update Runtime storage
                        PinkoStorage.RemoveExpression(PinkoStorageCode.SubsribeBucketName, HandlerPartitionId, expression.DataFeedIdentifier.SubscribtionId, expression);

                        // Remove from list 
                        ClientSubscriptions.Remove(expression.DataFeedIdentifier.SubscribtionId);

                        // Broadcast to Calc engines to unsubscribe
                        //expression.MsgAction = PinkoMessageAction.ManagerUnsubscribe;
                        var outboundEnvelop = new PinkoServiceMessageEnvelop()
                        {
                            Message = new PinkoMsgClientTimeout
                                {
                                    DataFeedIdentifier = expression.DataFeedIdentifier.DeepClone()
                                },
                            ReplyTo = string.Empty,
                            WebRoleId = expression.DataFeedIdentifier.WebRoleId,
                            QueueName = PinkoConfiguration.PinkoMessageBusToWorkerCalcEngineAllTopic
                        };

                        // publish
                        TopicPublisher.Publish(outboundEnvelop);
                    });

        }



        /// <summary>
        /// Process real time data subscription
        /// </summary>
        public IBusMessageOutbound ProcessConnectionChange(IBusMessageInbound envelop, PinkoMsgClientConnect clientConnect)
        {
            ClientSubscriptions.ReplaceCondition(clientConnect.DataFeedIdentifier.SubscribtionId,
                                                 x => null != x && !x.CalcExpression.DataFeedIdentifier.IsEqual(clientConnect.DataFeedIdentifier),
                                                 x =>
                                                     {
                                                         var itemClone = x.CalcExpression.DeepClone();

                                                         itemClone.DataFeedIdentifier = itemClone.DataFeedIdentifier.PartialClone(clientConnect.DataFeedIdentifier);

                                                         // Replace with new change
                                                         return new ClientSubscription {CalcExpression = itemClone};
                                                     });
            return null;
        }

        /// <summary>
        /// Clients 
        /// </summary>
        public PinkoDictionary<string, ClientSubscription> ClientSubscriptions = new PinkoDictionary<string, ClientSubscription>();

        /// <summary>
        /// Storage partition Id.
        /// </summary>
        public string HandlerPartitionId;


        /// <summary>
        /// Keep unique for this Worker Role
        /// </summary>
        public long RuntimeIdAtomicSequence
        {
            get { return Interlocked.Read(ref _runtimeIdSequenceStart); }
        }

        public readonly long RuntimeIdStart = 10000000;
        private long _runtimeIdSequenceStart = 10000000;
        private IDisposable _timeoutCheck;


        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }


        /// <summary>
        /// IUnityContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }


        /// <summary>
        /// IPinkoStorage
        /// </summary>
        [Dependency]
        public IPinkoStorage PinkoStorage { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }


        /// <summary>
        /// IPinkoMarketEnvManager
        /// </summary>
        [Dependency]
        public IPinkoMarketEnvManager PinkoMarketEnvManager { get; set; }

        /// <summary>
        /// Topic publisher
        /// </summary>
        [Dependency]
        public IRxMemoryBus<IBusMessageOutbound> TopicPublisher { get; set; }
    }

    /// <summary>
    /// ClientSubscription
    /// </summary>
    public class ClientSubscription
    {
        public PinkoMsgCalculateExpression CalcExpression;

        public DateTime LastPing
        {
            get { return _lastPing; }
            set { _lastPing = value; }
        }
        private DateTime _lastPing = DateTime.Now;
    }
}
