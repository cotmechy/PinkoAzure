using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using PinkDao;
using PinkoCommon.Extension;
using PinkoExpressionCommon;

namespace PinkoWorkerCommon.Utility
{
    /// <summary>
    /// Container of expression subscribers 
    /// NOTE: Not thread safe
    /// </summary>
    public class PinkoExpressionSubscribers<T>
    {
        /// <summary>
        /// Subscribers - SubsId -> ClientId -> expression
        /// NOTE: Not thread safe
        /// </summary>
        public Dictionary<string, PinkoSubscription<T>> Subscribers = new Dictionary<string, PinkoSubscription<T>>(10000);

        /// <summary>
        /// Last Results
        /// </summary>
        public T LastResults;

        /// <summary>
        /// Add/Insert
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="compiledExpression"> </param>
        /// <returns>
        /// True: Updated
        /// False: Inserted
        /// </returns>
        public bool UpdateSubscriber(PinkoMsgCalculateExpressionResult subscriber, Func<IPinkoMarketEnvironment, T> compiledExpression)
        {
            PinkoSubscription<T> idSubscribtion = null;

            // Add pinko subscription
            if (!Subscribers.TryGetValue(subscriber.DataFeedIdentifier.SubscribtionId, out idSubscribtion))
                Subscribers[subscriber.DataFeedIdentifier.SubscribtionId] = idSubscribtion = new PinkoSubscription<T>();

            idSubscribtion.CompiledExpression = compiledExpression;

            // find client 
            var subs =
                idSubscribtion
                    .Subcribers
                    .FirstOrDefault(x => x.Item2.DataFeedIdentifier.ClientCtx.Equals(subscriber.DataFeedIdentifier.ClientCtx));

            if (!subs.IsNull())
            {
                subscriber.CopyTo(subs.Item2);
                return true;
            }

            var pinkoCalcTuple = new PinkoCalcTuple<IPinkoMarketEnvironment, PinkoMsgCalculateExpressionResult>
                {
                    Item1 = null,
                    Item2 = subscriber.DeepClone()
                };

            idSubscribtion.Subcribers.Add(pinkoCalcTuple);
            return false;
        }


        /// <summary>
        /// Remove subscribers
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns>Removed PinkoMsgCalculateExpression</returns>
        public PinkoMsgCalculateExpressionResult RemoveSubscriber(PinkoMsgCalculateExpressionResult subscriber)
        {
            PinkoSubscription<T> idSubscribtion = null;
            PinkoCalcTuple<IPinkoMarketEnvironment, PinkoMsgCalculateExpressionResult> expression = null;

            if (Subscribers.TryGetValue(subscriber.DataFeedIdentifier.SubscribtionId, out idSubscribtion))
            {
                expression = idSubscribtion
                                .Subcribers
                                .FirstOrDefault(x => x.Item2.DataFeedIdentifier.ClientCtx.Equals(subscriber.DataFeedIdentifier.ClientCtx));

                if (!expression.IsNull())
                    idSubscribtion.Subcribers.Remove(expression);

                if (idSubscribtion.Subcribers.Count == 0)
                    Subscribers.Remove(subscriber.DataFeedIdentifier.SubscribtionId);
            }

            return expression.IsNull() ? null :  expression.Item2;
        }


        /// <summary>
        /// IPinkoExpressionEngine
        /// </summary>
        [Dependency]
        public IPinkoExpressionEngine PinkoExpressionEngine { get; set; }
    }

}
