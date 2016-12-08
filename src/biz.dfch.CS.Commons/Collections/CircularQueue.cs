/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;

namespace biz.dfch.CS.Commons.Collections
{
    public class CircularQueue<T>
        where T : class
    {
        public const int CAPACITY_DEFAULT = 16;

        private volatile bool isInitialised;

        private int enqueuePointer;
        private int dequeuePointer;
        private int enqueuedItems;
        private readonly int capacity;
        public int Capacity
        {
            get { return capacity; }
        }

        private readonly List<T> list;

        private readonly object _lock = new object();

        private ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(false);

        public CircularQueue()
            : this(CAPACITY_DEFAULT)
        {
            // nothing to be done here
        }

        public CircularQueue(int capacity)
        {
            Contract.Requires(0 < capacity);

            list = new List<T>(capacity);
            this.capacity = capacity;
        }

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                if (!isInitialised)
                {
                    list.Add(item);

                    isInitialised = 0 == (enqueuePointer = ++enqueuePointer % capacity);
                }
                else
                {
                    list[enqueuePointer] = item;

                    enqueuePointer = ++enqueuePointer % capacity;
                }

                enqueuedItems++;
                manualResetEventSlim.Set();
            }
        }

        public bool TryDequeue(out T item, int waitTimeoutMs)
        {
            var result = TryDequeue(out item);
            if (result)
            {
                return true;
            }

            result = manualResetEventSlim.Wait(waitTimeoutMs);
            if (!result)
            {
                return false;
            }

            manualResetEventSlim.Reset();

            result = TryDequeue(out item);
            return result;
        }

        public bool TryDequeue(out T item)
        {
            lock (_lock)
            {
                if (0 >= enqueuedItems)
                {
                    item = default(T);
                    return false;
                }

                item = list[dequeuePointer];

                dequeuePointer = ++dequeuePointer % capacity;

                enqueuedItems--;

                return true;
            }
        }

        public bool TryPeek(out T item)
        {
            lock (_lock)
            {
                if (0 >= enqueuedItems)
                {
                    item = default(T);
                    return false;
                }

                item = list[dequeuePointer];

                return true;
            }
        }
    }
}
