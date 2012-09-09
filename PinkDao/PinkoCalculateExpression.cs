using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Calculate expression
    /// </summary>
    public class PinkoCalculateExpression
    {
        //public PinkoDataFeedIdentifier Identifier = new PinkoDataFeedIdentifier();
        public string ExpressionFormula = string.Empty;
        //public object ResultValue;
        public int ResultType;    // 0 = double, 1 = double[]
        public string MaketEnvId = string.Empty;
        
        /// <summary>
        /// Client context id. client to use for routing, etc.
        /// </summary>
        public string ClientCtx = string.Empty;
    }


    /// <summary>
    /// Calculation Result
    /// </summary>
    public class PinkoCalculateExpressionResult
    {
        //public PinkoDataFeedIdentifier Identifier = new PinkoDataFeedIdentifier();
        public string ExpressionFormula = string.Empty;
        public object ResultValue;
        public int ResultType;    // 0 = double, 1 = double[]
        public string MaketEnvId = string.Empty;

        /// <summary>
        /// Client context id. client to use for routing, etc.
        /// </summary>
        public string ClientCtx = string.Empty;
    }


    /// <summary>
    /// PinkoCalculateExpressionDaoExtensions
    /// </summary>
    public static class PinkoCalculateExpressionDaoExtensions
    {
        /// <summary>
        /// PinkoCalculateExpressionDao
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoCalculateExpression obj)
        {
            return string.Format("PinkoCalculateExpression: Formula: {0} - MaketEnvId: {1} - ClientCtx: {2}",
                                 obj.ExpressionFormula,
                                 obj.MaketEnvId,
                                 obj.ClientCtx
                                 );
        }

        /// <summary>
        /// PinkoCalculateExpressionResult
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoCalculateExpressionResult obj)
        {
            return string.Format("PinkoCalculateExpressionResult: Result: {0} - Formula: {1} - MaketEnvId: {2} - ClientCtx: {3}",
                                 obj.ResultType == 0 ? "double" : "double[]",
                                 obj.ExpressionFormula,
                                 obj.MaketEnvId,
                                 obj.ClientCtx
                                 );
        }

        /// <summary>
        /// PinkoCalculateExpressionResult from PinkoCalculateExpressionResult
        /// </summary>
        public static PinkoCalculateExpressionResult FromRequest(this PinkoCalculateExpressionResult obj, PinkoCalculateExpression src)
        {
            obj.ClientCtx = src.ClientCtx;
            obj.MaketEnvId = src.MaketEnvId;
            obj.ExpressionFormula = src.ExpressionFormula;
            obj.ResultType = src.ResultType;

            return obj;
        }

        public const int ResultDouble = 0;
        public const int ResultDoubleSeries = 1;
    }



}
