using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace PinkDao
{
    /// <summary>
    /// Single point structure
    /// </summary>
    public struct PinkoFormPoint
    {
        public double PointValue;
        public double PointTime;
    }

    /// <summary>
    /// PinkoFormPointExtensions
    /// </summary>
    public static class PinkoFormPointExtensions
    {
        public static readonly PinkoFormPoint[] PinkoFormPointDeault = new PinkoFormPoint[] {};

        /// <summary>
        /// Comparer - used in Linq Sequence
        /// </summary>
        public static readonly IEqualityComparer<PinkoFormPoint> Comparer = new PinkoFormPointComparer();

        /// <summary>
        /// compare pinko point content
        /// </summary>
        public static bool IsEqual(this PinkoFormPoint src, PinkoFormPoint compareTo)
        {
            return Comparer.Equals(src, compareTo) && src.PointTime == compareTo.PointTime;
        }


        /// <summary>
        /// Copy array to new one
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static PinkoFormPoint[] DeepClone(this PinkoFormPoint[] src)
        {
            var points = new PinkoFormPoint[src.Length];

            Array.Copy(src, points, src.Length);

            return points;
        }


        /// <summary>
        /// PinkoFormPoint
        /// </summary>
        public static string Verbose(this PinkoFormPoint obj)
        {
            return string.Format("( {0} / {1} )", obj.PointValue, obj.PointTime);
        }


        /// <summary>
        /// PinkoFormPoint[]
        /// </summary>
        public static string Verbose(this PinkoFormPoint[] obj)
        {
            var sb = new StringBuilder(PinkoDaoStatic.StringBuilderDefaultSize);

            obj.ForEach(x => sb.Append(x.Verbose()));

            return sb.ToString();
        }
    }
    /// <summary>
    /// IEqualityComparer<PinkoFormPoint>
    /// </summary>
    public class PinkoFormPointComparer : IEqualityComparer<PinkoFormPoint>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
        public bool Equals(PinkoFormPoint x, PinkoFormPoint y)
        {
            return x.PointTime == y.PointTime && x.PointValue == y.PointValue;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(PinkoFormPoint obj)
        {
            return obj.GetHashCode();
        }
    }


}
