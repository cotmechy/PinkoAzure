using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkDao.Extensions
{
    /// <summary>
    /// PinkoFormulaExtensions
    /// </summary>
    public static class PinkoFormulaExtensions
    {

        /// <summary>
        /// Convert type into string expression, ie: RForm("symbol", "IBM", "Price.Bid", "Reuters")
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToExpressionStr(this PinkoFormula obj)
        {
            return string.Format("=RForm(\"{0}\", \"{1}\", \"{2}\", \"{3}\")", obj.IdType, obj.IdName, obj.FieldName, obj.Source);
        }


        /// <summary>
        /// PinkoFormula
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Verbose(this PinkoFormula obj)
        {
            return string.Format("PinkoFormula: ", obj.ToExpressionStr());
        }
    }

}
