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
        /// <summary>
        /// Default array
        /// </summary>
        public static readonly ResultsTuppleWrapper[] ResultsTuppleWrapperDefault = new ResultsTuppleWrapper[] { };

        /// <summary>
        /// comparer
        /// </summary>
        public static readonly IEqualityComparer<ResultsTuppleWrapper> Comparer = new ResultsTuppleWrapperComparer();

        ///// <summary>
        ///// Getdeltas from both sequences
        ///// </summary>
        //public static bool GetDeltas(this ResultsTuppleWrapper[] src, ResultsTuppleWrapper[] compareTo)
        //{

        //    //return Comparer.Equals(src, compareTo);
        //}


        /// <summary>
        /// compare ResultsTuppleWrapper
        /// </summary>
        public static bool IsEqual(this ResultsTuppleWrapper src, ResultsTuppleWrapper compareTo)
        {
            return Comparer.Equals(src, compareTo);
        }


        /// <summary>
        /// Copy array to new one
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static ResultsTuppleWrapper[] DeepClone(this ResultsTuppleWrapper[] src)
        {
            return src == null ? new ResultsTuppleWrapper[] { } : src.Select(x => x.CopyTo(new ResultsTuppleWrapper())).ToArray();
        }



        ///// <summary>
        ///// Is array equal values
        ///// </summary>
        //public static bool IsEqual(this ResultsTuppleWrapper[] src, ResultsTuppleWrapper[] dest)
        //{
        //    if (null == src && null == dest)
        //        return true;

        //    if (null == src || null == dest)
        //        return false;

        //    return src.SequenceEqual(dest, Comparer);
        //}


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


    /// <summary>
    /// ResultsTuppleWrapper
    /// </summary>
    public class ResultsTuppleWrapperComparer : IEqualityComparer<ResultsTuppleWrapper>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
        public bool Equals(ResultsTuppleWrapper x, ResultsTuppleWrapper y)
        {
            return x.PointSeries.SequenceEqual(y.PointSeries);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(ResultsTuppleWrapper obj)
        {
            return obj.GetHashCode();
        }
    }

    
}