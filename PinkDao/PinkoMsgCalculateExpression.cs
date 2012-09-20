using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Calculate expression
    /// </summary>
    public class PinkoMsgCalculateExpression
    {
        //public PinkoDataFeedIdentifier Identifier = new PinkoDataFeedIdentifier();
        public string ExpressionFormula = string.Empty;
        public int ResultType;    // 0 = double, 1 = double[]

        public PinkoDataFeedIdentifier DataFeedIdentifier = new PinkoDataFeedIdentifier();

        //public string MaketEnvId = string.Empty;
        
        ///// <summary>
        ///// Client context id. client to use for routing, etc.
        ///// </summary>
        //public string ClientCtx = string.Empty;

        ///// <summary>
        ///// ClientId
        ///// </summary>
        //public string ClientId = string.Empty;

        ///// <summary>
        ///// SignalRId
        ///// </summary>
        //public string SignalRId = string.Empty;

        ///// <summary>
        ///// WebRoleId
        ///// </summary>
        //public string WebRoleId = string.Empty;
    }


    /// <summary>
    /// Calculation Result
    /// </summary>
    public class PinkoMsgCalculateExpressionResult
    {
        //public PinkoDataFeedIdentifier Identifier = new PinkoDataFeedIdentifier();
        public string ExpressionFormula = string.Empty;
        public object ResultValue;
        public int ResultType;    // 0 = double, 1 = double[]
        public PinkoDataFeedIdentifier DataFeedIdentifier = new PinkoDataFeedIdentifier();

        //public string MaketEnvId = string.Empty;

        ///// <summary>
        ///// Client context id. client to use for routing, etc.
        ///// </summary>
        //public string ClientCtx = string.Empty;

        ///// <summary>
        ///// ClientId
        ///// </summary>
        //public string ClientId = string.Empty;

        ///// <summary>
        ///// SignalRId
        ///// </summary>
        //public string SignalRId = string.Empty;

        ///// <summary>
        ///// WebRoleId
        ///// </summary>
        //public string WebRoleId = string.Empty;
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
        public static string Verbose(this PinkoMsgCalculateExpression obj)
        {
            return string.Format("PinkoCalculateExpression: Formula: {0} - {1}", //"MaketEnvId: {1} - ClientCtx: {2} - ClientId: {3} - SignalRId: {4} - WebRoleId: {5}",
                                 obj.ExpressionFormula,
                                 obj.DataFeedIdentifier.Verbose()
                                 //obj.MaketEnvId,
                                 //obj.ClientCtx,
                                 //obj.ClientId,
                                 //obj.SignalRId,
                                 //obj.WebRoleId
                                 );
        }

        /// <summary>
        /// PinkoCalculateExpressionResult
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoMsgCalculateExpressionResult obj)
        {
            return string.Format("PinkoCalculateExpressionResult: Result: {0} - Formula: {1} - {2}", //"MaketEnvId: {2} - ClientCtx: {3} - ClientId: {4} - SignalRId: {5} - WebRoleId: {6}",
                                 obj.ResultType == 0 ? "double" : "double[]",
                                 obj.ExpressionFormula,
                                 obj.DataFeedIdentifier
                                 //obj.MaketEnvId,
                                 //obj.ClientCtx,
                                 //obj.ClientId,
                                 //obj.SignalRId,
                                 //obj.WebRoleId
                                 );
        }

        /// <summary>
        /// PinkoCalculateExpressionResult from PinkoCalculateExpressionResult
        /// </summary>
        public static PinkoMsgCalculateExpressionResult FromRequest(this PinkoMsgCalculateExpressionResult obj, PinkoMsgCalculateExpression src)
        {
            //obj.SignalRId = src.SignalRId;
            //obj.WebRoleId = src.WebRoleId;
            //obj.ClientId = src.ClientId;
            //obj.ClientCtx = src.ClientCtx;
            //obj.MaketEnvId = src.MaketEnvId;
            obj.DataFeedIdentifier = src.DataFeedIdentifier;
            obj.ExpressionFormula = src.ExpressionFormula;
            obj.ResultType = src.ResultType;

            return obj;
        }

        public const int ResultDouble = 0;
        public const int ResultDoubleSeries = 1;
    }



}
