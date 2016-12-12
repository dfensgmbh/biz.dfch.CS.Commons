using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using biz.dfch.CS.Commons.Collections;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Collections
{
    [TestClass]
    public class CircularQueueTest
    {
        public const int CAPACITY = 32;

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
            var capacity = CAPACITY;

            var sut = new CircularQueue<string>(capacity);

            const int count = 1000*1000;
            for (var c = 0; c < count; c++)
            {
                for (var i = c; i < c + capacity; i++)
                {
                    var item = "item" + i;
                    sut.Enqueue(item);

                    string enqueuedItem;

                    var result = sut.TryPeek(out enqueuedItem);

                    var baseItem = "item" + sut.DiscardedItems;

                    Assert.IsTrue(result, c + "; " + i);
                    Assert.AreEqual(baseItem, enqueuedItem, c + "; " + i);
                }

                c += capacity -1;
            }

            Assert.AreEqual(count - (ulong) capacity, sut.DiscardedItems);
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

        private volatile int enqueuePointer;
        private volatile int capacity;

        [TestMethod]
        public void TestWithAndXorModulo()
        {
            const int cStart = 0;
            const int cCount = 1000 * 1000;

            Stopwatch sw;

            enqueuePointer = 0;
            capacity = 4096;

            sw = Stopwatch.StartNew();
            for (var c = cStart; c < cStart + cCount; c++)
            {
                if (0 != (++enqueuePointer & capacity))
                {
                    enqueuePointer = 0;
                }
            }
            sw.Stop();

            Trace.WriteLine(string.Format("Count {0}'. Ticks/Count {1}. Ticks {2}. ms {3}", cCount, sw.ElapsedTicks / cCount, sw.ElapsedTicks, sw.ElapsedMilliseconds));

            sw = Stopwatch.StartNew();
            for (var c = cStart; c < cStart + cCount; c++)
            {
                if (0 == (++enqueuePointer ^ capacity))
                {
                    enqueuePointer = 0;
                }
            }
            sw.Stop();

            Trace.WriteLine(string.Format("Count {0}'. Ticks/Count {1}. Ticks {2}. ms {3}", cCount, sw.ElapsedTicks / cCount, sw.ElapsedTicks, sw.ElapsedMilliseconds));
            enqueuePointer = 0;
            capacity = 4096;

            sw = Stopwatch.StartNew();
            for (var c = cStart; c < cStart + cCount; c++)
            {
                enqueuePointer = ++enqueuePointer % capacity;
            }
            sw.Stop();

            Trace.WriteLine(string.Format("Count {0}'. Ticks/Count {1}. Ticks {2}. ms {3}", cCount, sw.ElapsedTicks / cCount, sw.ElapsedTicks, sw.ElapsedMilliseconds));
        }

        [TestMethod]
        public void AssertAndXorModulo()
        {
            const int cStart = 0;
            const int cCount = 1000*1000;

            var andPointer = 0;
            var xorPointer = 0;
            var modPointer = 0;
            capacity = 4096;

            for (var c = cStart; c < cStart + cCount; c++)
            {
                if (0 != (++andPointer & capacity))
                {
                    andPointer = 0;
                }

                if (0 == (++xorPointer ^ capacity))
                {
                    xorPointer = 0;
                }

                modPointer = ++modPointer % capacity;

                Assert.AreEqual(andPointer, xorPointer);
                Assert.AreEqual(modPointer, xorPointer);
            }
        }

        private volatile ManualResetEventSlim eventSlim;
        private volatile int interlocked;

        [TestMethod]
        public void CompareManualResetEventSlimAndInterlocked()
        {
            eventSlim = new ManualResetEventSlim(false);
            interlocked = 0;

            Stopwatch sw;
            const int cStart = 0;
            const int cCount = 1000*1000;

            sw = Stopwatch.StartNew();
            for (var c = cStart; c < cStart + cCount; c++)
            {
                eventSlim.Set();

                if (eventSlim.IsSet)
                {
                    eventSlim.Reset();
                }
            }
            sw.Stop();
            Trace.WriteLine(string.Format("Count {0}'. Ticks/Count {1}. Ticks {2}. ms {3}", cCount, sw.ElapsedTicks / cCount, sw.ElapsedTicks, sw.ElapsedMilliseconds));

            sw = Stopwatch.StartNew();
            for (var c = cStart; c < cStart + cCount; c++)
            {
                Interlocked.Increment(ref interlocked);

                if (0 != interlocked)
                {
                    Interlocked.Decrement(ref interlocked);
                    //Interlocked.CompareExchange(ref interlocked, 0, 0);
                }
            }
            sw.Stop();
            Trace.WriteLine(string.Format("Count {0}'. Ticks/Count {1}. Ticks {2}. ms {3}", cCount, sw.ElapsedTicks / cCount, sw.ElapsedTicks, sw.ElapsedMilliseconds));
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Precondition.+2.+capacity")]
        public void CircularQueueWithCapacity0ThrowsContractException()
        {
            var sut = new CircularQueue<string>(0);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Precondition.+2.+capacity")]
        public void CircularQueueWithCapacity1ThrowsContractException()
        {
            var sut = new CircularQueue<string>(1);
        }

        [TestMethod]
        public void CircularQueueWithCapacity3Succeeds()
        {
            var sut = new CircularQueue<string>(3);
        }

        [TestMethod]
        public void CircularQueueWithCapacity7Succeeds()
        {
            var sut = new CircularQueue<string>(7);
        }

        [TestMethod]
        public void CircularQueueWithCapacity4096Succeeds()
        {
            var sut = new CircularQueue<string>(4096);
        }
    }
}
