using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao
{
    /// <summary>
    /// Calculate expression
    /// </summary>
    public class PinkoCalculateExpressionDao
    {
        //public PinkoDataFeedIdentifier Identifier = new PinkoDataFeedIdentifier();
        public string ExpressionFormula = string.Empty;
        public object ResultValue;
        public int ResultType;    // 0 = double, 1 = double[]
        public string MaketEnvId = string.Empty;
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
        public static string Verbose(this PinkoCalculateExpressionDao obj)
        {
            return string.Format("PinkoCalculateExpressionDao: Result: {0} - Formula: {1} - MaketEnvId: {2}",
                                 obj.ResultType == 0 ? "double" : "double[]",
                                 obj.ExpressionFormula,
                                 obj.MaketEnvId
                                 );
        }

        public const int ResultDouble = 0;
        public const int ResultDoubleSeries = 1;
    }

}
