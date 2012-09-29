using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <summary>
        /// PinkoFormPoint
        /// </summary>
        public static string Verbose(this PinkoFormPoint obj)
        {
            return string.Format("PinkoFormPoint: PointValue: {0} - PointTime: {1}", obj.PointValue, obj.PointTime);
        }
    }
}
