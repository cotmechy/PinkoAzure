using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PinkoCommon.Extensions
{
    static public class PinkoExtensions
    {
        /// <summary>
        /// Object Identity
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string VerboseIdentity(this object obj)
        {
            return string.Format("ObjectIdentity: (TID: {2}: {0}) {1}"
                                            , obj.GetHashCode()
                                            , obj.GetType()
                                            , Thread.CurrentThread.ManagedThreadId
                                            );
        }

        /// <summary>
        /// Is null check for all objects
        /// </summary>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }
    }
}
