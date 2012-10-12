using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace PinkDao
{
    /// <summary>
    /// Wrapper to use as tuple.  Tuple cannot be serialized due to missing default constructor.
    /// </summary>
    public class ResultsTuppleWrapper
    {
        public PinkoUserExpressionFormula OriginalFormula = new PinkoUserExpressionFormula();
        public PinkoFormPoint[] PointSeries = PinkoFormPointExtensions.PinkoFormPointDeault;
    }

    /// <summary>
    /// ResultsTuppleWrapperExtensions
    /// </summary>
    public static class ResultsTuppleWrapperExtensions
    {
        public static readonly ResultsTuppleWrapper[] ResultsTuppleWrapperDeault = new ResultsTuppleWrapper[] { };


        
        /// <summary>
        /// Copy array to new one
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static ResultsTuppleWrapper[] DeepClone(this ResultsTuppleWrapper[] src)
        {
            return src == null ? new ResultsTuppleWrapper[] { } : src.Select(x => x.CopyTo(new ResultsTuppleWrapper())).ToArray();
        }

        /// <summary>
        /// Copy all value
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns>Destination passed in parameter</returns>
        public static ResultsTuppleWrapper CopyTo(this ResultsTuppleWrapper src, ResultsTuppleWrapper dest)
        {
            src.OriginalFormula.CopyTo(dest.OriginalFormula);
            dest.PointSeries = src.PointSeries.DeepClone();

            return dest;
        }


        /// <summary>
        /// ResultsTuppleWrapper[]
        /// </summary>
        public static string Verbose(this ResultsTuppleWrapper[] obj)
        {
            var sb = new StringBuilder(PinkoDaoStatic.StringBuilderDefaultSize);

            obj.ForEach(x => sb.Append(x.Verbose()));

            return sb.ToString();
        }


        /// <summary>
        /// ResultsTuppleWrapper
        /// </summary>
        public static string Verbose(this ResultsTuppleWrapper obj)
        {
            var sb = new StringBuilder(PinkoDaoStatic.StringBuilderDefaultSize);

            sb.AppendFormat("ResultsTuppleWrapper: {0} - Points: {1}", obj.OriginalFormula, obj.PointSeries.Verbose());

            return sb.ToString();
        }

    }

    
}