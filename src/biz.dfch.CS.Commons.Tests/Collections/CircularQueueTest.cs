using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using biz.dfch.CS.Commons.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Collections
{
    [TestClass]
    public class CircularQueueTest
    {
        public const int CAPACITY = 27;

        [TestMethod]
        public void EmptyTryDequeueReturnsFalse()
        {
            var sut = new CircularQueue<string>();
            var item = default(string);

            for (var c = 0; c < 1000*1000; c++)
            {
                var result = sut.TryDequeue(out item);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void EmptyTryPeekReturnsFalse()
        {
            var sut = new CircularQueue<string>();
            var item = default(string);

            for (var c = 0; c < 1000*1000; c++)
            {
                var result = sut.TryPeek(out item);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void EnqueueWithTryDequeueCirculates()
        {
            var sut = new CircularQueue<string>(CAPACITY);

            for (var c = 0; c < 1000*1000; c++)
            {
                var baseItem = "item" + c;

                for (var i = c; i < c + CAPACITY; i++)
                {
                    var item = "item" + i;
                    sut.Enqueue(item);

                    string enqueuedItem;

                    var result = sut.TryPeek(out enqueuedItem);
                    
                    Assert.IsTrue(result);
                    Assert.AreEqual(baseItem, enqueuedItem);
                }

                c += CAPACITY;
            }
        }

        [TestMethod]
        public void SingleEnqueueSingleDequeueSucceeds()
        {
            var stateInfo = new StateInfo
            {
                CircularQueue = new CircularQueue<string>(CAPACITY),
                Count = 1000*1000
            };

            ThreadPool.QueueUserWorkItem(new WaitCallback(DequeueProc), stateInfo);

            Thread.Sleep(1 * 1000);

            EnqueueProc(stateInfo);

            Trace.WriteLine(string.Format("EnqueueOperations '{0}'", stateInfo.EnqueueOperations));
            Trace.WriteLine(string.Format("DequeueOperations '{0}'", stateInfo.DequeueOperations));
        }

        public class StateInfo
        {
            public CircularQueue<string> CircularQueue { get; set; }

            public int Count;
            public int EnqueueOperations;
            public int DequeueOperations;
        }
            
        public static void EnqueueProc(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var info = stateInfo as StateInfo;
            Contract.Assert(null != info);

            for (var c = 0; c < info.Count; c++)
            {
                ++info.EnqueueOperations;

                var item = "item" + info.EnqueueOperations;

                info.CircularQueue.Enqueue(item);
            }

            info.EnqueueOperations = info.Count;
        }

        public static void DequeueProc(object stateInfo)
        {
            Contract.Requires(null != stateInfo);
            var info = stateInfo as StateInfo;
            Contract.Assert(null != info);

            var sw = Stopwatch.StartNew();

            do
            {
                string item;
                var result = info.CircularQueue.TryDequeue(out item);

                if (!result)
                {
                    continue;
                }

                info.DequeueOperations++;
            }
            while (info.Count >= info.DequeueOperations || info.Count == info.EnqueueOperations);
        }

        [TestMethod]
        public void TryDequeueWithWaitTimeSucceeds()
        {
            var enqueuedItem = "arbitrary-string";
            string item;
            var sut = new CircularQueue<string>(CAPACITY);

            var waitTimeoutMs1 = 1000;
            var sw1 = Stopwatch.StartNew();

            // try dequeue inexisting item and wait for timeout
            var result = sut.TryDequeue(out item, waitTimeoutMs1);

            sw1.Stop();
            Assert.IsFalse(result);
            Assert.IsTrue(waitTimeoutMs1 <= sw1.ElapsedMilliseconds);

            // dequeue existing item
            sut.Enqueue(enqueuedItem);

            var waitTimeoutMs2 = 60*1000;
            var sw2 = Stopwatch.StartNew();

            result = sut.TryDequeue(out item, waitTimeoutMs2);

            sw2.Stop();
            Assert.IsTrue(result);
            Assert.IsTrue(waitTimeoutMs2 > sw2.ElapsedMilliseconds);
            Assert.AreEqual(enqueuedItem, item);

        }

    }
}
