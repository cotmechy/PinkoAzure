using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PinkoCommon.Utility
{
    /// <summary>
    /// Time status reporting
    /// </summary>
    public class TryTimerMessage
    {
        public string Message { get; set; }
        public double TimeElapsedMs { get; set; }
        public double TimeThresholdMs { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    static public class TryTimer
    {
        /// <summary>
        /// Set static threshold
        /// </summary>
        public static double TimerThresholdMs = 2000;


        ///// <summary>
        ///// Provide Tag, time elapse, time threshold amount 
        ///// </summary>
        //static public Action<TryTimerMessage> TimerAction = null;


        /// <summary>
        /// Run in time and always log the time taken to process. Use for profiling.
        /// </summary>
        static public TryTimerMessage RunInTimerLog(Action action)
        {
            return RunInTimer(() => string.Empty, 0, action);
        }

        /// <summary>
        /// Pass constant string as the tag
        /// Log warning message when time take longer than TimerThresholdMs to process.
        /// </summary>
        static public TryTimerMessage RunInTimer(string msgtag, Action action)
        {
            return RunInTimer(() => msgtag, TimerThresholdMs, action);
        }

        /// <summary>
        /// It user a time target Func<> to delay expensive building message until needed.
        /// Log warning message when time take longer than TimerThresholdMs to process.
        /// </summary>
        static public TryTimerMessage RunInTimer(Func<string> msgactionTag, Action action)
        {
            return RunInTimer(msgactionTag, TimerThresholdMs, action);
        }


        /// <summary>
        /// Raw time processing
        /// </summary>
        static public TryTimerMessage RunInTimer(Func<string> msgactionTag, double msThreshold, Action action)
        {
            var dtStart = DateTime.UtcNow;
            TryTimerMessage timeStat = null;
            try
            {
                action();
            }
            //catch (Exception ex)
            //{
            //    Trace.TraceWarning("ERROR: {0}", ex.ToString());
            //}
            finally
            {
                double timeElapse;
                if ((timeElapse = (DateTime.UtcNow - dtStart).TotalMilliseconds) >= msThreshold)
                {
                    timeStat = new TryTimerMessage()
                    {
                        TimeElapsedMs = timeElapse,
                        TimeThresholdMs = TimerThresholdMs,
                        Message = string.Format("TimerTag: {0}: Start: {1} - End: {2} - Ms: {3:0.000}",
                                msgactionTag(),
                                dtStart.ToShortTimeString(),
                                DateTime.UtcNow.ToShortTimeString(),
                                timeElapse)
                    };

                    Trace.TraceWarning(timeStat.Verbose());
                }
            }

            return timeStat;
        }




        ///// <summary>
        ///// 
        ///// </summary>
        //static public TryTimerMessage RunInTimer(string msgtag, double msThreshold, Action action)
        //{
        //    var dtStart = DateTime.UtcNow;
        //    TryTimerMessage timeStat = null;
        //    try
        //    {
        //        action();
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceWarning("ERROR: {0}", ex.ToString());
        //    }
        //    finally
        //    {
        //        double timeElapse;
        //        if ((timeElapse = (DateTime.UtcNow - dtStart).TotalMilliseconds) >= msThreshold)
        //        {
        //            timeStat = new TryTimerMessage()
        //            {
        //                TimeElapsedMs = timeElapse,
        //                TimeThresholdMs = TimerThresholdMs,
        //                Message = string.Format("TimerTag: {0}: Start: {1} - End: {2} - Ms: {3:0.000}",
        //                        msgtag,
        //                        dtStart.ToShortTimeString(),
        //                        DateTime.UtcNow.ToShortTimeString(),
        //                        timeElapse)
        //            };

        //            Trace.TraceWarning(timeStat.Verbose());
        //        }
        //    }

        //    return timeStat;
        //}
    }

    /// <summary>
    /// TryTimerMessageExtensions
    /// </summary>
    public static class TryTimerMessageExtensions
    {
        /// <summary>
        /// TryTimerMessage
        /// </summary>
        public static string Verbose(this TryTimerMessage obj)
        {
            return string.Format("TryTimerMessage: Message: {0} - TimeElapsedMs: {1} - TimeThresholdMs: {2}", obj.Message, obj.TimeElapsedMs, obj.TimeThresholdMs);
        }
    }

}
