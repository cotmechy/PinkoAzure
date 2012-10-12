using System;
using System.Diagnostics;

namespace PinkoCommon.Utility
{
    /// <summary>
    /// Warpp trycath block via anonymous delegate
    /// </summary>
    public class TryCatch
    {
        /// <summary>
        /// Prvide string delayed tag Func<>
        /// </summary>
        /// <param name="msgaction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        static public Exception RunInTry(Func<string> msgaction, Action action)
        {
            var ex = RunInTrySilent(action);

            if (null != ex)
                Trace.TraceError("{0}\r\n{1}", msgaction, ex);

            return ex;
        }

        /// <summary>
        /// 
        /// </summary>
        static public Exception RunInTry(Action action)
        {
            Exception ex = null;
            try
            {
                action();
            }
            catch (Exception e)
            {
                ex = e;
                Trace.TraceError("ERROR: {0}", ex.ToString());
            }

            return ex;
        }

        /// <summary>
        /// 
        /// </summary>
        static public void RunInTryThrow(Action action)
        {
            var ex = RunInTry(action);

            if (ex != null)
                throw ex;
        }

        /// <summary>
        /// 
        /// </summary>
        static public Exception RunInTrySilent(Action action)
        {
            Exception ex = null;
            try
            {
                action();
            }
            catch (Exception e)
            {
                ex = e;
            }

            return ex;
        }
    }
}
