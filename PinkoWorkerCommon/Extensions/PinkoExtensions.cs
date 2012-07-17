using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PinkoWorkerCommon.Extensions
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
            return string.Format("(TID: {2}: {0}) {1}"
                                            , obj.GetHashCode()
                                            , obj.GetType()
                                            , Thread.CurrentThread.ManagedThreadId
                                            );
        }
    }
}
