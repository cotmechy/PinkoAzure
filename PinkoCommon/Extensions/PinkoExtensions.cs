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

    }
}
