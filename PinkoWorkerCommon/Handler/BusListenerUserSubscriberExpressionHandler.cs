//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;
//using Microsoft.Practices.Unity;
//using PinkDao;
//using PinkoCommon;
//using PinkoCommon.BaseMessageHandlers;
//using PinkoCommon.Extension;
//using PinkoCommon.Interface;
//using PinkoCommon.Interface.Handlers;
//using PinkoCommon.Interface.Storage;
//using PinkoCommon.Utility;

//namespace PinkoWorkerCommon.Handler
//{
//    /// <summary>
//    /// Handle subscription request.  
//    /// </summary>
//    public class BusListenerUserSubscriberExpressionHandler : InboundMessageHandler<PinkoMsgCalculateExpression>, IBusListenerUserSubscriberExpressionHandler
//    {
//        /// <summary>
//        /// Handle adhoc request
//        /// </summary>
//        public IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpression expression)
//        {
//            var response = (IBusMessageOutbound)msg;
//            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);

//            response.Message = resultMsg;
//            response.WebRoleId = expression.DataFeedIdentifier.WebRoleId;
//            response.ErrorCode = PinkoErrorCode.Success;

//            var handlers = _handlers[expression.MsgAction];

//            var ex = TryCatch.RunInTry(() =>
//                {
//                    if (!handlers.IsNull())
//                        handlers.ToArray().ForEach(handler => handler(resultMsg, msg, expression));
//                });
//                    //var ex = TryCatch.RunInTry(() => _handlers[expression.MsgAction](resultMsg, msg, expression));
            


//            // 
//                // 2. New subscription

//                //
//                // Unsubscribe
//                //  -   remove from storage
//                //  -   Tell the calc engines to unsubscribe

//                // Calc Engine monitor
//                //  -   Monitor calc engines heart beat
//                //  -   If failed, the re-subscribe the client

//                // Monitor siblings
//                //  -   If sibling missing, the take over its subscriptions
//                //  -   Load from storage and re-subscribe the client formulas


//            // Exception ? 
//            if (null != ex)
//            {
//                response.ErrorCode = PinkoErrorCode.UnexpectedException;
//                response.ErrorDescription = ex.Message;
//                response.ErrorSystem = ex.ToString();
//                resultMsg.ResultType = PinkoErrorCode.UnexpectedException;
//            }

//            Trace.TraceInformation("BusListenerSubscribeExpressionHandler: {0}", response.Verbose());
//            Trace.TraceInformation("BusListenerSubscribeExpressionHandler: {0}", resultMsg.Verbose());

//            return response;
//        }

//        /// <summary>
//        /// Add handler
//        /// </summary>
//        public void AddHandler(PinkoMessageAction action, Func<PinkoMsgCalculateExpressionResult, IBusMessageInbound, PinkoMsgCalculateExpression, PinkoMsgCalculateExpressionResult> callback)
//        {
//            // TODO: remove
//            var listeners = _handlers.Get(action, () => new ConcurrentBag<Func<PinkoMsgCalculateExpressionResult, IBusMessageInbound, PinkoMsgCalculateExpression, PinkoMsgCalculateExpressionResult>>());
//            listeners.Add(callback);
//        }



//        ///// <summary>
//        ///// Process real time data subscription
//        ///// </summary>
//        ///// <param name="response"> </param>
//        ///// <param name="expression"></param>
//        ///// <returns>outbound message</returns>
//        //public PinkoMsgCalculateExpressionResult ProcessSubscribe(PinkoMsgCalculateExpressionResult response, IBusMessageInbound envelop, PinkoMsgCalculateExpression expression)
//        //{
//        //    PinkoMsgCalculateExpression updatedExpMsg = null;
//        //    var isNew = false;

//        //    //  - Do check for current existence
//        //    //  - look for Id changes.
//        //    updatedExpMsg =
//        //        ClientSubscriptions.ReplaceCondition(expression.DataFeedIdentifier.SubscribtionId,
//        //                                             // only replace if null, or expressions are not equal.
//        //                                             x => (isNew = (null == x)) || !x.IsEqual(expression),
//        //                                             x =>
//        //                                             {
//        //                                                 // create new Runtime ids if formula was changed
//        //                                                 if (x.IsFormulaChanged(expression))
//        //                                                     // create new RuntimeId
//        //                                                     foreach (var formula in expression.ExpressionFormulas)
//        //                                                         formula.RuntimeId =
//        //                                                             Interlocked.Increment(ref _runtimeIdSequenceStart);

//        //                                                 return expression;
//        //                                             });


//        //    // TODO: Azure only one manager is processing it   


//        //    // Expression was either replaced, or added
//        //    if (null != updatedExpMsg)
//        //    {
//        //        // Update Runtime storage
//        //        PinkoStorage.SaveExpression(PinkoStorageCode.SubsribeBucketName, HandlerPartitionId, expression.DataFeedIdentifier.SubscribtionId, expression);

//        //        // Broadcast to Calc engines that a change happened. They need to update their ids.
//        //        expression.MsgAction = PinkoMessageAction.ManagerSubscription;
//        //        var outboundEnvelop = new PinkoServiceMessageEnvelop()
//        //            {
//        //                Message = updatedExpMsg,
//        //                ReplyTo = PinkoConfiguration.PinkoMessageBusToWebRoleCalcResultTopic,
//        //                //                               New Calc Engine                                             Subscription update to specific role
//        //                QueueName = isNew ? PinkoConfiguration.PinkoMessageBusToCalcEngineQueue  : PinkoConfiguration.PinkoMessageBusToWorkerCalcEngineTopic
//        //            };

//        //        // publish
//        //        TopicPublisher.Publish((IBusMessageOutbound) outboundEnvelop);
//        //    }

//        //    return response;
//        //}



//        /// <summary>
//        /// Allow only subscription based messages
//        /// </summary>
//        protected bool FilterIncomingMsg(System.Tuple<IBusMessageInbound, PinkoMsgCalculateExpression> msg)
//        {
//            return msg.Item2.MsgAction == PinkoMessageAction.UserSubscription || msg.Item2.MsgAction == PinkoMessageAction.Unsubscribe;
//        }

//        /// <summary>
//        /// message handler
//        /// </summary>
//        private readonly PinkoDictionary<PinkoMessageAction, ConcurrentBag<Func<PinkoMsgCalculateExpressionResult, IBusMessageInbound, PinkoMsgCalculateExpression, PinkoMsgCalculateExpressionResult>>> _handlers
//            = new PinkoDictionary<PinkoMessageAction, ConcurrentBag<Func<PinkoMsgCalculateExpressionResult, IBusMessageInbound, PinkoMsgCalculateExpression, PinkoMsgCalculateExpressionResult>>>();
//        //public ConcurrentBag<T> Handlers = new ConcurrentBag<T>();

//        ///// <summary>
//        ///// Storage partition Id.
//        ///// </summary>
//        //public string HandlerPartitionId;

//        ///// <summary>
//        ///// Keep unique for this Worker Role
//        ///// </summary>
//        //public long RuntimeIdAtomicSequence
//        //{
//        //    get { return Interlocked.Read(ref _runtimeIdSequenceStart); }
//        //}

//        //public const long RuntimeIdStart = 10000000;
//        //private long _runtimeIdSequenceStart = RuntimeIdStart;
        


//        ///// <summary>
//        ///// IPinkoConfiguration
//        ///// </summary>
//        //[Dependency]
//        //public IPinkoConfiguration PinkoConfiguration { private get; set; }


//        ///// <summary>
//        ///// IPinkoMarketEnvManager
//        ///// </summary>
//        //[Dependency]
//        //public IPinkoMarketEnvManager PinkoMarketEnvManager { get; set; }

//        ///// <summary>
//        ///// IPinkoStorage
//        ///// </summary>
//        //[Dependency]
//        //public IPinkoStorage PinkoStorage { get; set; }

//        ///// <summary>
//        ///// Topic publisher
//        ///// </summary>
//        //[Dependency]
//        //public IRxMemoryBus<IBusMessageOutbound> TopicPublisher { get; set; }

//        ///// <summary>
//        ///// Clients 
//        ///// </summary>
//        //public PinkoDictionary<string, PinkoMsgCalculateExpression> ClientSubscriptions = new PinkoDictionary<string, PinkoMsgCalculateExpression>();
//        ////[Dependency]
//        ////public PinkoDictionary<string, PinkoMsgCalculateExpression> ClientSubscriptions { get; set; }

//        ///// <summary>
//        ///// Constructor
//        ///// </summary>
//        //public BusListenerUserSubscriberExpressionHandler()
//        //{
//        //    HandlerPartitionId = "SubsHandler_" + MsgHandlerId;
//        //    _handlers = new Dictionary
//        //        <PinkoMessageAction, Func<PinkoMsgCalculateExpressionResult, IBusMessageInbound, PinkoMsgCalculateExpression, PinkoMsgCalculateExpressionResult>>()
//        //        {
//        //            { PinkoMessageAction.UserSubscription, ProcessSubscribe }
//        //        };
//        //}
//    }
//}
