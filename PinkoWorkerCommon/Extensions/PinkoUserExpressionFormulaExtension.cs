using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinkDao;

namespace PinkoWorkerCommon.Extensions
{
    static public class PinkoUserExpressionFormulaExtension
    {
        /// <summary>
        /// Get Result tuple - double[] - in ResultsWrapper to serialize into the message bus
        /// </summary>
        static public ResultsTuppleWrapper[] GetResultsTuple(this PinkoUserExpressionFormula[] expressions, object results)
        {
            if (results is double[])
                return expressions.GetTupleResult(results as double[]);

            if (results is double[][])
                return expressions.GetTupleResult(results as double[][]);

            return ResultsTuppleWrapperExtensions.ResultsTuppleWrapperDeault;
        }

        /// <summary>
        /// Get Result tuple - double[] - in ResultsWrapper to serialize into the message bus
        /// </summary>
        static public ResultsTuppleWrapper[] GetTupleResult(this PinkoUserExpressionFormula[] expressions, double[] results)
        {
            var tuple = new ResultsTuppleWrapper[results.Count()];

            for (var idx = 0; idx < results.Count(); idx++)
            {
                var pinkoResult = new[]
                    {
                        new PinkoFormPoint()
                            {
                                PointValue = results[idx],
                                PointTime = DateTime.Now.ToOADate()
                            }
                    };

                tuple[idx] = new ResultsTuppleWrapper() { OriginalFormula = expressions[idx], PointSeries = pinkoResult };
            }

            return tuple;
        }

        /// <summary>
        /// Get Result - double[][] - in ResultsWrapper to serialize into the message bus
        /// </summary>
        static public ResultsTuppleWrapper[] GetTupleResult(this PinkoUserExpressionFormula[] expressions, double[][] results)
        {
            var tuple = new ResultsTuppleWrapper[expressions.Count()];

            for (var idx = 0; idx < results.Count(); idx++)
            {
                var pinkoResults = results[idx].Select(x =>
                        new PinkoFormPoint()
                        {
                            PointValue = x,
                            PointTime = DateTime.Now.ToOADate()
                        }
                    ).ToArray();

                tuple[idx] = new ResultsTuppleWrapper() { OriginalFormula = expressions[idx], PointSeries = pinkoResults };
            }

            return tuple;
        }
    }
}
