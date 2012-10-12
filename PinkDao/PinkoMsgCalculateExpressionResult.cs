using System;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace PinkDao
{
    /// <summary>
    /// Outgoing Calculation Result message
    /// </summary>
    public class PinkoMsgCalculateExpressionResult 
    {
        public PinkoDataFeedIdentifier DataFeedIdentifier = new PinkoDataFeedIdentifier();
        public PinkoUserExpressionFormula[] ExpressionFormulas = new PinkoUserExpressionFormula[] { };
        public int ResultType;
        public ResultsTuppleWrapper[] ResultsTupple = PinkoMsgCalculateExpressionResultExtensions.DefaultResultTupple;
        public long TimeSequence;
    }

    /// <summary>
    /// PinkoMsgCalculateExpressionResult Extensions
    /// </summary>
    public static class PinkoMsgCalculateExpressionResultExtensions
    {
        /// <summary>
        /// DeepClone
        /// </summary>
        public static PinkoMsgCalculateExpressionResult DeepClone(this PinkoMsgCalculateExpressionResult src)
        {
            return src.CopyTo(new PinkoMsgCalculateExpressionResult());
        }


        /// <summary>
        /// Copy all value
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns>Destination passed in parameter</returns>
        public static PinkoMsgCalculateExpressionResult CopyTo(this PinkoMsgCalculateExpressionResult src, PinkoMsgCalculateExpressionResult dest)
        {
            dest.ExpressionFormulas = src.ExpressionFormulas.DeepClone();
            dest.DataFeedIdentifier = src.DataFeedIdentifier.DeepClone();
            dest.ResultType = src.ResultType;
            dest.ResultsTupple = src.ResultsTupple.DeepClone();

            return dest;
        }


        /// <summary>
        /// Default result 
        /// </summary>
        public static readonly ResultsTuppleWrapper[] DefaultResultTupple
            = new[]
                {
                    new ResultsTuppleWrapper()
                        {
                            OriginalFormula = new PinkoUserExpressionFormula() {ExpressionFormula = string.Empty, ExpressionLabel = string.Empty, FormulaId = string.Empty, RuntimeId = 0 },
                            PointSeries = new[] {new PinkoFormPoint() {PointTime = double.NaN}}
                        }
                };


        /// <summary>
        /// PinkoMsgCalculateExpressionResult 
        /// </summary>
        public static string Verbose(this PinkoMsgCalculateExpressionResult  obj)
        {
            return string.Format("PinkoMsgCalculateExpressionResult: ResultType: {1} - {0} - {2}", 
                                        obj.DataFeedIdentifier.Verbose(), 
                                        obj.ResultType,
                                        obj.ResultsTupple.Verbose()
                                        );
        }
    }
}