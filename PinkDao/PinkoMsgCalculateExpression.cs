using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace PinkDao
{
    /// <summary>
    /// UserFormaula - single user formula
    /// </summary>
    public class PinkoUserExpressionFormula
    {
        /// <summary>
        /// Id created at runtime as an int to improve performance. The RuntimeId is sent to the client 
        /// with each update.  It is only valid for each runtime session.  Not designed to be stored.
        /// </summary>
        public int RuntimeId;

        /// <summary>
        /// UUID for formula
        /// </summary>
        public string FormulaId;

        /// <summary>
        /// User assigned label. used in expression.
        /// </summary>
        public string ExpressionLabel;

        /// <summary>
        /// Formula expression.
        /// </summary>
        public string ExpressionFormula;
    }


    /// <summary>
    /// Calculation Expression 
    /// </summary>
    public class PinkoMsgCalculateExpression
    {
        public PinkoDataFeedIdentifier DataFeedIdentifier = new PinkoDataFeedIdentifier();
        public PinkoUserExpressionFormula[] ExpressionFormulas = null;
        public string ExpressionFormulasStr = string.Empty;

        public int ResultType;
        public PinkoMessageAction MsgAction = PinkoMessageAction.Snapshot;

    }


    /// <summary>
    /// PinkoCalculateExpressionDaoExtensions
    /// </summary>
    public static class PinkoCalculateExpressionDaoExtensions
    {
        /// <summary>
        /// Copy all value
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns>Destination passed in parameter</returns>
        public static PinkoMsgCalculateExpression CopyTo(this PinkoMsgCalculateExpression src, PinkoMsgCalculateExpression dest)
        {
            dest.ExpressionFormulas = src.ExpressionFormulas;
            dest.ExpressionFormulasStr = src.ExpressionFormulasStr;
            dest.MsgAction = src.MsgAction;
            dest.ResultType = src.ResultType;
            src.DataFeedIdentifier.CopyTo(dest.DataFeedIdentifier);

            return dest;
        }



        /// <summary>
        /// Checks values are the same by comparing internal values.
        /// </summary>
        /// <param name="src"></param>
        /// <returns>
        /// True: they are equal
        /// False: they not are equal
        /// </returns>
        public static bool IsEqual(this PinkoMsgCalculateExpression dest, PinkoMsgCalculateExpression src)
        {
            return
                   dest.ExpressionFormulasStr.Equals(src.ExpressionFormulasStr)
                   &&
                   dest.DataFeedIdentifier.IsEqual(src.DataFeedIdentifier)
                ;
        }

        /// <summary>
        /// Default result 
        /// </summary>
        public static ResultsTuppleWrapper[] DefaultResultTupple
            = new[]
                {
                    new ResultsTuppleWrapper()
                        {
                            OriginalFormula = new PinkoUserExpressionFormula() {ExpressionFormula = string.Empty, ExpressionLabel = string.Empty, FormulaId = string.Empty, RuntimeId = 0 },
                            PointSeries = new[] {new PinkoFormPoint() {PointTime = double.NaN}}
                        }
                };


        /// <summary>
        /// Get formula string representation for expression engine
        /// </summary>
        public static string GetExpression(this PinkoMsgCalculateExpression obj)
        {
            return GetExpression(obj.ExpressionFormulas);
        }

        /// <summary>
        /// Get formula string representation for expression engine
        /// </summary>
        private static string GetExpression(IEnumerable<PinkoUserExpressionFormula> expressionFormulas)
        {
            var sb = new StringBuilder(1028);
            var sbLbl = new StringBuilder(1028);
            var comma = string.Empty;

            sb.Append("{ ");

            if (expressionFormulas != null)
                expressionFormulas.ForEach(x =>
                    {
                        sb.AppendFormat("{0} = {1}; ", x.ExpressionLabel, x.ExpressionFormula);
                        sbLbl.AppendFormat("{1} {0}", x.ExpressionLabel, comma);
                        comma = ",";
                    });

            sb.AppendFormat("[ {0} ] {1}", sbLbl, " }");

            return sb.ToString();
        }

        /// <summary>
        /// PinkoCalculateExpressionDao
        /// </summary>
        public static string Verbose(this PinkoMsgCalculateExpression obj)
        {
            return string.Format("PinkoCalculateExpression: Formula: {0} - {1} - Expression: {2}", 
                                 obj.GetExpression(),
                                 obj.DataFeedIdentifier.Verbose(),
                                 obj.ExpressionFormulasStr
                                 );
        }

        /// <summary>
        /// PinkoCalculateExpressionResult
        /// </summary>
        public static string Verbose(this PinkoMsgCalculateExpressionResult obj)
        {
            return string.Format("PinkoCalculateExpressionResult: Result: {0} - {1}",
                                 string.Join(" = ", obj.ResultsTupple.Select(x => string.Format("{0} = {1}", x.OriginalFormula.ExpressionLabel, x.PointSeries.Select(p => p.Verbose())))),
                                 obj.DataFeedIdentifier
                                 );
        }

        /// <summary>
        /// PinkoCalculateExpressionResult from PinkoCalculateExpressionResult
        /// </summary>
        public static PinkoMsgCalculateExpressionResult FromRequest(this PinkoMsgCalculateExpressionResult obj, PinkoMsgCalculateExpression src)
        {
            obj.DataFeedIdentifier = src.DataFeedIdentifier.CopyTo(new PinkoDataFeedIdentifier());
            obj.ResultType = src.ResultType;

            return obj;
        }

        /// <summary>
        /// PinkoUserExpressionFormula
        /// </summary>
        public static string Verbose(this PinkoUserExpressionFormula obj)
        {
            return string.Format("PinkoUserExpressionFormula: " +
                                     "ExpressionFormula: {0} - " +
                                     "ExpressionLabel: {1} - " +
                                     "FormulaId: {2} - " +
                                     "RuntimeId: {3}", 
                                                obj.ExpressionFormula, 
                                                obj.ExpressionLabel, 
                                                obj.FormulaId, 
                                                obj.RuntimeId);
        }

        public const int ResultDouble = 0;
        public const int ResultDoubleSeries = 1;
    }
}
