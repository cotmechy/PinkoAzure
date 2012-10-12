using System;
using System.Collections;
using System.Linq;

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
        public long RuntimeId;

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
    /// PinkoUserExpressionFormulaExtensions
    /// </summary>
    public static class PinkoUserExpressionFormulaExtensions
    {

        public static readonly PinkoUserExpressionFormulaComparer Comparer = new PinkoUserExpressionFormulaComparer();

        ///// <summary>
        ///// Are both items equal
        ///// </summary>
        ///// <param name="src"></param>
        ///// <param name="dest"></param>
        ///// <returns></returns>
        //public static bool IsEqual(this PinkoUserExpressionFormula src, PinkoUserExpressionFormula dest)
        //{
        // return dest.ExpressionFormula.Equals(src.ExpressionFormula) &&
        //           dest.ExpressionLabel.Equals(src.ExpressionLabel) &&
        //           dest.FormulaId.Equals(src.FormulaId) &&
        //           dest.RuntimeId.Equals(src.RuntimeId)
        //        ;
        //}


        /// <summary>
        /// Copy array to new one
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static PinkoUserExpressionFormula[] DeepClone(this PinkoUserExpressionFormula[] src)
        {
            return src == null ? new PinkoUserExpressionFormula[] { } : src.Select(x => x.CopyTo(new PinkoUserExpressionFormula())).ToArray();
        }

        /// <summary>
        /// Copy to values to destination
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns>Destination</returns>
        public static PinkoUserExpressionFormula CopyTo(this PinkoUserExpressionFormula src, PinkoUserExpressionFormula dest)
        {
            dest.ExpressionFormula = src.ExpressionFormula;
            dest.ExpressionLabel = src.ExpressionLabel;
            dest.FormulaId = src.FormulaId;
            dest.RuntimeId = src.RuntimeId;

            return dest;
        }


        /// <summary>
        /// PinkoUserExpressionFormula
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoUserExpressionFormula obj)
        {
            return string.Format("PinkoUserExpressionFormula: ExpressionLabel: {0} - " +
                                                             "ExpressionFormula: {1} - " +
                                                             "RuntimeId: {2} - " +
                                                             "FormulaId: {3}", 
                                                                obj.ExpressionLabel, 
                                                                obj.ExpressionFormula, 
                                                                obj.RuntimeId, 
                                                                obj.FormulaId);
        }
    }

    /// <summary>
    /// Comparer
    /// </summary>
    public class PinkoUserExpressionFormulaComparer : System.Collections.Generic.IEqualityComparer<PinkoUserExpressionFormula>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        public bool Equals(PinkoUserExpressionFormula dest, PinkoUserExpressionFormula src)
        {
            return dest.ExpressionFormula.Equals(src.ExpressionFormula) &&
                      dest.ExpressionLabel.Equals(src.ExpressionLabel) &&
                      dest.FormulaId.Equals(src.FormulaId) &&
                      dest.RuntimeId.Equals(src.RuntimeId)
                   ;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        public int GetHashCode(PinkoUserExpressionFormula obj)
        {
            return obj.GetHashCode();
        }
    }

}