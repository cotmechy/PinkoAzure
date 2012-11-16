using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoCommon.Utility;

namespace PinkoTests
{
    [TestClass]
    public class PinkoManualResetEventTests
    {

        /// <summary>
        /// Default timeout
        /// </summary>
        [TestMethod]
        public void TestDefaultSignaled()
        {
            var ev = new PinkoManualResetEvent(true);

            Assert.IsTrue(ev.WaitOne(500));
        }

        /// <summary>
        /// Default timeout
        /// </summary>
        [TestMethod]
        public void TestDefaultTimeout()
        {
            var ev = new PinkoManualResetEvent(false);

            Assert.IsFalse(ev.WaitOne(500));
        }

        /// <summary>
        /// Delayed signal = Timeout
        /// </summary>
        [TestMethod]
        public void TestDelayedSignaled()
        {
            var ev = new PinkoManualResetEvent(false) {DefaultMilliseconds = 2000};

            long signaled = -1;
            Task.Factory.StartNew(() => signaled = ev.WaitOne());
            Thread.Sleep(2200);

            ev.Set();
            Thread.Sleep(1000);

            // assure it waited and eventually signaled
            Assert.IsTrue(signaled == 1);
        }


        /// <summary>
        /// Delayed signal = Timeout
        /// </summary>
        [TestMethod]
        public void TestDelayedNonSignaled()
        {
            var ev = new PinkoManualResetEvent(false) { DefaultMilliseconds = 200000 };

            long signaled = -1;
            Task.Factory.StartNew(() => signaled = ev.WaitOne());
            Thread.Sleep(1000);

            // assure it waited and eventually signaled
            Assert.IsTrue(signaled == -1);

            ev.Set();
            Thread.Sleep(1000);
        }


        /// <summary>
        /// Wait timeout - non signaled
        /// </summary>
        [TestMethod]
        public void TestDelayedTimedNoSignaled()
        {
            var ev = new PinkoManualResetEvent(false);

            var signaled = true;
            Task.Factory.StartNew(() => signaled = ev.WaitOne(500));
            Thread.Sleep(2000);

            // assure it timeout since it did not get signaled
            Assert.IsFalse(signaled);
        }


        /// <summary>
        /// Wait timeout - Signaled
        /// </summary>
        [TestMethod]
        public void TestDelayedTimedSignaled()
        {
            var ev = new PinkoManualResetEvent(false);

            var signaled = true;
            Task.Factory.StartNew(() => signaled = ev.WaitOne(30000));
            Thread.Sleep(1000);

            ev.Set();
            Thread.Sleep(1000);

            // assure it waited and eventually signaled
            Assert.IsTrue(signaled);
        }

    }
}
