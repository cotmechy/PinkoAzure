using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Extension
{
    
    /// <summary>
    /// ObjectExtensions
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Is object null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        /// True: when object is null
        /// </returns>
        public static bool IsNull(this Object obj)
        {
            return null == obj;
        }

        /// <summary>
        /// Remove all spaces
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reduce(this string str)
        {
            return str.Replace(" ", string.Empty);
        }
    }


}
