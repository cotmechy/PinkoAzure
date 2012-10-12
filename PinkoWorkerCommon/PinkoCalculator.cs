using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon;
using PinkoCommon.Extension;
using PinkoCommon.Extensions;
using PinkoCommon.Interface;
using PinkoCommon.Utility;
using PinkoExpressionCommon;
using PinkoWebRoleCommon.Extensions;
using PinkoWorkerCommon.Extensions;
using PinkoWorkerCommon.Utility;

namespace PinkoWorkerCommon
{
    /// <summary>
    /// Calculate expression and process results
    /// </summary>
    public class PinkoCalculator<T> 
    {
        /// <summary>
        /// Prepare and calculate dictionary
        /// </summary>
        public PinkoExpressionSubscribers<T> Caculate(PinkoExpressionSubscribers<T> subscribers)
        {
            //
            // Start calculating
            //
            List<PinkoCalcTrio<T>> calcs = null;

                // Get list of calcs
                TryTimer.RunInTimer( () => string.Format("Setting up {0} subscribers", subscribers.Subscribers.Keys.Count()),
                                        () => calcs = SetupSubscribers(subscribers));

                // Calculate prepared list
                TryTimer.RunInTimer(() => string.Format("Calculating {0} subscribers", calcs.Count),
                                    () => CalculatePreparedList(calcs));
            return subscribers;
        }


        /// <summary>
        /// Run parallel calculations on all subscribers
        /// </summary>
        /// 
        public void CalculatePreparedList(List<PinkoCalcTrio<T>> calcs)
        {
            PinkoApplication.ForEachParallel(calcs, 
                    pinkoCalcTrio =>
                    {
                        // Prepare outbound message
                        var pinkoMsgCalculateExpressionResult = pinkoCalcTrio.PinkoMsgCalculateExpressionResult.DeepClone();

                        var msgEnvelop = PinkoApplication.FactorWebEnvelop(pinkoCalcTrio.PinkoMsgCalculateExpressionResult.DataFeedIdentifier.ClientId,
                                                                            pinkoCalcTrio.PinkoMsgCalculateExpressionResult.DataFeedIdentifier.WebRoleId,
                                                                            pinkoMsgCalculateExpressionResult);

                        // Run calc
                        var ex = 
                        TryCatch.RunInTry( () => 
                            TryTimer.RunInTimer( 
                                                 () => string.Format("SubscritpionId: {0}", pinkoCalcTrio.PinkoMsgCalculateExpressionResult.Verbose()), 
                                                 () => pinkoCalcTrio.PinkoSubscription.LastResults = PinkoExpressionEngine.Invoke(pinkoCalcTrio.PinkoMarketEnvironment, pinkoCalcTrio.PinkoSubscription.CompiledExpression)
                                                )
                        );

                        // Exception ? 
                        if (ex.IsNull())
                        {
                            // save results
                            var resultsTupple = pinkoCalcTrio.PinkoMsgCalculateExpressionResult.ExpressionFormulas.GetResultsTuple(pinkoCalcTrio.PinkoSubscription.LastResults);

                            //// TODO: If any changes, send deltas to clients
                            //pinkoCalcTrio
                            //    .PinkoMsgCalculateExpressionResult
                            //    .ResultsTupple
                            //    .Where(x => x.PointSeries.SequenceEqual())


                            // Set latest results
                            pinkoMsgCalculateExpressionResult.ResultsTupple =
                                pinkoCalcTrio.PinkoMsgCalculateExpressionResult.ResultsTupple =
                                resultsTupple;

                            //var pinkoMsgCalculateExpressionResult = pinkoCalcTrio.PinkoMsgCalculateExpressionResult.DeepClone();
                            //pinkoMsgCalculateExpressionResult.TimeSequence = PinkoApplication.GetTimeSequence();
                        }
                        else
                        {
                            // 
                            // Unexpected exception
                            //
                            msgEnvelop.ErrorCode = PinkoErrorCode.UnexpectedException;
                            msgEnvelop.ErrorDescription = ex.Message;
                            msgEnvelop.ErrorSystem = ex.ToString();
                            pinkoMsgCalculateExpressionResult.ResultType = PinkoErrorCode.UnexpectedException;
                        }

                        // Broadcast to all Worker roles
                        msgEnvelop.QueueName = PinkoConfiguration.PinkoMessageBusToWebRoleCalcResultTopic;
                        pinkoMsgCalculateExpressionResult.TimeSequence = PinkoApplication.GetTimeSequence();

                        ServerMessageOutboundBus.Publish(msgEnvelop);
                    });
        }

        /// <summary>
        /// Set up subscribers to start Async processing
        /// </summary>
        public List<PinkoCalcTrio<T>> SetupSubscribers(PinkoExpressionSubscribers<T> subscribers)
        {
            var pinkoSubscriptions = new List<PinkoCalcTrio<T>>();

            // collect all subscribers. Flatten structure for parallel run.
            subscribers
                .Subscribers
                .ForEach(results => pinkoSubscriptions.AddRange(
                    results.Value.Subcribers.Select(x =>
                                                    new PinkoCalcTrio<T>()
                                                        {
                                                            PinkoMsgCalculateExpressionResult = x.Item2,
                                                            PinkoSubscription = results.Value
                                                        }
                        )));

            // Set market environments
            pinkoSubscriptions
                .AsParallel()
                .ForAll(x => x.PinkoMarketEnvironment = PinkoMarketEnvManager.GetMarketEnv(x.PinkoMsgCalculateExpressionResult.DataFeedIdentifier.MaketEnvId).CloneEnv());

            return pinkoSubscriptions;
        }


        /// <summary>
        /// IPinkoMarketEnvManager
        /// </summary>
        [Dependency]
        public IPinkoMarketEnvManager PinkoMarketEnvManager { get; set; }


        /// <summary>
        /// IPinkoExpressionEngine
        /// </summary>
        [Dependency]
        public IPinkoExpressionEngine PinkoExpressionEngine { get; set; }

        /// <summary>
        /// Outbound message bus to worker roles
        /// </summary>
        [Dependency]
        public IRxMemoryBus<IBusMessageOutbound> ServerMessageOutboundBus { get; set; }


        /// <summary>
        /// PinkoApplication
        /// </summary>
        [Dependency]
        public IPinkoApplication PinkoApplication { get; set; }

        /// <summary>
        /// IPinkoConfiguration
        /// </summary>
        [Dependency]
        public IPinkoConfiguration PinkoConfiguration { private get; set; }

    }
}
