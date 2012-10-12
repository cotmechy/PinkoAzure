using System;
using System.Collections.Generic;
using PinkoExpressionCommon;

namespace PinkDao
{
    /// <summary>
    /// PinkoSubscription
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PinkoSubscription<T>
    {
        /// <summary>
        /// Cache compiled expression
        /// </summary>
        public Func<IPinkoMarketEnvironment, T> CompiledExpression;

        ///// <summary>
        ///// Latest Market Environment
        ///// </summary>
        //public IPinkoMarketEnvironment MarketEnvironment = null;

        /// <summary>
        /// Last Results
        /// </summary>
        public T LastResults;

        /// <summary>
        /// Expression
        /// </summary>
        public List<PinkoCalcTuple<IPinkoMarketEnvironment, PinkoMsgCalculateExpressionResult>> Subcribers = new List<PinkoCalcTuple<IPinkoMarketEnvironment, PinkoMsgCalculateExpressionResult>>(10000);
    }

    ///// <summary>
    ///// Tuple sibling
    ///// </summary>
    public class PinkoCalcTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
    }

    public class PinkoCalcTrio<T>
    {
        public IPinkoMarketEnvironment PinkoMarketEnvironment;
        public PinkoMsgCalculateExpressionResult PinkoMsgCalculateExpressionResult;
        //public Func<IPinkoMarketEnvironment, T> CompileExpression;
        public PinkoSubscription<T> PinkoSubscription;
    }
}
