using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.BaseMessageHandlers;
using PinkoCommon.Interface;
using PinkoCommon.Utility;

namespace PinkoWorkerCommon.Handler
{
    /// <summary>
    /// Handle subscription request.  
    /// </summary>
    public class BusListenerSubscribeExpressionHandler : InboundMessageHandler<PinkoMsgCalculateExpression>
    {
        /// <summary>
        /// Handle adhoc request
        /// </summary>
        public override IBusMessageOutbound ProcessRequest(IBusMessageInbound msg, PinkoMsgCalculateExpression expression)
        {
            var outMsg = (IBusMessageOutbound)msg;
            var resultMsg = new PinkoMsgCalculateExpressionResult().FromRequest(expression);

            outMsg.Message = resultMsg;
            outMsg.WebRoleId = expression.DataFeedIdentifier.WebRoleId;
            outMsg.ErrorCode = PinkoErrorCode.Success;

            var ex = TryCatch.RunInTry(() =>
            {
                PinkoMsgCalculateExpression updatedExpMsg = null; 

                switch (expression.MsgAction)
                {
                    // Subscribe
                    case PinkoMessageAction.Subscription:
                        {
                            //  - Do check for current existence
                            //  - look for Id changes.
                            updatedExpMsg =
                                ClientSubscriptions.ReplaceCondition(expression.DataFeedIdentifier.SubscribtionId,
                                                                     x => (null == x) || (!x.IsEqual(expression)),
                                                                     x =>
                                                                         {
                                                                                 // set unique subs formula runtimeId
                                                                                 foreach (var formula in expression.ExpressionFormulas)
                                                                                     formula.RuntimeId = Interlocked.Increment(ref _formulaIdSequence);

                                                                                return expression;
                                                                         });

                            // 1. Is subscription already in place ? (change)
                            if (null != updatedExpMsg)
                            {
                                // Expression was replaced. Need to do some house keeping.
                                //  - Drop out duplicates
                                //  - Remove subscription from storage

                                ..

                            }

                        }
                        break;

                    // Unsubscribe
                    case PinkoMessageAction.Unsubscribe:
                        {

                        }
                        break;
                }


                //
                // Monitor client connects ? 
                //


                // 


                // 
                // 2. New subscription
                //  - Azure only one manager is processing it   
                //  - create unique int id for subscription
                //  - forward message to calc engine for subscription with resolved ids
                //  - Save subscription in storage as real time 
                //  - 

                //
                // Unsubscribe
                //  -   remove from storage
                //  -   Tell the calc engines to unsubscribe

                // Calc Engine monitor
                //  -   Monitor calc engines heart beat
                //  -   If failed, the re-subscribe the client

                // Monitor siblings
                //  -   If sibling missing, the take over its subscribstions
                //  -   Load from storage and re-subscribe the client formulas



            });

            // Exception ? 
            if (null != ex)
            {
                outMsg.ErrorCode = PinkoErrorCode.UnexpectedException;
                outMsg.ErrorDescription = ex.Message;
                outMsg.ErrorSystem = ex.ToString();
                resultMsg.ResultType = PinkoErrorCode.UnexpectedException;
            }

            Trace.TraceInformation("BusListenerSubscribeExpressionHandler: {0}", outMsg.Verbose());
            Trace.TraceInformation("BusListenerSubscribeExpressionHandler: {0}", resultMsg.Verbose());

            return outMsg;
        }

        /// <summary>
        /// Allow only subscription based messages
        /// </summary>
        protected override bool FilterIncomingMsg(System.Tuple<IBusMessageInbound, PinkoMsgCalculateExpression> msg)
        {
            return msg.Item2.MsgAction == PinkoMessageAction.Subscription;
        }

        /// <summary>
        /// Keep unique for this Worker Role
        /// </summary>
        private int _formulaIdSequence = 1000000;

        /// <summary>
        /// IPinkoMarketEnvManager
        /// </summary>
        [Dependency]
        public IPinkoMarketEnvManager PinkoMarketEnvManager { get; set; }

        /// <summary>
        /// Clients 
        /// </summary>
        public PinkoDictionary<string, PinkoMsgCalculateExpression> ClientSubscriptions = new PinkoDictionary<string, PinkoMsgCalculateExpression>();
    }
}
