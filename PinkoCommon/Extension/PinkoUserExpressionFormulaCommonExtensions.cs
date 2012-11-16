using System.Linq;
using PinkDao;
using PinkoCommon.Utility;

namespace PinkoCommon.Extension
{
    /// <summary>
    /// PinkoUserExpressionFormulaExtensions
    /// </summary>
    public static class PinkoUserExpressionFormulaCommonExtensions
    {
        /// <summary>
        /// Convert a string to typed PinkoUserExpressionFormula[]
        /// </summary>
        static public PinkoUserExpressionFormula[] FromUrlParameter(string serialized)
        {
            PinkoUserExpressionFormula[] formulas = null;

            var ex = TryCatch.RunInTry(() =>
                {
                    //  formulaId:Label:formula; formulaId:Label:formula; formulaId:Label:formula; 
                    formulas = serialized
                        .Replace("=", string.Empty)
                        .Split(DelimFormula)
                        .Select(formula => formula.Trim())
                        .Where(formula => !string.IsNullOrEmpty(formula))
                        .Select(formula =>
                            {
                                var parts = formula
                                    .Split(DelimField)
                                    .Select(part => part.Trim())
                                    .ToArray();

                                return new PinkoUserExpressionFormula
                                    {
                                        FormulaId = parts[0],
                                        ExpressionLabel = parts[1],
                                        ExpressionFormula = parts[2]
                                    };
                            })
                        .ToArray();
                });

            // assure at least an empty array is returned
            if (ex != null)
                return new PinkoUserExpressionFormula[] {};

            return formulas;
        }

        public const char DelimField = ':';
        public const char DelimFormula = ';';
    }
}
