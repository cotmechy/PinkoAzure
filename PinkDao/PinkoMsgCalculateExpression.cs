using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace PinkDao
{
    /// <summary>
    /// Calculation Expression 
    /// </summary>
    public class PinkoMsgCalculateExpression
    {
        public PinkoDataFeedIdentifier DataFeedIdentifier = new PinkoDataFeedIdentifier();
        public PinkoUserExpressionFormula[] ExpressionFormulas = PinkoCalculateExpressionDaoExtensions.DefaultnewPinkoUserExpressionFormula;
        public string ExpressionFormulasStr = string.Empty;

        public int ResultType;
        public PinkoMessageAction MsgAction = PinkoMessageAction.UserSnapshot;
    }


    /// <summary>
    /// PinkoCalculateExpressionDaoExtensions
    /// </summary>
    public static class PinkoCalculateExpressionDaoExtensions
    {
        /// <summary>
        /// Default 
        /// </summary>
        public static readonly PinkoUserExpressionFormula[] DefaultnewPinkoUserExpressionFormula = new PinkoUserExpressionFormula[] {};


        /// <summary>
        /// DeepClone
        /// </summary>
        /// <returns>new instance</returns>
        public static PinkoMsgCalculateExpression DeepClone(this PinkoMsgCalculateExpression src)
        {
            return CopyTo(src, new PinkoMsgCalculateExpression());
        }



        /// <summary>
        /// Copy all value
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns>Destination passed in parameter</returns>
        public static PinkoMsgCalculateExpression CopyTo(this PinkoMsgCalculateExpression src, PinkoMsgCalculateExpression dest)
        {
            dest.ExpressionFormulas = src.ExpressionFormulas.DeepClone();
            dest.ExpressionFormulasStr = src.ExpressionFormulasStr;
            dest.MsgAction = src.MsgAction;
            dest.ResultType = src.ResultType;
            src.DataFeedIdentifier.CopyTo(dest.DataFeedIdentifier);

            return dest;
        }

        /// <summary>
        /// Copy all value
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns>Destination passed in parameter</returns>
        public static PinkoMsgCalculateExpressionResult CopyTo(this PinkoMsgCalculateExpression src, PinkoMsgCalculateExpressionResult dest)
        {
            src.DataFeedIdentifier.CopyTo(dest.DataFeedIdentifier);
            dest.ExpressionFormulas = src.ExpressionFormulas.DeepClone();
            dest.ResultType = src.ResultType;

            return dest;
        }

        /// <summary>
        /// Was formula changed ? 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <returns>
        /// True: formula was changed
        /// False: formula was not changed
        /// </returns>
        public static bool IsFormulaChanged(this PinkoMsgCalculateExpression src, PinkoMsgCalculateExpression dest)
        {
            return src == null ||  false == dest.ExpressionFormulasStr.Equals(src.ExpressionFormulasStr);
        }


        /// <summary>
        /// Checks values are the same by comparing internal values.
        /// </summary>
        /// <param name="dest"> </param>
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
        /// Get formula string representation for expression engine
        /// Build:  [{ A = 1 + 1; B = A +2; [A, B] }
        /// </summary>
        public static string GetExpression(this PinkoMsgCalculateExpression obj)
        {
            return GetExpression(obj.ExpressionFormulas);
        }

        /// <summary>
        /// Get formula string representation for expression engine
        /// Build:  [{ A = 1 + 1; B = A +2; [A, B] }
        /// </summary>
        public static string GetExpression(this IEnumerable<PinkoUserExpressionFormula> expressionFormulas)
        {
            var sb = new StringBuilder(PinkoDaoStatic.StringBuilderDefaultSize);
            var sbLbl = new StringBuilder(PinkoDaoStatic.StringBuilderDefaultSize);
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

        ///// <summary>
        ///// PinkoCalculateExpressionResult
        ///// </summary>
        //public static string Verbose(this PinkoMsgCalculateExpressionResult obj)
        //{
        //    return string.Format("PinkoCalculateExpressionResult: Result: {0} - {1}",
        //                         string.Join(" = ", obj.ResultsTupple.Select(x => string.Format("{0} = {1}", x.OriginalFormula.ExpressionLabel, x.PointSeries.Select(p => p.Verbose())))),
        //                         obj.DataFeedIdentifier
        //                         );
        //}

        /// <summary>
        /// PinkoCalculateExpressionResult from PinkoCalculateExpressionResult
        /// </summary>
        public static PinkoMsgCalculateExpressionResult FromRequest(this PinkoMsgCalculateExpressionResult obj, PinkoMsgCalculateExpression src)
        {
            obj.DataFeedIdentifier = src.DataFeedIdentifier.CopyTo(new PinkoDataFeedIdentifier());
            obj.ResultType = src.ResultType;
            obj.ExpressionFormulas = src.ExpressionFormulas.DeepClone();

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
