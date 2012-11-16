using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoCommon.Utility;

namespace PinkoTests
{
    [TestClass]
    public class PinkoMutexTests
    {

        /// <summary>
        /// Acquired with no timeout
        /// </summary>
        [TestMethod]
        public void TestacquiredWithnoTimeout()
        {
            var mutex = new PinkoMutex();

            var acquired = true;
            Task.Factory.StartNew(() => acquired = mutex.Lock() == 0);
            Thread.Sleep(1000);

            // assure lock was acquired
            Assert.IsTrue(acquired);
        }


        /// <summary>
        /// Acquired with timeout
        /// </summary>
        [TestMethod]
        public void TestacquiredWithTimeout()
        {
            var mutex = new PinkoMutex();

            var acquired = true;
            Task.Factory.StartNew(() => acquired = mutex.Lock(500));
            Thread.Sleep(2000);

            // assure lock was acquired
            Assert.IsTrue(acquired);
        }


        /// <summary>
        /// Signaled with disposable
        /// </summary>
        [TestMethod]
        public void TestDelayedTimedSignaled()
        {
            var mutex = new PinkoMutex();

            Task.Factory.StartNew(() =>
            {
                using (mutex.LockDisposible())
                {
                }
            })
            ;

            Thread.Sleep(1000);

            using (var dispMustex = mutex.LockDisposible())
                Assert.IsTrue(dispMustex.IsLocked);
        }


        /// <summary>
        /// Wait timeout - wit disposable
        /// </summary>
        [TestMethod]
        public void TestDelayedTimedTimeoutDisposable()
        {
            var mutex = new PinkoMutex();
            var ev = new ManualResetEvent(false);

            Task.Factory.StartNew(() =>
            {
                using (mutex.LockDisposible())
                    ev.WaitOne();
            });

            Thread.Sleep(1000);

            // should be locked by the worker thread above
            using (var dispMustex = mutex.LockDisposible(500))
                Assert.IsFalse(dispMustex.IsLocked);

            ev.Set();
            Thread.Sleep(1000);

            // should be available now
            using (var dispMustex = mutex.LockDisposible(500))
                Assert.IsTrue(dispMustex.IsLocked);

        }
    }
}
